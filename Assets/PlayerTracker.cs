using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    public Transform player;
    public Transform enemy;

    private Vector3 lastPlayerPosition;
    private float distanceMovedAway = 0f;
    private float distanceMovedToward = 0f;

    public float timeWindow = 5.0f;
    public float maxEscapeDistance = 10f;  // Max distance player can move away in timeWindow
    public float maxApproachDistance = 10f; // Max distance player can move toward enemy
    private float startTime;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        if (enemy == null)
        {
            enemy = GameObject.FindGameObjectWithTag("Enemy")?.transform;
        }
        lastPlayerPosition = player.position;
        startTime = Time.time;
    }

    void Update()
    {
        if (player == null || enemy == null) return;

        float prevDistance = Vector3.Distance(lastPlayerPosition, enemy.position);
        float currentDistance = Vector3.Distance(player.position, enemy.position);
        float distanceChange = currentDistance - prevDistance;

        if (distanceChange > 0)
        {
            distanceMovedAway += distanceChange; // Moving away
        }
        else if (distanceChange < 0)
        {
            distanceMovedToward += Mathf.Abs(distanceChange); // Moving toward
        }

        lastPlayerPosition = player.position;

        // Reset every timeWindow seconds
        if (Time.time - startTime > timeWindow)
        {
            distanceMovedAway = 0;
            distanceMovedToward = 0;
            startTime = Time.time;
        }
    }

    public float GetNormalizedDefensiveness()
    {
        return Mathf.Clamp01(distanceMovedAway / maxEscapeDistance);
    }

    public float GetNormalizedAggressiveness()
    {
        return Mathf.Clamp01(distanceMovedToward / maxApproachDistance);
    }
}