using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    enum ZombieStartState
    {
        Idle,
        Walk
    }
    enum ZombieGoToPlayer
    {
        Walk,
        Run
    }

    [Tooltip("Jak daleko ma sie znajdowaæ od gracza aby szed³ w jego strone")]
    [SerializeField] float distanceToWalk;
    [SerializeField] float distanceToWalkAfterZombieSawPlayer = 20f;
    float defaultDistanceToWalk;

    [Tooltip("Jak daleko ma sie znajdowaæ od gracza aby atakowa³")]
    [SerializeField] float distanceToAttack;

    [SerializeField] float patrolRadius = 5f;
    [SerializeField] float patrolDelay = 6f;

    float patrolTimer = 0f;
    Vector3 patrolOrigin;

    float distance;

    public GameObject mapMark;

    [SerializeField] ZombieStartState startState;

    [Tooltip("Aktywnoœæ zobmie podczas zbil¿ania siê do gracza")]
    [SerializeField] ZombieGoToPlayer zombieGoToPlayerState;

    float walkSpeed;
    [SerializeField] float runningSpeed;

    [SerializeField] float noiseAlertDuration = 5f;

    bool isHit = false;
    float hitDuration = 0f;
    float hitTimer = 0f;

    bool zombieFolowPlayer;

    Transform player;
    NavMeshAgent agent;

    ZombieAnimationsController animationsController;

    [Header("Parametry uderzenia")]
    [SerializeField] float fadeDuration = 0.5f;

    [Header("DŸwiêki")]
    [SerializeField] AudioClip walkSound, walkingTowardsPlayerSound, runingSound, idleSound;
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip attackSound;

    AudioSource walkAudioSource; // do loopów: chodzenie, idle
    AudioSource fxAudioSource;   // do efektów: uderzenie, atak

    AudioClip randomWalkingSound;

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

        defaultDistanceToWalk = distanceToWalk;

        randomWalkingSound = Random.Range(0, 2) == 0 ? walkSound : walkingTowardsPlayerSound;
    }
    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>().transform;
    }
    private void OnEnable()
    {
        NoiseSystem.OnNoise += ReactToNoise;
    }
    private void OnDisable()
    {
        NoiseSystem.OnNoise -= ReactToNoise;
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

        HandleRotation();

        // dystans do gracza
        distance = Vector3.Distance(transform.position, player.position);

        if (distance > distanceToWalk)
        {
            HandlePlayerTooFar();
        }
        else if (distance > distanceToAttack)
        {
            HandleZombieMoveTorwardsPlayer();
        }
        else
        {
            HandleAttack();
        }
    }
    void HandleRotation()
    {
        // rotacja zombie
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 direction = agent.velocity.normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
    void HandlePlayerTooFar()
    {
        if (ZombieStartState.Idle == startState)
        {
            // Gracz za daleko. 
            if (zombieFolowPlayer)
            {
                animationsController.ChangeAnimation(animationsController.lookAround);
            }
            else
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
    void HandleZombieMoveTorwardsPlayer()
    {
        agent.SetDestination(player.position);

        zombieFolowPlayer = true;

        if (distanceToWalk != distanceToWalkAfterZombieSawPlayer)
        {
            distanceToWalk = distanceToWalkAfterZombieSawPlayer;
            defaultDistanceToWalk = distanceToWalkAfterZombieSawPlayer;
        }

        if (zombieGoToPlayerState == ZombieGoToPlayer.Walk)
        {
            animationsController.ChangeAnimation(animationsController.walk);

            PlayWalkSound(randomWalkingSound);

            agent.speed = walkSpeed;
        }
        else
        {
            animationsController.ChangeAnimation(animationsController.run);
            PlayWalkSound(runingSound);

            if (agent.speed == walkSpeed)
            {
                agent.speed = runningSpeed;
            }
        }
    }
    void HandleAttack()
    {
        // Atak
        animationsController.ChangeAnimation(animationsController.attack);
        agent.ResetPath();

        if (walkAudioSource.isPlaying)
        {
            StartCoroutine(FadeOutAudio(walkAudioSource, fadeDuration));
        }
    }

    void ReactToNoise(Vector3 noisePosition, float noiseRange)
    {
        float dist = Vector3.Distance(transform.position, noisePosition);

        if (dist <= noiseRange)
        {
            // zwieksza tymczasowo zasieg widzenia
            distanceToWalk = Mathf.Max(distanceToWalk, noiseRange);

            // ustawia cel pod¹¿ania na punkt dzwiêku
            agent.SetDestination(noisePosition);

            StartCoroutine(NoiseAlertRoutine(noiseRange));
        }
    }
    IEnumerator NoiseAlertRoutine(float newDistance)
    {
        distanceToWalk = newDistance;

        yield return new WaitForSeconds(noiseAlertDuration);

        distanceToWalk = defaultDistanceToWalk;

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

    public void DisableLookingAround() => zombieFolowPlayer = false;

}


