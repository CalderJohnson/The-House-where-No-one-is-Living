using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class TrainableEnemy : BaseEnemy
{
    public bool training; // Training mode toggle
    public GameObject floor = null; // For training, set floor colour to indicate win/loss
    private int deathCount;

    protected override void Start()
    {
        acceleration = 8f;
        vision = 25f;
        attackRangeClose = 3f;

        deathCount = 0;

        base.Start();
    }

    protected void Reset()
    {
        // Reset health
        System.Random rng = new System.Random();
        int rand1 = rng.Next(10, (int)maxHealth);

        health = rand1;
        //Debug.Log($"Health set to {health}");
        healthbar.SetHealth(health);

        // Reset position to a random NavMesh point within bounds
        Vector3 spawnPoint;
        if (GetRandomPointOnNavMesh(transform.position, new Vector3(12, 0, 12), out spawnPoint))
        {
            GetComponent<NavMeshAgent>().Warp(spawnPoint);
        }
        else
        {
            // Debug.LogWarning("Failed to find a valid spawn point on the NavMesh.");
        }

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

    private bool GetRandomPointOnNavMesh(Vector3 center, Vector3 range, out Vector3 result, int maxAttempts = 50)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomPoint = new Vector3(
                Random.Range(center.x - range.x, center.x + range.x),
                center.y,
                Random.Range(center.z - range.z, center.z + range.z)
            );

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
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
