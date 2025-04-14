using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class lockRotation : MonoBehaviour
{
    private Quaternion initialRotation;

    void Start()
    {
        // Store the initial rotation of the object
        initialRotation = transform.rotation;
    }

    void LateUpdate()
    {
        // Lock the rotation to the initial value every frame
        transform.rotation = initialRotation;
    }
}