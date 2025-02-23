using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10; // Damage dealt by the projectile

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Healthbar>().TakeDamage(damage);
            Debug.Log("Ranged hit enemy");
            Destroy(gameObject); // Destroy the projectile on impact
        }
    }
}
