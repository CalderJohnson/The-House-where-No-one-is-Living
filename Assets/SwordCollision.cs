using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    public int damage = 10; // Damage dealt by the projectile
    public bool attackActive = false;

    public void attackStart(){
        attackActive = true;
    }
    public void attackEnd(){
        attackActive = false;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && attackActive ==true)
        {
            collision.gameObject.GetComponent<Healthbar>().TakeDamage(damage);
            // Debug.Log("Ranged hit enemy");
            //Destroy(gameObject); // Destroy the projectile on impact
        }
    }
}
