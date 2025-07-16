using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackPoint : MonoBehaviour
{
    public LayerMask playerLayer;
    public float radius=1f,damage;


    // Update is called once per frame
    void Update()
    {
        AddDamage();
    }
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
