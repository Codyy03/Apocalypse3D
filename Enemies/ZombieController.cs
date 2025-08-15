using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    [Tooltip("Jak daleko ma sie znajdowaæ od gracza aby szed³ w jego strone")]
    [SerializeField] float distanceToWalk;

    [Tooltip("Jak daleko ma sie znajdowaæ od gracza aby atakowa³")]
    [SerializeField] float distanceToAttack;

    [SerializeField] float patrolRadius = 5f;
    [SerializeField] float patrolDelay = 6f;

    private float patrolTimer = 0f;
    private Vector3 patrolOrigin;

    public float distance;
    public float maxChaseDistance = 10f;

    public GameObject mapMark;
    enum ZombieStartState
    { 
        Idle,
        Walk
    }
    [SerializeField] ZombieStartState startState;

    enum ZombieGoToPlayer
    {
        Walk,
        Run
    }

    [Tooltip("Aktywnoœæ zobmie podczas zbil¿ania siê do gracza")]
    [SerializeField] ZombieGoToPlayer zombieGoToPlayerState;

    float walkSpeed;
    [SerializeField] float runningSpeed;

    bool isHit = false;
    float hitDuration = 0f;
    float hitTimer = 0f;

    [SerializeField] Transform player;
    NavMeshAgent agent;

    ZombieAnimationsController animationsController;

    [Header("Parametry uderzenia")]
    [SerializeField] float fadeDuration = 0.5f;

    [Header("DŸwiêki")]
    [SerializeField] AudioClip walkSound, walkingTowardsPlayerSound,runingSound, idleSound;
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip attackSound;

    AudioSource walkAudioSource; // do loopów: chodzenie, idle
    AudioSource fxAudioSource;   // do efektów: uderzenie, atak

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animationsController = GetComponent<ZombieAnimationsController>();

        // Domyœlny AudioSource z inspektora jako walking
        walkAudioSource = GetComponent<AudioSource>();
        walkAudioSource.loop = true;
        walkAudioSource.spatialBlend = 1f;
        walkAudioSource.volume = Random.Range(0.5f, 1f);

        // FX AudioSource do dŸwiêków jednorazowych
        fxAudioSource = gameObject.AddComponent<AudioSource>();
        fxAudioSource.spatialBlend = 1f;

        patrolOrigin = transform.position;

        walkSpeed = agent.speed;

        agent.updateRotation = false;
    }
    void Update()
    {
        if (isHit)
        {
            hitTimer -= Time.deltaTime;
            if (hitTimer <= 0f)
            {
                isHit = false;
                agent.isStopped = false;
            }
            return;
        }

        // rotacja zombie
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 direction = agent.velocity.normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // dystans do gracza
        distance = Vector3.Distance(transform.position, player.position);

        if (distance > distanceToWalk)
        {
            if(ZombieStartState.Idle == startState)
            {
                // Gracz za daleko. 
                animationsController.ChangeAnimation(animationsController.idle);
                agent.ResetPath();
                PlayWalkSound(idleSound);

                if (walkAudioSource.isPlaying && walkAudioSource.clip != idleSound)
                {
                    StartCoroutine(FadeOutAudio(walkAudioSource, fadeDuration));
                }
            }
            else
            {
                // Zombie nie widzi gracza, patroluje
                animationsController.ChangeAnimation(animationsController.walk);
                agent.speed = walkSpeed;
                patrolTimer -= Time.deltaTime;
                if (patrolTimer <= 0f || agent.remainingDistance <= 0.5f)
                {
                    Vector3 newPatrolPoint = GetRandomPointInPatrolRadius();
                    agent.SetDestination(newPatrolPoint);
                    patrolTimer = patrolDelay + Random.Range(-2f, 2f);
                }

                PlayWalkSound(walkSound);
            }
        }
        else if (distance > distanceToAttack)
        {
            agent.SetDestination(player.position);
         
            if(zombieGoToPlayerState == ZombieGoToPlayer.Walk)
            {
                animationsController.ChangeAnimation(animationsController.walk);
                PlayWalkSound(Random.Range(0, 2) == 0 ? walkSound : walkingTowardsPlayerSound);

                agent.speed = walkSpeed;
            }
            else
            {
                animationsController.ChangeAnimation(animationsController.run);
                PlayWalkSound(runingSound);

                if(agent.speed == walkSpeed)
                {
                    agent.speed = runningSpeed;
                }
            }
        }
        else
        {
            // Atak
            animationsController.ChangeAnimation(animationsController.attack);
            agent.ResetPath();

            if (walkAudioSource.isPlaying)
            {
                StartCoroutine(FadeOutAudio(walkAudioSource, fadeDuration));
            }
        }
    }
    void PlayWalkSound(AudioClip sound)
    {
        if (!walkAudioSource.isPlaying || sound != walkAudioSource.clip)
        {
            walkAudioSource.clip = sound;
            walkAudioSource.Play();
        }
    }
    public void TriggerHitAnimation()
    {
        if (isHit) return;

        var clip = animationsController.GetAnimator()
            .runtimeAnimatorController
            .animationClips
            .FirstOrDefault(c => c.name == animationsController.hit);

        hitDuration = clip != null ? clip.length : 0.5f;
        agent.isStopped = true;

        isHit = true;
        hitTimer = hitDuration;

        animationsController.ChangeAnimation(animationsController.hit);

        if (walkAudioSource.isPlaying)
        {
            StartCoroutine(FadeOutAudio(walkAudioSource, fadeDuration));
        }

        PlayFX(hitSound);
    }

    // Wywo³ywana jako Event z animacji ataku
    public void PlayAttackSound() => PlayFX(attackSound);
    void PlayFX(AudioClip clip)
    {
        fxAudioSource.Stop();
        fxAudioSource.volume = 1f;
        fxAudioSource.PlayOneShot(clip);
    }
    IEnumerator FadeOutAudio(AudioSource source, float duration)
    {
        float startVolume = source.volume;

        while (source.volume > 0f)
        {
            source.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }
    Vector3 GetRandomPointInPatrolRadius()
    {
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        Vector3 target = patrolOrigin + new Vector3(randomCircle.x, 0, randomCircle.y);
        NavMeshHit hit;

        if (NavMesh.SamplePosition(target, out hit, 1.5f, NavMesh.AllAreas))
            return hit.position;

        return transform.position;
    }

    public void SetMarkMapVisibility(bool visible) => mapMark.SetActive(visible);
    
}


