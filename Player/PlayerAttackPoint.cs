using UnityEngine;

public class PlayerAttackPoint : MonoBehaviour
{
    public byte attackType;
    [SerializeField] float radius;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float damage;

    [SerializeField] GameObject bloodPrefab;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;

    [SerializeField] AudioClip firstAttackMissSound;
    [SerializeField] AudioClip firstAttackHitSound;

    [SerializeField] AudioClip secondAttackMissSound;
    [SerializeField] AudioClip secondAttackHitSound;

    private void Update()
    {
       Attack();
    }
    public void Attack()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyLayer);

        if (hits.Length == 0)
        {
            if (!audioSource.isPlaying)
                audioSource.PlayOneShot(attackType == 1 ? firstAttackMissSound : secondAttackMissSound);

            return;
        }
        
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(attackType == 1 ? firstAttackHitSound : secondAttackHitSound);

        foreach (var hit in hits)
        {
            hit.transform.GetComponent<EnemyHealth>()?.ChangeHealth(-damage);

            GameObject blood = Instantiate(bloodPrefab, hit.transform.position, transform.rotation);

            blood.GetComponent<ParticleSystem>().Play();
           
        }
        gameObject.SetActive(false);
        
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
