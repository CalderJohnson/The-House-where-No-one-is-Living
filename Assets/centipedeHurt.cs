using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class centipedeHurt : MonoBehaviour
{
    public int damage = 3; // Damage dealt by the projectile

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Healthbar>().TakeDamage(damage);
            // Debug.Log("Ranged hit enemy");
            
            //Destroy(gameObject); // Destroy the projectile on impact
        }
    }
}
