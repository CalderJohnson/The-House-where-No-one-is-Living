using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : BaseEnemy
{
    private Slingshot slingshot;

    protected override void Start()
    {
        speed = 4f;
        maxHealth = 80f;
        vision = 25f;
        attackRangeRanged = 15f;
        retreatThreshold = 30f;

        slingshot = GetComponentInChildren<Slingshot>();
        base.Start();
    }

    protected override void AttackRangedBehavior()
    {
        if (lastShotTime < 0 || (Time.time - lastShotTime) >= 2f)
        {
            Debug.Log("Attacking Ranged!");

            if (target != null)
            {
                Vector3 pos = target.position;
                Debug.Log($"Target Position: x={pos.x}, y={pos.y}, z={pos.z}");
            }
            else
            {
                Debug.LogWarning("Target is null!");
            }

            lastShotTime = Time.time;
        }
    }
}
