using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakePlayer : TrainableEnemy
{
    private Slingshot slingshot;

    protected override void Start()
    {
        // Fake player specific parameters
        attackRangeRanged = 15f;
        retreatThreshold = 30f;
        aggressiveness = 0.5f;
        blockRate = 0.001f; // Chance to block each frame

        maxHealth = 80f;
        vision = 25f;
        attackRangeClose = 2f;
        speed = 7f;

        // Ranged attacking
        slingshot = GetComponentInChildren<Slingshot>();
        base.Start();
    }

    protected override void AttackRangedBehavior()
    {
        base.AttackRangedBehavior();
        if (lastShotTime < 0 || (Time.time - lastShotTime) >= 2f)
        {
            //Debug.Log("Attacking Ranged!");

            if (target != null)
            {
                Vector3 pos = target.position;
                //Debug.Log($"Shooting target Position: x={pos.x}, y={pos.y}, z={pos.z}");
                slingshot.ShootProjectile();
            }
            else
            {
                //Debug.LogWarning("Target is null!");
            }

            lastShotTime = Time.time;
        }
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
                    enemy.GetComponent<Healthbar>().TakeDamage(20 + (10 * (int)Math.Round(aggressiveness - 0.5))); // Aggressiveness modifies damage output (+/- 5 points)
                }
            }
            lastAttackTime = Time.time;
        }
    }

    protected override void HandleDeath()
    {
        floor.GetComponent<ColorChange>().ChangeMaterialGreen();
        base.Reset();
    }
}
