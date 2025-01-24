using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class knife : MonoBehaviour
{
    // Attacking
    public GameObject knifePrefab;      // Knife prefab
    public Transform attackPoint;       // Position where the knife attack spawns
    public float attackRange = 1.5f;    // Range of the knife attack
    public float attackCooldown;   // Cooldown between attacks

    private float nextAttackTime = 0f;
    float lastAttack;

    // Start is called before the first frame update
    void Start()
    {
        lastAttack = Time.time - attackCooldown;
    }

    //Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextAttackTime)
        {
            PerformKnifeAttack();
            nextAttackTime = Time.time + attackCooldown; // Set next attack time
        }

    }

    public void PerformKnifeAttack()
    {

        if (Time.time - lastAttack > attackCooldown)
        { //cooldown system

            lastAttack = Time.time;

            Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange);
            foreach (Collider enemy in hitEnemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    Debug.Log("Hit enemy: " + enemy.name);
                    enemy.GetComponent<EnemyController>().TakeDamage(20);
                }
            }

            //Debug.Log("attack");
            AttackEffect();
            return;
        }
        else
        {
            //Debug.Log("dont attack");
            return;
        }
    }





    //    Short range damage
    //    Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange);
    //    foreach (Collider enemy in hitEnemies)
    //    {
    //        if (enemy.CompareTag("Enemy"))
    //        {
    //            Debug.Log("Hit enemy: " + enemy.name);
    //            enemy.GetComponent<EnemyController>().TakeDamage(20);
    //        }
    //    }

    //    Visual effect
    //    GameObject knife = Instantiate(knifePrefab, attackPoint.position, Quaternion.identity);
    //    Destroy(knife, 1f); // Destroy the visual effect after 1 seconds

    //}

    public void AttackEffect(){ //particle effects
        GameObject effect = Instantiate(knifePrefab,attackPoint.position,Quaternion.identity);
        Destroy(effect,1f);
    }
}
