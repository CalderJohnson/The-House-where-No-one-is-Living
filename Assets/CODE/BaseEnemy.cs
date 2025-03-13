using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

public abstract class BaseEnemy : MonoBehaviour
{
    // Fixed attributes
    protected float speed;
    protected float maxHealth;
    protected float health;
    protected float vision;
    protected float attackRangeClose;
    protected float attackRangeRanged;

    // Params influencing behavior
    protected float retreatThreshold;

    protected Transform target;
    protected Vector3 wanderTarget;
    protected NavMeshAgent pathfindingAgent;
    protected Healthbar healthbar;

    protected float lastShotTime = -1;

    protected StateMachine fsm = new StateMachine();

    // Initialization
    protected virtual void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        healthbar = GetComponent<Healthbar>();

        if (healthbar != null)
        {
            healthbar.Initialize(maxHealth);
            healthbar.OnDeath += HandleDeath;
        }

        pathfindingAgent = GetComponent<NavMeshAgent>();
        pathfindingAgent.speed = speed;

        DefineFSM();
        fsm.SetStartState("Wander");
        fsm.Init();
    }

    protected virtual void Update()
    {
        health = healthbar.GetHealth();
        fsm.OnLogic();
        RegenerateHealth();
    }

    // FSM Setup
    protected virtual void DefineFSM()
    {
        fsm.AddState("Wander", new State(onLogic: (s) => WanderBehavior(), onEnter: (s) => SetWanderTarget()));
        fsm.AddState("Chase", new State(onLogic: (s) => ChaseBehavior()));
        fsm.AddState("AttackClose", new State(onLogic: (s) => AttackCloseBehavior()));
        fsm.AddState("AttackRanged", new State(onLogic: (s) => AttackRangedBehavior()));
        fsm.AddState("Retreat", new State(onLogic: (s) => RetreatBehavior()));

        fsm.AddTwoWayTransition(new Transition("Wander", "Chase", (t) => Vector3.Distance(transform.position, target.position) <= vision));
        fsm.AddTwoWayTransition(new Transition("Chase", "AttackClose", (t) => Vector3.Distance(transform.position, target.position) <= attackRangeClose));
        fsm.AddTwoWayTransition(new Transition("Chase", "AttackRanged", (t) => Vector3.Distance(transform.position, target.position) <= attackRangeRanged));
        fsm.AddTwoWayTransition(new Transition("Chase", "Retreat", (t) => healthbar.GetHealth() <= retreatThreshold));
    }

    // Enemy Behaviors (Can be overridden in subclasses)
    protected virtual void WanderBehavior()
    {
        if (Vector3.Distance(transform.position, wanderTarget) < 2f)
        {
            StartCoroutine(DelayThenWander(1f));
        }
    }

    protected virtual void ChaseBehavior()
    {
        if (target != null)
        {
            pathfindingAgent.SetDestination(target.position);
        }
    }

    protected virtual void AttackCloseBehavior()
    {
        Debug.Log("Attacking close!");
    }

    protected virtual void AttackRangedBehavior()
    {
        if (lastShotTime < 0 || (Time.time - lastShotTime) >= 2f)
        {
            Debug.Log("Attacking Ranged!");
            Debug.Log(target.position);
            lastShotTime = Time.time;
        }
    }

    protected virtual void RetreatBehavior()
    {
        if (target != null)
        {
            Vector3 directionAwayFromPlayer = (transform.position - target.position).normalized;
            Vector3 retreatPosition = transform.position + directionAwayFromPlayer * 10f;
            pathfindingAgent.SetDestination(retreatPosition);
        }
    }

    // Utility Functions
    protected void SetWanderTarget()
    {
        wanderTarget = transform.position + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
        pathfindingAgent.SetDestination(wanderTarget);
    }

    protected void RegenerateHealth()
    {
        if (healthbar.GetLastDamageTime() > 0 && Time.time - healthbar.GetLastDamageTime() >= 5f && health < maxHealth)
        {
            healthbar.SetHealth(health + 1);
        }
    }

    protected void HandleDeath()
    {
        Debug.Log(gameObject.name + " died!");
        Destroy(gameObject);
    }

    IEnumerator DelayThenWander(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SetWanderTarget();
    }
}
