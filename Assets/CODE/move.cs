using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
    }

    public float speed = 2;

    void Update()
    {
      //   if (Input.GetKey(KeyCode.X))
    //{
       // Moves the object forward at two units per second.
       transform.Translate(Vector3.forward * speed * Time.deltaTime);
    //}
    
    
    }
}
