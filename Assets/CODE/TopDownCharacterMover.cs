using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//https://www.youtube.com/watch?v=-0GFb9l3NHM
[RequireComponent(typeof(InputHandler))]
public class TopDownCharacterMover : MonoBehaviour
{
    private InputHandler _input;

    [SerializeField]
    private bool RotateTowardMouse;

    [SerializeField]
    private float MovementSpeed;
    [SerializeField]
    private float SprintSpeed;
    [SerializeField]
    private float RotationSpeed;

    [SerializeField]
    private Camera Camera;

    Animator playerAnim;

    [SerializeField] private float AccelerationTime = 0.5f; // Time to reach full sprint speed
    private float currentSpeed = 0f; // Tracks current movement speed
    private float speedVelocity = 0f; // Used for Mathf.SmoothDamp

    private void Awake()
    {
        playerAnim = GetComponent<Animator>();
        _input = GetComponent<InputHandler>();
    }

    public UnityEvent Right_Hand;
    public UnityEvent Left_Hand;
    // Update is called once per frame
    void FixedUpdate()
    {
        
        var targetVector = new Vector3(_input.InputVector.x, 0, _input.InputVector.y);
        var movementVector = MoveTowardTarget(targetVector);

        if (!RotateTowardMouse)
        {
            RotateTowardMovementVector(movementVector);
        }
        if (RotateTowardMouse)
        {
            RotateFromMouseVector();
        }

        if(Input.GetKeyDown(KeyCode.Mouse0)){//left click
            playerAnim.SetBool("Attack",true);
            Right_Hand.Invoke(); //attack with right hand
            Invoke("ResetAttack", 1f); 
            
        }
        if(Input.GetKeyDown(KeyCode.Mouse1)){ //right click
            Left_Hand.Invoke(); //attack with left hand
        }
        

    }

    void ResetAttack()
    {
    playerAnim.SetBool("Attack", false);
    }

    private void RotateFromMouseVector()
    {
        Ray ray = Camera.ScreenPointToRay(_input.MousePosition);
        //Debug.Log(_input.MousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance: 300f))
        {
            // var target = hitInfo.point;
            // target.y = transform.position.y;
            // transform.LookAt(target);

            var target = hitInfo.point;
            target.y = transform.position.y;
            Vector3 direction = target - transform.position;
            float rotationSpeed = 5f; // Adjust this value for speed of rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private Vector3 MoveTowardTarget(Vector3 targetVector)
    {
        // var speed = MovementSpeed * Time.deltaTime;
        // // transform.Translate(targetVector * (MovementSpeed * Time.deltaTime)); Demonstrate why this doesn't work
        // //transform.Translate(targetVector * (MovementSpeed * Time.deltaTime), Camera.gameObject.transform);


        
        // if(Input.GetKey(KeyCode.LeftShift) && ( Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.W))){
        //     speed = SprintSpeed * Time.deltaTime;
        //     RotateTowardMouse = false;
        //     playerAnim.SetBool("running",true);
        //     playerAnim.SetBool("Walking",false);
        // }
        // // else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.W) ){
        // else if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0){
        //     speed = MovementSpeed * Time.deltaTime;
        //     RotateTowardMouse = true;
        //     playerAnim.SetBool("running",false);
        //     playerAnim.SetBool("Walking",true);
        // }
        // else{
        //     speed = MovementSpeed * Time.deltaTime;
        //     RotateTowardMouse = true;
        //     playerAnim.SetBool("running",false);
        //     playerAnim.SetBool("Walking",false);
        // }
        // targetVector = Quaternion.Euler(0, Camera.gameObject.transform.rotation.eulerAngles.y, 0) * targetVector;
        // var targetPosition = transform.position + targetVector * speed;
        // transform.position = targetPosition;
        // return targetVector;
    bool isMoving = targetVector.magnitude > 0;
    float backwardsMagnitude = 0.5f; 
    float sidesMagnitude = 0.5f; 

    // Convert targetVector to world space using camera rotation
    targetVector = Quaternion.Euler(0, Camera.gameObject.transform.rotation.eulerAngles.y, 0) * targetVector;

    // Determine movement direction
    Vector3 movementDirection = targetVector.normalized;

    // Calculate the dot product to determine if moving forward or backward
    float forwardDot = Vector3.Dot(transform.forward, movementDirection);
    bool isMovingForward = forwardDot > 0.5f;
    bool isMovingBackward = forwardDot < -0.5f;

    // Calculate the cross product to determine if moving left or right
    Vector3 crossProduct = Vector3.Cross(transform.forward, movementDirection);
    bool isMovingRight = crossProduct.y > 0.5f;
    bool isMovingLeft = crossProduct.y < -0.5f;

    // Log the movement direction
    if (isMovingForward)
    {
        backwardsMagnitude = 1.0f; 
        Debug.Log("Moving Forward");
    }
    else if (isMovingBackward)
    {
        backwardsMagnitude = 0.0f; 
        Debug.Log("Moving Backward");
    }
    else if (isMovingLeft)
    {
        sidesMagnitude = 1.0f; 
        Debug.Log("Moving Left");
    }
    else if (isMovingRight)
    {
        sidesMagnitude = 0.0f; 
        Debug.Log("Moving Right");
    }

    // Smoothly accelerate/decelerate sprint speed
    float targetSpeed = MovementSpeed;
    if (Input.GetKey(KeyCode.LeftShift) && isMovingForward)
    {
        targetSpeed = SprintSpeed;
        RotateTowardMouse = false;
    }
    else{
        RotateTowardMouse = true;
    }

    // Use Mathf.SmoothDamp to gradually adjust speed
    currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, AccelerationTime);

    // Apply movement
    float finalSpeed = currentSpeed * Time.deltaTime;
    transform.position += targetVector * finalSpeed;

    // Calculate velocity magnitude for animation blending
    float velocityMagnitude = targetVector.magnitude * (currentSpeed / SprintSpeed); // Normalize to 0-1 range
    playerAnim.SetFloat("BlendRun", velocityMagnitude);
    playerAnim.SetFloat("FWDBWD", backwardsMagnitude);
    playerAnim.SetFloat("LR", sidesMagnitude);
    //Debug.Log(velocityMagnitude);

    return targetVector;
    }

    private void RotateTowardMovementVector(Vector3 movementDirection)
    {
        if(movementDirection.magnitude == 0) { return; }
        var rotation = Quaternion.LookRotation(movementDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, RotationSpeed);
    }


}