using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class forwardRaycast : MonoBehaviour
{
    // Direction to cast the ray
    //public float x = 0.0f;
    //public float y = 0.0f;
    //public float z = 0.0f;
    public float stepLength = 1f;
   
    

    // Length of the ray
    public float rayLength = 10f;
    // private Vector3 rayDirection = default;
    public Vector3 origin = default;

    // Reference to the target object
    public GameObject target1;
    public Vector3 direction1 = new Vector3(0.0f, 0.0f, 0.0f).normalized;
    
    public GameObject target2;
    public Vector3 direction2 = new Vector3(0.0f, 0.0f, 0.0f).normalized;

    public GameObject target3;
    public Vector3 direction3 = new Vector3(0.0f, 0.0f, 0.0f).normalized;

    public GameObject target4;
    public Vector3 direction4 = new Vector3(0.0f, 0.0f, 0.0f).normalized;

    public GameObject target5;
    public Vector3 direction5 = new Vector3(0.0f, 0.0f, 0.0f).normalized;

    public GameObject target6;
    public Vector3 direction6 = new Vector3(0.0f, 0.0f, 0.0f).normalized;

    public GameObject target7;
    public Vector3 direction7 = new Vector3(0.0f, 0.0f, 0.0f).normalized;

    public GameObject target8;
    public Vector3 direction8 = new Vector3(0.0f, 0.0f, 0.0f).normalized;

    //public float moveSpeed = 5f;

    

    

    public void moveLeg(Vector3 origin ,Vector3 rayDirection, float rayLength, GameObject targetObject){
        //Debug.Log(originPoint+" "+direction+" "+length+" "+target);
        Debug.DrawRay(origin, rayDirection * rayLength, Color.red);
        //Debug.Log($"moving leg {originPoint}  {direction} {length} {target}");
        
        if (Physics.Raycast(origin, rayDirection, out RaycastHit hitInfo, rayLength))
        {
            //Debug.Log($"Hit {hitInfo.collider.name} at distance {hitInfo.distance}");

            if (targetObject != null)// Calculate the distance to the target object, if assigned
            {
                float distanceToHit = Vector3.Distance(targetObject.transform.position, hitInfo.point);
                //Debug.Log($"Distance from {targetObject.name} to hit point: {distanceToHit}");

                if (distanceToHit > stepLength)
                {
                    targetObject.transform.position = hitInfo.point;
                }
            }
        }


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //direction = new Vector3(x, y, z).normalized;
        // Define the origin of the ray (e.g., the object's position)
        origin = transform.position;

        // Calculate the direction in world space (e.g., relative to the object's rotation)
        //rayDirection = transform.TransformDirection(direction1);
        moveLeg(origin,transform.TransformDirection(direction1),rayLength,target1);
        moveLeg(origin,transform.TransformDirection(direction2),rayLength,target2);
        moveLeg(origin,transform.TransformDirection(direction3),rayLength,target3);
        moveLeg(origin,transform.TransformDirection(direction4),rayLength,target4);
        moveLeg(origin,transform.TransformDirection(direction5),rayLength,target5);
        moveLeg(origin,transform.TransformDirection(direction6),rayLength,target6);
        moveLeg(origin,transform.TransformDirection(direction7),rayLength,target7);
        moveLeg(origin,transform.TransformDirection(direction8),rayLength,target8);
        // Perform the raycast
    //     if (Physics.Raycast(origin, rayDirection, out RaycastHit hitInfo, rayLength))
    //     {
    //         // Log hit information
    //         //Debug.Log($"Hit {hitInfo.collider.name} at distance {hitInfo.distance}");

    //         // Calculate the distance to the target object, if assigned
    //         if (targetObject != null)
    //         {
    //             float distanceToHit = Vector3.Distance(targetObject.transform.position, hitInfo.point);
    //             //Debug.Log($"Distance from {targetObject.name} to hit point: {distanceToHit}");

    //             bool stopMoving = true;
    //             Vector3 stepPoint = hitInfo.point; // Store the hit point in stepPoint

    //             // If the distance is greater than stepLength and stopMoving is true
    //             if (stopMoving == true && distanceToHit > stepLength)
    //             {
    //                 // stepPoint = hitInfo.point; // Update stepPoint if conditions are met
    //                 // stopMoving = false;
    //                 targetObject.transform.position = hitInfo.point;
    //             }

    //             // // If stopMoving is false and the target hasn't reached the stepPoint
    //             // if (stopMoving == false && targetObject.transform.position != stepPoint)
    //             // {
    //             //     // Move the targetObject towards the stepPoint
    //             //     targetObject.transform.position = Vector3.MoveTowards(
    //             //         targetObject.transform.position,
    //             //         stepPoint,
    //             //         moveSpeed * Time.deltaTime
    //             //     );
    //             // }
    //             // // If stopMoving is false and the targetObject has reached stepPoint
    //             // else if (stopMoving == false && targetObject.transform.position == stepPoint)
    //             // {
    //             //     stopMoving = true; // Stop the movement
    //             // }
    //         }
    //     }

    //     // Visualize the ray in the Scene view
    //     //Debug.DrawRay(origin, rayDirection * rayLength, Color.red);
    }
    

    // private void OnDrawGizmos()
    // {
        
    //     Gizmos.color = Color.blue;
        
    //     Gizmos.DrawRay(origin,transform.TransformDirection(direction1) * rayLength);
        
    // }
    
}
