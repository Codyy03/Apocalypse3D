using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;

    [SerializeField] AudioClip dyingSound;
    private float health;

    private AnimationsController animations;
    private ZombieController zombieController;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private AudioSource[] sources = new AudioSource[2];
    private SphereCollider attackCollider;

    void Awake()
    {
        health = maxHealth;
        animations = GetComponent<AnimationsController>();
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

    private void HandleDeath()
    {
        agent.enabled = false;
        rb.useGravity = false;
        capsuleCollider.enabled = false;
        sources[1].PlayOneShot(dyingSound);
        attackCollider.enabled = false;
        zombieController.enabled = false;
        sources[0].enabled = false;

        animations.ChangeAnimation(animations.dead);

        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(15f);
        Destroy(gameObject);
    }
}