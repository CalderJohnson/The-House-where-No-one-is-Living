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
        var speed = MovementSpeed * Time.deltaTime;
        // transform.Translate(targetVector * (MovementSpeed * Time.deltaTime)); Demonstrate why this doesn't work
        //transform.Translate(targetVector * (MovementSpeed * Time.deltaTime), Camera.gameObject.transform);
        if(Input.GetKey(KeyCode.LeftShift) && ( Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.W))){
            speed = SprintSpeed * Time.deltaTime;
            RotateTowardMouse = false;
            playerAnim.SetBool("running",true);
            playerAnim.SetBool("Walking",false);
        }
        // else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.W) ){
        else if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0){
            speed = MovementSpeed * Time.deltaTime;
            RotateTowardMouse = true;
            playerAnim.SetBool("running",false);
            playerAnim.SetBool("Walking",true);
        }
        else{
            speed = MovementSpeed * Time.deltaTime;
            RotateTowardMouse = true;
            playerAnim.SetBool("running",false);
            playerAnim.SetBool("Walking",false);
        }
        targetVector = Quaternion.Euler(0, Camera.gameObject.transform.rotation.eulerAngles.y, 0) * targetVector;
        var targetPosition = transform.position + targetVector * speed;
        transform.position = targetPosition;
        return targetVector;
    }

    private void RotateTowardMovementVector(Vector3 movementDirection)
    {
        if(movementDirection.magnitude == 0) { return; }
        var rotation = Quaternion.LookRotation(movementDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, RotationSpeed);
    }
}