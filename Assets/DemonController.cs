
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonController : BaseEnemy
{
    private Slingshot slingshot;

    protected override void Start()
    {
        speed = 5f;
        acceleration = 8f;
        maxHealth = 80f;
        vision = 25f;
        attackRangeRanged = 15f;
        retreatThreshold = 30f;
        aggressiveness = 0.5f;
        blockRate = 0.001f; // Chance to block each frame

        slingshot = GetComponentInChildren<Slingshot>();
        base.Start();
    }

    protected override void AttackRangedBehavior()
    {
        base.AttackRangedBehavior();
        if (lastShotTime < 0 || (Time.time - lastShotTime) >= 2f)
        {
            Debug.Log("Attacking Ranged!");

            if (target != null)
            {
                Vector3 pos = target.position;
                Debug.Log($"Shooting target Position: x={pos.x}, y={pos.y}, z={pos.z}");
                slingshot.ShootProjectile();
            }
            else
            {
                Debug.LogWarning("Target is null!");
            }

            lastShotTime = Time.time;
        }
    }
}