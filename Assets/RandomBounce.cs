using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RandomBounce : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float randomness = 0.5f; // How much randomness to add (0-1)
    public float minSpeed = 4f;
    private Rigidbody rb;
    private Vector3 currentDirection;
    Vector3 awayDirection;
    
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentDirection = new Vector3(1, 0, 1).normalized;
        rb.velocity =  currentDirection * moveSpeed; //Random.onUnitSphere * moveSpeed;
    }

    void FixedUpdate(){
        if(rb.velocity.magnitude < minSpeed)
        {
            rb.velocity = rb.velocity.normalized * minSpeed;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("RightWall"))
        {
            Debug.Log("rightwall");
            // Simple random direction away from collision point
            //Vector3 awayDirection = (transform.position - collision.transform.position).normalized;
            if(currentDirection == new Vector3(1, 0, 1).normalized ) {//upRight
                awayDirection= new Vector3(-1, 0, 1).normalized; // upLeft
                currentDirection = awayDirection;
            }
            else{//down right
                awayDirection = new Vector3(-1, 0, -1).normalized;//down left
                currentDirection = awayDirection;
            }
            
            // Vector3 randomDirection = (awayDirection + Random.insideUnitSphere).normalized;
            rb.velocity = awayDirection * moveSpeed;
        
        }
        if (collision.gameObject.CompareTag("LeftWall"))
        {
            if(currentDirection == new Vector3(-1, 0, 1).normalized ){ //upLeft
                awayDirection = new Vector3(1, 0, 1).normalized; // upRight
                currentDirection = awayDirection;
            }
            else{//down left
                awayDirection = new Vector3(1, 0, -1).normalized;//down Right
                currentDirection = awayDirection;
            }
            // Simple random direction away from collision point
            // Vector3 awayDirection = (transform.position - collision.transform.position).normalized;
            // Vector3 randomDirection = (awayDirection + Random.insideUnitSphere).normalized;
            //Vector3 awayDirection = new Vector3(1, 0, 1).normalized;   // upRight
            rb.velocity = awayDirection * moveSpeed;
        
        }
        if (collision.gameObject.CompareTag("BackWall"))
        {
            if(currentDirection == new Vector3(-1, 0, 1).normalized ){ //upLeft
                awayDirection = new Vector3(-1, 0, -1).normalized; // Down Left
                currentDirection = awayDirection;
            }
            else{//Up right
                awayDirection = new Vector3(1, 0, -1).normalized;//down Right
                currentDirection = awayDirection;
            }
            // Simple random direction away from collision point
            
            rb.velocity = awayDirection * moveSpeed;
        
        }
        if (collision.gameObject.CompareTag("FrontWall"))
        {
            if(currentDirection == new Vector3(-1, 0, -1).normalized ){ //down Left
                awayDirection = new Vector3(-1, 0, 1).normalized; // up Left
                currentDirection = awayDirection;
            }
            else{//down right
                awayDirection = new Vector3(1, 0, 1).normalized;//Up Right
                currentDirection = awayDirection;
            }
            rb.velocity = awayDirection * moveSpeed;
        
        }
    }  
}