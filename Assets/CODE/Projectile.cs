using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10; // Damage dealt by the projectile

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyController>().TakeDamage(damage);
            Destroy(gameObject); // Destroy the projectile on impact
        }
    }
}
