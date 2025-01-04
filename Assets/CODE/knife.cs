using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class knife : MonoBehaviour
{
    // Attacking
    public GameObject knifePrefab;      // Knife prefab
    public Transform attackPoint;       // Position where the knife attack spawns
    public float attackRange = 1.5f;    // Range of the knife attack
    public float attackCooldown = 1f;   // Cooldown between attacks

    private float nextAttackTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextAttackTime)
        {
            PerformKnifeAttack();
            nextAttackTime = Time.time + attackCooldown; // Set next attack time
        }
    }

    void PerformKnifeAttack()
    {
        // Short range damage
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Debug.Log("Hit enemy: " + enemy.name);
                enemy.GetComponent<EnemyController>().TakeDamage(20);
            }
        }

        // Visual effect
        GameObject knife = Instantiate(knifePrefab, attackPoint.position, Quaternion.identity);
        Destroy(knife, 0.5f); // Destroy the visual effect after 0.5 seconds
    }
}
