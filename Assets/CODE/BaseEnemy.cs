using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

public abstract class BaseEnemy : MonoBehaviour
{
    // Fixed attributes
    protected float speed;
    protected float acceleration;
    protected float maxHealth;
    protected float health;
    protected float vision;
    protected float attackRangeClose;
    protected float blockStartTime;

    // Params for agent to modify
    protected float retreatThreshold;
    protected float attackRangeRanged;
    protected float blockRate;
    protected float aggressiveness; // Aggressiveness trades speed and attack for health and defense

    // References held
    public GameObject targetRef;
    public GameObject floor = null; // For training
    protected Transform target;
    protected Vector3 wanderTarget;
    protected NavMeshAgent pathfindingAgent;
    public Healthbar healthbar;
    public event System.Action OnDeath; // Event for death notification

    protected float lastShotTime = -1;

    protected StateMachine fsm = new StateMachine();

    // Initialization
    protected virtual void Start()
    {
        target = targetRef.transform;
        healthbar = GetComponent<Healthbar>();

        if (healthbar != null)
        {
            healthbar.Initialize(maxHealth);
            healthbar.OnDeath += HandleDeath;
        }

        pathfindingAgent = GetComponent<NavMeshAgent>();
        pathfindingAgent.speed = speed;
        pathfindingAgent.acceleration = acceleration;
        pathfindingAgent.stoppingDistance = 1f;

        DefineFSM();
        fsm.SetStartState("Wander");
        fsm.Init();
    }

    protected virtual void Update()
    {
        if (targetRef != null)
        {
            health = healthbar.GetHealth();
            fsm.OnLogic();
            RegenerateHealth();
        }
    }

    // Interface for ML agent to update FSM
    public float GetRetreatThreshold() { return retreatThreshold; }
    public float GetAttackRangeRanged() { return attackRangeRanged; }
    public float GetBlockRate() { return blockRate; }
    public float GetAggressiveness() { return aggressiveness; }
    public string GetCurrentState() { return fsm.ActiveState.name; }
    public GameObject GetTargetRef() { return targetRef; }
    public void SetRetreatThreshold(float newThreshold) { retreatThreshold = newThreshold; }
    public void SetAttackRangeRanged(float newThreshold) { attackRangeRanged = newThreshold; }
    public void SetBlockRate(float newThreshold) { blockRate = newThreshold; }
    public void SetAggressiveness(float newValue) { aggressiveness = newValue; }

    // FSM Setup
    protected virtual void DefineFSM()
    {
        // Define the nodes of the FSM
        fsm.AddState("Wander", new State(onLogic: (s) => WanderBehavior(), onEnter: (s) => SetWanderTarget()));
        fsm.AddState("Chase", new State(onLogic: (s) => ChaseBehavior()));
        fsm.AddState("AttackClose", new State(onLogic: (s) => AttackCloseBehavior()));
        fsm.AddState("AttackRanged", new State(onLogic: (s) => AttackRangedBehavior()));
        fsm.AddState("Retreat", new State(onLogic: (s) => RetreatBehavior()));
        fsm.AddState("Block", new State(
            onLogic: (s) => BlockBehavior(),
            onEnter: (s) => blockStartTime = Time.time
        ));

        // Define the edges of the FSM
        fsm.AddTwoWayTransition(new Transition("Wander", "Chase", (t) => Vector3.Distance(transform.position, target.position) <= vision));
        fsm.AddTwoWayTransition(new Transition("Chase", "AttackClose", (t) => Vector3.Distance(transform.position, target.position) <= attackRangeClose));
        fsm.AddTwoWayTransition(new Transition("Chase", "AttackRanged", (t) => Vector3.Distance(transform.position, target.position) <= attackRangeRanged));
        fsm.AddTwoWayTransition(new Transition("Chase", "Retreat", (t) => healthbar.GetHealth() <= retreatThreshold));
        fsm.AddTransition(new Transition("AttackRanged", "AttackClose", (t) => Vector3.Distance(transform.position, target.position) <= attackRangeClose));
        fsm.AddTransition(new Transition("AttackRanged", "Retreat", (t) => healthbar.GetHealth() <= retreatThreshold));
        fsm.AddTransition(new Transition("Chase", "Wander", (t) => Vector3.Distance(transform.position, target.position) >= vision));
        fsm.AddTransition(new Transition("AttackClose", "Retreat", (t) => healthbar.GetHealth() <= retreatThreshold));

        // Random chance to block
        fsm.AddTransition(new Transition("Chase", "Block", (t) => Random.value < blockRate));
        fsm.AddTransition(new Transition("Block", "Chase", (t) => 1 < Time.time - blockStartTime));
    }

    // Enemy Behaviors (Can be overridden in subclasses)
    protected virtual void WanderBehavior()
    {
        pathfindingAgent.Resume();
        if (Vector3.Distance(transform.position, wanderTarget) < 2f)
        {
            StartCoroutine(DelayThenWander(1f));
        }
        Debug.Log("Wandering...");
    }

    protected virtual void ChaseBehavior()
    {
        pathfindingAgent.Resume();
        if (target != null)
        {
            pathfindingAgent.SetDestination(target.position);
            Debug.Log($"Chasing Position: x={target.position.x}, y={target.position.y}, z={target.position.z}");
        }
    }

    protected virtual void AttackCloseBehavior()
    {
        Debug.Log("Attacking close!");
        pathfindingAgent.Stop();
        transform.LookAt(target);
    }

    protected virtual void BlockBehavior()
    {
        Debug.Log("Blocking!");
        pathfindingAgent.Stop();
        transform.LookAt(target);
    }

    protected virtual void AttackRangedBehavior()
    {
        Debug.Log("Attacking Ranged!");
        pathfindingAgent.Stop();
        transform.LookAt(target);
    }

    protected virtual void RetreatBehavior()
    {
        pathfindingAgent.Resume();
        if (target != null)
        {
            Vector3 directionAwayFromPlayer = (transform.position - target.position).normalized;
            Vector3 retreatPosition = transform.position + directionAwayFromPlayer * 10f;
            pathfindingAgent.SetDestination(retreatPosition);
        }
    }

    // Utility Functions
    protected virtual void SetWanderTarget()
    {
        wanderTarget = transform.position + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
        pathfindingAgent.SetDestination(wanderTarget);
    }

    protected virtual void RegenerateHealth()
    {
        if (healthbar.GetLastDamageTime() > 0 && Time.time - healthbar.GetLastDamageTime() >= 5f && health < maxHealth)
        {
            healthbar.SetHealth(health + 1);
        }
    }

    protected virtual void HandleDeath()
    {
        //Debug.Log(gameObject.name + " died!");
        Destroy(gameObject);
    }

    IEnumerator DelayThenWander(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SetWanderTarget();
    }
}
