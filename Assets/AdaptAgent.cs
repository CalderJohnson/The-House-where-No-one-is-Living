using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AdaptAgent : Agent
{
    private BaseEnemy baseEnemy;
    private GameObject player;
    private Healthbar enemyHealthbar;
    private Healthbar playerHealthbar;
    private PlayerTracker playerTracker;
    private float lastPlayerHealth;
    private float lastEnemyHealth;

    public override void Initialize()
    {
        baseEnemy = GetComponent<BaseEnemy>();
        player = baseEnemy.GetTargetRef();
        playerTracker = GetComponent<PlayerTracker>();

        if (baseEnemy != null)
        {
            enemyHealthbar = GetComponent<Healthbar>();
            if (enemyHealthbar != null)
            {
                enemyHealthbar.OnDeath += OnEnemyDeath;  // Subscribe to enemy's death event
            }
        }

        if (player != null)
        {
            playerHealthbar = player.GetComponent<Healthbar>();
            if (playerHealthbar != null)
            {
                playerHealthbar.OnDeath += OnPlayerDeath;  // Subscribe to player's death event
            }
        }

        lastPlayerHealth = GetPlayerHealth();
        lastEnemyHealth = GetEnemyHealth();
    }

    public override void OnEpisodeBegin()
    {
        lastPlayerHealth = GetPlayerHealth();
        lastEnemyHealth = GetEnemyHealth();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Encode FSM state as one-hot
        string[] states = { "Wander", "Chase", "AttackClose", "AttackRanged", "Retreat", "Block" };
        foreach (var state in states)
            sensor.AddObservation(baseEnemy.GetCurrentState() == state ? 1.0f : 0.0f);

        // Player and enemy metrics
        float playerAggression = GetPlayerAggression();
        float playerDefensiveness = GetPlayerDefensiveness();
        float playerHealthLost = lastPlayerHealth - GetPlayerHealth();
        float enemyHealthLost = lastEnemyHealth - GetEnemyHealth();
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        sensor.AddObservation(playerAggression);
        sensor.AddObservation(playerDefensiveness);
        sensor.AddObservation(playerHealthLost);
        sensor.AddObservation(enemyHealthLost);
        sensor.AddObservation(baseEnemy.GetRetreatThreshold());
        sensor.AddObservation(baseEnemy.GetAttackRangeRanged());
        sensor.AddObservation(baseEnemy.GetBlockRate());
        sensor.AddObservation(baseEnemy.GetAggressiveness());
        sensor.AddObservation(distanceToPlayer);

        // Update health tracking
        lastPlayerHealth = GetPlayerHealth();
        lastEnemyHealth = GetEnemyHealth();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Debug purposes
        Debug.Log($"Retreat threshold altered by: {actions.ContinuousActions[0]}");
        Debug.Log($"Ranged attack threshold altered by: {actions.ContinuousActions[1]}");
        Debug.Log($"Block rate altered by: {actions.ContinuousActions[2]}");
        Debug.Log($"Aggressiveness altered by: {actions.ContinuousActions[3]}");

        // Update enemy parameters
        baseEnemy.SetRetreatThreshold(Mathf.Clamp(baseEnemy.GetRetreatThreshold() + actions.ContinuousActions[0], 0.1f, 0.8f));
        baseEnemy.SetAttackRangeRanged(Mathf.Clamp(baseEnemy.GetAttackRangeRanged() + actions.ContinuousActions[1], 1f, 10f));
        baseEnemy.SetBlockRate(Mathf.Clamp(baseEnemy.GetBlockRate() + actions.ContinuousActions[2], 0f, 1f));
        baseEnemy.SetAggressiveness(Mathf.Clamp(baseEnemy.GetAggressiveness() + actions.ContinuousActions[3], 0f, 1f));

        // Reward agent for appropriate responses
    }

    // Large reward for killing the player
    public void OnPlayerDeath()
    {
        AddReward(5.0f);
        EndEpisode();
    }

    // Large negative reward for dying
    public void OnEnemyDeath()
    {
        AddReward(-5.0f);
        EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Random.Range(-0.05f, 0.05f);
        continuousActions[1] = Random.Range(-0.2f, 0.2f);
        continuousActions[2] = Random.Range(-0.1f, 0.1f);
        continuousActions[3] = Random.Range(-0.1f, 0.1f);
    }

    private float GetPlayerHealth()
    {
        return playerHealthbar != null ? playerHealthbar.GetHealth() : 100f;
    }

    private float GetEnemyHealth()
    {
        return enemyHealthbar != null ? enemyHealthbar.GetHealth() : 100f;
    }

    private float GetPlayerAggression()
    {
        return playerTracker != null ? playerTracker.GetNormalizedAggressiveness() : 0.0f;
        //return 0.0f;
    }

    private float GetPlayerDefensiveness()
    {
        return playerTracker != null ? playerTracker.GetNormalizedDefensiveness() : 0.0f;
        //return 0.0f;
    }
}