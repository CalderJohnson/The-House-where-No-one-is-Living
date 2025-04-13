using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachArm : MonoBehaviour
{
    public GameObject arm; // Assign the upper arm GameObject in the Inspector
    public Rigidbody armRigidbody;
    public CharacterJoint armJoint;

    void Start()
    {
        // Get the Rigidbody and Joint if not assigned
        if (!armRigidbody) armRigidbody = arm.GetComponent<Rigidbody>();
        if (!armJoint) armJoint = arm.GetComponent<CharacterJoint>();
    }

    public void Detach()
    {
        if (armJoint)
        {
            Destroy(armJoint); // Remove the joint
        }

        if (armRigidbody)
        {
            armRigidbody.isKinematic = false; // Ensure physics is active
            armRigidbody.AddForce(Vector3.up * 500f + transform.forward * 200f); // Apply force
        }
    }
}