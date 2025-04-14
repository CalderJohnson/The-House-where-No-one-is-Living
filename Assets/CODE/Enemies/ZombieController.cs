using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : TrainableEnemy
{
    protected override void Start()
    {
        // Zombie specific parameters
        attackRangeRanged = -1f; // Zombie only has a melee attack
        retreatThreshold = 30f;
        aggressiveness = 0.5f;
        blockRate = 0.001f;

        maxHealth = 50f;
        vision = 25f;
        attackRangeClose = 2f;
        speed = 9f;

        base.Start();
    }

    protected override void AttackCloseBehavior()
    {
        base.AttackCloseBehavior();
        if (lastAttackTime < 0 || (Time.time - lastAttackTime) >= 2f)
        {
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRangeClose);
            foreach (Collider enemy in hitEnemies)
            {
                if (enemy.CompareTag("Player"))
                {
                    Debug.Log("Hit enemy: " + enemy.name);
                    enemy.GetComponent<Healthbar>().TakeDamage(25 + (10 * (int)Math.Round(aggressiveness - 0.5))); // Aggressiveness modifies damage output (+/- 5 points)
                }
            }
            lastAttackTime = Time.time;
        }
    }
}
