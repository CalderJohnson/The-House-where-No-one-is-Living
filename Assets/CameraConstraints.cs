using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConstraints : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public Transform player;
    public float positiveX = 20;
    public float negativeX = -20;
    public float positiveZ = 20;
    public float negativeZ = -20;
    // Update is called once per frame
    // Set these in the inspector to tune your bounds.
    private Vector3 MinCamPosition = new Vector3(-20,-20,-20);
    private Vector3 MaxCamPosition = new Vector3(20,20,20);


    void Update()
    {

        transform.position = new Vector3(
            Mathf.Clamp(player.transform.position.x, MinCamPosition.x, MaxCamPosition.x),
            Mathf.Clamp(player.transform.position.y +15f, MinCamPosition.y, MaxCamPosition.y), 
            Mathf.Clamp(player.transform.position.z - 17f, MinCamPosition.z, MaxCamPosition.z)
        ); 
    }
    
        
    

}
