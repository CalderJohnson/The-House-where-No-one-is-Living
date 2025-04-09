using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AdaptAgent : Agent
{
    // References to enemy and player so observations can be collected
    private BaseEnemy baseEnemy;
    private GameObject player;
    private Healthbar enemyHealthbar;
    private Healthbar playerHealthbar;
    private PlayerTracker playerTracker;

    // Health tracking used for observations (for one-step updates)
    private float lastPlayerHealth;
    private float lastEnemyHealth;

    // Baseline health recorded when an action is taken
    private float baselinePlayerHealth;
    private float baselineEnemyHealth;

    // Variables to control the fight phase
    private bool isFighting = true;
    private float fightTimer = 0f;
    private readonly float fightDuration = 3.0f; // Fight phase lasts 3 seconds

    public override void Initialize()
    {
        baseEnemy = GetComponent<BaseEnemy>();
        player = baseEnemy.GetTargetRef();
        playerTracker = GetComponent<PlayerTracker>();

        if (baseEnemy != null)
        {
            enemyHealthbar = GetComponent<Healthbar>();
        }

        if (player != null)
        {
            playerHealthbar = player.GetComponent<Healthbar>();
        }

        lastPlayerHealth = GetPlayerHealth();
        lastEnemyHealth = GetEnemyHealth();

        Debug.Log("Training Initialized");
    }

    public override void OnEpisodeBegin()
    {
        // Reset health tracking
        lastPlayerHealth = GetPlayerHealth();
        lastEnemyHealth = GetEnemyHealth();

        // Reset fight phase
        isFighting = false;
        fightTimer = 0f;

        Debug.Log("Episode Begins");
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

        Debug.Log($"Observed state {baseEnemy.GetCurrentState()}");
        Debug.Log($"Observed player aggression: {playerAggression}");
        Debug.Log($"Observed player defensiveness: {playerDefensiveness}");
        Debug.Log($"Observed player hp lost: {playerHealthLost}");
        Debug.Log($"Observed enemy hp lost: {enemyHealthLost}");
        Debug.Log($"Observed retreat threshold {baseEnemy.GetRetreatThreshold()}");
        Debug.Log($"Observed attack range ranged {baseEnemy.GetAttackRangeRanged()}");
        Debug.Log($"Observed block rate {baseEnemy.GetBlockRate()}");
        Debug.Log($"Observed enemy aggression {baseEnemy.GetAggressiveness()}");
        Debug.Log($"Observed distance to player {distanceToPlayer}");

        // Update health tracking for next observation cycle
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

        baseEnemy.SetRetreatThreshold(Mathf.Clamp(baseEnemy.GetRetreatThreshold() + actions.ContinuousActions[0], 0.1f, 0.8f));
        baseEnemy.SetAttackRangeRanged(Mathf.Clamp(baseEnemy.GetAttackRangeRanged() + actions.ContinuousActions[1], 1f, 10f));
        baseEnemy.SetBlockRate(Mathf.Clamp(baseEnemy.GetBlockRate() + actions.ContinuousActions[2], 0f, 1f));
        baseEnemy.SetAggressiveness(Mathf.Clamp(baseEnemy.GetAggressiveness() + actions.ContinuousActions[3], 0f, 1f));

        baselinePlayerHealth = GetPlayerHealth();
        baselineEnemyHealth = GetEnemyHealth();

        // Begin the fight phase (3 seconds of fighting using updated configuration)
        isFighting = true;
        fightTimer = 0f;
    }

    void Update()
    {
        // If in fight phase, accumulate time until 3 seconds have elapsed
        //Debug.Log($"Update Ocurred, is fighting: {isFighting}");
        if (isFighting)
        {
            fightTimer += Time.deltaTime;
            if (fightTimer >= fightDuration)
            {
                // End of the fight phase, calculate reward for the action taken 3 seconds ago
                CalculateReward();
                EndEpisode();
                isFighting = false;
            }
        }
    }

    private void CalculateReward()
    {
        float reward = 0.0f;

        // Reward based on health changes during the fight phase
        float playerHealthLost = baselinePlayerHealth - GetPlayerHealth();
        float enemyHealthLost = baselineEnemyHealth - GetEnemyHealth();

        // Reward damage dealt and penalize damage taken
        reward += playerHealthLost * 0.01f;
        reward -= enemyHealthLost * 0.01f;

        if (enemyHealthbar.DiedRecently())
        {
            reward -= 0.5f;
        }
        else if (playerHealthbar.DiedRecently())
        {
            reward += 0.5f;
        }

        Debug.Log($"Reward calculated {reward}");
        SetReward(reward);
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
    }

    private float GetPlayerDefensiveness()
    {
        return playerTracker != null ? playerTracker.GetNormalizedDefensiveness() : 0.0f;
    }
}
