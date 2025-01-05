using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorrs : MonoBehaviour
{
    public float degree;
    public float RotationSpeed;
    // Start is called before the first frame update
    public void talk(){
        //Debug.Log("michael is cool");

        gameObject.transform.Rotate ( Vector3.up * ( RotationSpeed * Time.deltaTime ) );
        
    }
}
