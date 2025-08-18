using UnityEngine;

public class ZombieAttackPoint : MonoBehaviour
{
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float radius=1f,damage;

    void Update()
    {
        AddDamage();
    }

    /// <summary>
    /// zadaj obra¿enia graczowi
    /// </summary>
    void AddDamage()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, radius, playerLayer);
        if(hit.Length>0)
        {
            hit[0].GetComponent<PlayerHealth>().ChangePlayerHealth(-damage);
            gameObject.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
