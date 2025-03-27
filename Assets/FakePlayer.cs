
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakePlayer : BaseEnemy
{
    private Slingshot slingshot;

    // Public params to alter training configs
    public float setHealth;
    public float setSpeed;

    protected override void Start()
    {
        speed = setSpeed != 0 ? setSpeed : 5f;
        acceleration = 8f;
        maxHealth = setHealth != 0 ? setHealth : 80f;
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

    protected override void HandleDeath()
    {
        floor.GetComponent<ColorChange>().ChangeMaterialGreen();
        base.HandleDeath();
    }
}