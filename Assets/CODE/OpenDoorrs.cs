using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorrs : MonoBehaviour
{
    public float degree;
    public float RotationSpeed;
    // Start is called before the first frame update
    public void talk(){
        Debug.Log("michael is cool");

        int counter = 0;
        while(counter <= RotationSpeed){
            transform.Rotate ( new Vector3(0,RotationSpeed, 0) *  Time.deltaTime );
            counter = counter + 1;
        }
    }
    void Update(){
        
    }
}
