using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrainableEnemy : BaseEnemy
{
    public float setHealth;
    public float setSpeed;
    public bool training; // Training mode toggle
    public GameObject floor = null; // For training, set floor colour to indicate win/loss
    private int deathCount;

    protected override void Start()
    {
        speed = setSpeed != 0 ? setSpeed : 5f;
        acceleration = 8f;
        maxHealth = setHealth != 0 ? setHealth : 80f;
        vision = 25f;
        attackRangeClose = 2f;

        deathCount = 0;

        base.Start();
    }

    protected void Reset()
    {
        // Reset health
        System.Random rng = new System.Random();
        int rand1 = rng.Next(10, 80);

        health = rand1;
        //Debug.Log($"Health set to {health}");
        healthbar.SetHealth(health);

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
