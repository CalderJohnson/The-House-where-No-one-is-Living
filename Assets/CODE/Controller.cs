using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Controller : MonoBehaviour
{
    // Moving
    //public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool canMove = true;

    // // Attacking
    // public GameObject knifePrefab;      // Knife prefab
    // public Transform attackPoint;       // Position where the knife attack spawns
    // public float attackRange = 1.5f;    // Range of the knife attack
    // public float attackCooldown = 1f;   // Cooldown between attacks

    // private float nextAttackTime = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.R) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;

        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 6f;
            runSpeed = 12f;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            //playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        // // Knife attack
        // if (Input.GetKeyDown(KeyCode.E) && Time.time >= nextAttackTime)
        // {
        //     PerformKnifeAttack();
        //     nextAttackTime = Time.time + attackCooldown; // Set next attack time
        // }
    }

    // void PerformKnifeAttack()
    // {
    //     // Short range damage
    //     Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange);
    //     foreach (Collider enemy in hitEnemies)
    //     {
    //         if (enemy.CompareTag("Enemy"))
    //         {
    //             Debug.Log("Hit enemy: " + enemy.name);
    //             enemy.GetComponent<EnemyController>().TakeDamage(20);
    //         }
    //     }

    //     // Visual effect
    //     GameObject knife = Instantiate(knifePrefab, attackPoint.position, Quaternion.identity);
    //     Destroy(knife, 0.5f); // Destroy the visual effect after 0.5 seconds
    // }
}