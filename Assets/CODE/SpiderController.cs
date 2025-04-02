using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : BaseEnemy
{
    private Slingshot slingshot;
    private int deathCount;
    public bool training = false;

    protected override void Start()
    {
        speed = 5f;
        acceleration = 8f;
        maxHealth = 80f;
        vision = 25f;
        attackRangeClose = 2f;

        attackRangeRanged = 15f;
        retreatThreshold = 30f;
        aggressiveness = 0.5f;
        blockRate = 0.001f; // Chance to block each frame

        deathCount = 0;

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

    private void Reset()
    {
        // Reset health
        health = maxHealth;
        healthbar.SetHealth(maxHealth);

        // Reset position (TODO: randomize position (currently annoying to do due to rotation))
        transform.position = new Vector3(-12.2f, -4f, -0.5f);

        // Reset to default stats every 10 deaths
        deathCount++;
        if (deathCount % 10 == 0)
        {
            attackRangeRanged = 15f;
            retreatThreshold = 30f;
            aggressiveness = 0.5f;
            blockRate = 0.001f; // Chance to block each frame
        }

        // Reset FSM to initial state
        fsm.SetStartState("Wander");
        fsm.Init();

        // Reset other attributes
        lastShotTime = -1;
    }

    protected override void HandleDeath()
    {
        if (training)
        {
            floor.GetComponent<ColorChange>().ChangeMaterialRed();
            Reset();
        }
        else
        {
            base.HandleDeath();
        }
    }
}
