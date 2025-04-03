using System;
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
        attackRangeClose = 2f;

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

    private void Reset()
    {
        // Reset health
        System.Random rng = new System.Random();
        int rand1 = rng.Next(1, 50);

        health = rand1;
        Debug.Log($"Health set to {health}");
        healthbar.SetHealth(maxHealth);

        // Reset position (TODO: randomize position (currently annoying to do due to rotation))
        transform.position = new Vector3(-12.2f, -4f, -0.5f);

        // Reset FSM to initial state
        fsm.SetStartState("Wander");
        fsm.Init();

        // Reset other attributes
        lastShotTime = -1;
    }

    protected override void HandleDeath()
    {
        floor.GetComponent<ColorChange>().ChangeMaterialGreen();
        Reset();
    }
}
