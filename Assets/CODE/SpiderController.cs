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
            Debug.Log("Ranged Enemy shooting!");
            slingshot.ShootProjectile();
            lastShotTime = Time.time;
        }
    }
}
