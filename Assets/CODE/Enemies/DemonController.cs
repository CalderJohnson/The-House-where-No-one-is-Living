using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonController : TrainableEnemy
{
    private Slingshot slingshot;

    protected override void Start()
    {
        // Demon specific parameters
        attackRangeRanged = 15f;
        retreatThreshold = 30f;
        aggressiveness = 0.5f;
        blockRate = 0.001f;

        maxHealth = 80f;
        vision = 25f;
        attackRangeClose = 2f;
        speed = 7f;

        // Demon has a long ranged attack
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
}
