using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;

    [SerializeField] AudioClip dyingSound;
    float health;

    ZombieAnimationsController animations;
    ZombieController zombieController;
    NavMeshAgent agent;
    Rigidbody rb;
    CapsuleCollider capsuleCollider;
    AudioSource[] sources = new AudioSource[2];
    SphereCollider attackCollider;

    public UnityEvent onDeadEvent;

    void Awake()
    {
        health = maxHealth;
        animations = GetComponent<ZombieAnimationsController>();
        zombieController = GetComponent<ZombieController>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        attackCollider = GetComponentInChildren<SphereCollider>();
    }
    private void Start()
    {
        sources = GetComponents<AudioSource>();
    }
    /// <summary>
    /// zmieñ poziom ¿ycia zombie
    /// </summary>
    /// <param name="value">wartoœæ ¿ycia do zmiany</param>
    public void ChangeHealth(float value)
    {
        health = Mathf.Clamp(health + value, 0f, maxHealth);
        if (health <= 0f)
        {
            HandleDeath();
            return;
        }

        if (health <= maxHealth * 0.2f)
        {
            zombieController.TriggerHitAnimation();
        }
    }
    /// <summary>
    /// obs³u¿ œmieræ zombie, gdzie ¿ycie >= 0
    /// </summary>
    private void HandleDeath()
    {
        agent.enabled = false;
        rb.useGravity = false;
        capsuleCollider.enabled = false;

        sources[1].PlayOneShot(dyingSound);

        attackCollider.enabled = false;

        zombieController.enabled = false;

        sources[0].enabled = false;

        onDeadEvent?.Invoke();

        animations.ChangeAnimation(animations.dead);

        zombieController.mapMark.SetActive(false);

        StartCoroutine(DestroyAfterDelay());
    }
    /// <summary>
    /// poczekaj 15 sekund nastepnie zniszcz obiekt zombie
    /// </summary>
    /// <returns></returns>
    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(15f);
        Destroy(gameObject);
    }
}