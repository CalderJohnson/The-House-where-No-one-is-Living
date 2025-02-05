using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Fixed attributes
    private float speed = 3f;
    private float maxHealth = 100f;
    private float health;
    private float vision = 20f;
    private float attackRangeClose = 2f;
    private float attackRangeRanged = 20f;
    private Healthbar healthbar;
    private Transform target;
    private Vector3 wanderTarget;

    // Variable attributes (parameters)
    private float retreatThreshold = 20f; // Health threshold to retreat and attempt to regenerate

    // Data collected about player to influence behavior
    // Players distance over last 180 frames (3 seconds) to see if player moving towards/away
    // 


    // FSM controlling enemy behavior
    private enum State
    {
        Wander,
        Chase,
        AttackClose,
        AttackRanged,
        Defend,
        Retreat
    }

    private State currentState;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        health = maxHealth;
        currentState = State.Wander;
        SetWanderTarget();
        healthbar = GetComponent<Healthbar>();
        if (healthbar != null)
        {
            healthbar.Initialize(100); // Set initial health
            healthbar.OnDeath += HandleDeath; // Subscribe to the death event
        }
    }

    void Update()
    {
        health = healthbar.GetHealth();
        switch (currentState)
        {
            case State.Wander:
                WanderBehavior();
                break;
            case State.Chase:
                ChaseBehavior();
                break;
            case State.AttackClose:
                AttackCloseBehavior();
                break;
            case State.AttackRanged:
                AttackRangedBehavior();
                break;
            case State.Defend:
                DefendBehavior();
                break;
            case State.Retreat:
                RetreatBehavior();
                break;
        }

        if (healthbar.GetLastDamageTime() > 0 && Time.time - healthbar.GetLastDamageTime() >= 5f) // Regenerate
        {
            Debug.Log($"Regenerating! {health}");
            healthbar.SetHealth(health + 1);
        }
        StateTransitions();
    }


    private void SetWanderTarget()
    {
        // Pick a random direction to wander
        wanderTarget = transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
    }

    private void WanderBehavior()
    {
        if (Vector3.Distance(transform.position, wanderTarget) < 0.5f)
        {
            SetWanderTarget();
        }
        transform.LookAt(wanderTarget); // Rotate towards the player
        transform.position = Vector3.MoveTowards(transform.position, wanderTarget, speed * Time.deltaTime);
    }

    private void ChaseBehavior()
    {
        // TODO: replace this with actual pathfinding algorithm
        transform.LookAt(target); // Rotate towards the player
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    private void AttackCloseBehavior()
    {
        // Placeholder for attack logic
        Debug.Log("Attacking close!");
    }

    private void AttackRangedBehavior()
    {
        // Placeholder for ranged attack logic
        Debug.Log("Attacking ranged!");
    }

    private void DefendBehavior()
    {
        // Placeholder for defend logic
        Debug.Log("Defending!");
    }

    private void RetreatBehavior()
    {
        transform.LookAt(target);
        Vector3 directionAwayFromPlayer = (transform.position - target.position).normalized;
        directionAwayFromPlayer.y = 0; // Enemy cannot travel up
        transform.position += directionAwayFromPlayer * speed * Time.deltaTime;
    }

    private void StateTransitions()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        switch (currentState)
        {
            case State.Wander:
                if (distanceToPlayer <= vision) // If enemy can see player, chase!
                {
                    currentState = State.Chase;
                }
                break;
            case State.Chase:
                if (distanceToPlayer >= vision) // If player gets too far away, wander
                {
                    currentState = State.Wander;
                }
                break;
            case State.AttackClose:
                AttackCloseBehavior();
                break;
            case State.AttackRanged:
                AttackRangedBehavior();
                break;
            case State.Defend:
                DefendBehavior();
                break;
            case State.Retreat:
                RetreatBehavior();
                break;
        }
    }

    private void HandleDeath()
    {
        Debug.Log(gameObject.name + " died!");
        Destroy(gameObject);
    }
}