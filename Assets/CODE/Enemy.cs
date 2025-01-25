using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Attributes
    private float speed;
    private float maxHealth = 100f;
    private float health;
    private Healthbar healthbar;
    private float attackRangeClose = 2f;
    private float attackRangeRanged = 5f;
    private float retreatThreshold = 20f;
    private Transform target;
    private Vector3 wanderTarget;

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
        speed = 3f;
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
        health = healthbar.getHealth();
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

        transform.position = Vector3.MoveTowards(transform.position, wanderTarget, speed * Time.deltaTime);
    }

    private void ChaseBehavior()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    private void AttackCloseBehavior()
    {
        // Placeholder for attack logic
        //Debug.Log("Attacking close!");
    }

    private void AttackRangedBehavior()
    {
        // Placeholder for ranged attack logic
        //Debug.Log("Attacking ranged!");
    }

    private void DefendBehavior()
    {
        // Placeholder for defend logic
        // Debug.Log("Defending!");
    }

    private void RetreatBehavior()
    {
        Vector3 directionAwayFromPlayer = (transform.position - target.position).normalized;
        directionAwayFromPlayer.y = 0; // Enemy cannot travel up
        transform.position += directionAwayFromPlayer * speed * Time.deltaTime;
    }

    private void StateTransitions()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (health <= retreatThreshold)
        {
            currentState = State.Retreat;
        }
        else if (distanceToPlayer <= attackRangeClose)
        {
            currentState = State.AttackClose;
        }
        else if (distanceToPlayer <= attackRangeRanged)
        {
            currentState = State.AttackRanged;
        }
        else if (distanceToPlayer > attackRangeRanged && health < maxHealth)
        {
            currentState = State.Defend;
        }
        else if (distanceToPlayer <= 10f)
        {
            currentState = State.Chase;
        }
        else
        {
            currentState = State.Wander;
        }
    }

    private void HandleDeath()
    {
        Debug.Log(gameObject.name + " died!");
        Destroy(gameObject);
    }
}