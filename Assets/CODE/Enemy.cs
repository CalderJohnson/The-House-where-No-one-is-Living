using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;

public class EnemyController : MonoBehaviour
{
    // Fixed attributes
    private float speed = 6f;
    private float maxHealth = 100f;
    private float health;
    private float vision = 20f;
    private float attackRangeClose = 4f;
    private float attackRangeRanged = 10f;
    private Healthbar healthbar;
    private Transform target;
    private Vector3 wanderTarget;

    // Variable attributes (parameters)
    private float retreatThreshold = 20f; // Health threshold to retreat and attempt to regenerate

    // Data collected about player to influence behavior
    // Players distance over last 180 frames (3 seconds) to see if player moving towards/away
    // 

    // FSM controlling enemy behavior
    private StateMachine fsm = new StateMachine();
    private State currentState;

    // Objects the enemy holds
    private Knife knife;
    private Slingshot slingshot;
    private float lastShotTime = -1;

    // Start is called before the first frame update
    void Start()
    {
        // Get player position, initialize healthbar
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        healthbar = GetComponent<Healthbar>();
        if (healthbar != null)
        {
            healthbar.Initialize(maxHealth); // Set initial health
            healthbar.OnDeath += HandleDeath; // Subscribe to the death event
        }

        // Initialize weapons
        slingshot = GetComponentInChildren<Slingshot>();

        // Define FSM
        fsm.AddState("Wander", new State(
            onLogic: (s) => WanderBehavior(),
            onEnter: (s) => SetWanderTarget()
        ));

        fsm.AddState("Chase", new State(
            onLogic: (s) => ChaseBehavior()
        ));

        fsm.AddState("AttackClose", new State(
            onLogic: (s) => AttackCloseBehavior()
        ));

        fsm.AddState("AttackRanged", new State(
            onLogic: (s) => AttackRangedBehavior()
        ));

        fsm.AddState("Defend", new State(
            onLogic: (s) => DefendBehavior()
        ));

        fsm.AddState("Retreat", new State(
            onLogic: (s) => RetreatBehavior()
        ));


        // Transitions
        fsm.AddTwoWayTransition(new Transition("Wander", "Chase", (t) => Vector3.Distance(transform.position, target.position) <= vision));
        fsm.AddTwoWayTransition(new Transition("Chase", "AttackClose", (t) => Vector3.Distance(transform.position, target.position) <= attackRangeClose));
        fsm.AddTwoWayTransition(new Transition("Chase", "AttackRanged", (t) => Vector3.Distance(transform.position, target.position) <= attackRangeRanged));
        fsm.AddTransition(new Transition("AttackRanged", "AttackClose", (t) => Vector3.Distance(transform.position, target.position) <= attackRangeClose));
        fsm.AddTransition(new Transition("AttackRanged", "Retreat", (t) => healthbar.GetHealth() <= retreatThreshold));
        fsm.AddTwoWayTransition(new Transition("Chase", "Retreat", (t) => healthbar.GetHealth() <= retreatThreshold));
        // TODO: implement block fsm.AddTransition(new Transition("Retreat", "Defend", (t) => Vector3.Distance(transform.position, target.position) >= attackRangeRanged));

        fsm.SetStartState("Wander");
        fsm.Init();

        SetWanderTarget();
    }

    void Update()
    {
        health = healthbar.GetHealth();
        fsm.OnLogic();
        RegenerateHealth();
    }

    private void RegenerateHealth()
    {
        if (healthbar.GetLastDamageTime() > 0 && Time.time - healthbar.GetLastDamageTime() >= 5f && health < maxHealth)
        {
            Debug.Log($"Regenerating! {health}");
            healthbar.SetHealth(health + 1);
        }
    }

    private void SetWanderTarget()
    {
        // Pick a random direction to wander
        wanderTarget = transform.position + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
    }

    private void WanderBehavior()
    {
        if (Vector3.Distance(transform.position, wanderTarget) < 7f)
        {
            delay_time(1f);
            SetWanderTarget();
        }
        transform.LookAt(target); // Rotate towards the player
        //Vector3 direction = (wanderTarget - transform.position).normalized;
        //Quaternion targetRotation = Quaternion.LookRotation(direction);
        //Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, wanderTarget, speed * Time.deltaTime);
        Debug.Log("Wandering...");
    }

    private void ChaseBehavior()
    {
        // TODO: replace this with actual pathfinding algorithm
        transform.LookAt(target); // Rotate towards the player
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        Debug.Log("Chasing!");
    }

    private void AttackCloseBehavior()
    {
        // Placeholder for attack logic
        Debug.Log("Attacking close!");
    }

    private void AttackRangedBehavior()
    {
        if (lastShotTime < 0 || (Time.time - lastShotTime) >= 2f)
        {
            Debug.Log("Attacking Ranged!");
            slingshot.ShootProjectile();
            lastShotTime = Time.time;
        }
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
        //directionAwayFromPlayer.y = 0; // Enemy cannot travel up
        directionAwayFromPlayer.y = Random.Range(0,0.2f); 
        transform.position += directionAwayFromPlayer * speed * Time.deltaTime;
        Debug.Log("Retreating!");
    }

    private void HandleDeath()
    {
        Debug.Log(gameObject.name + " died!");
        Destroy(gameObject);
    }

    IEnumerator delay_time(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }
}
