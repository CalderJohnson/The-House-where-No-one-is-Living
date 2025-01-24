using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConstraints : MonoBehaviour
{
    
    public Transform player;
    public float positiveX = 20f;
    public float negativeX = -20f;
    public float positiveZ = 20f;
    public float negativeZ = -20f;

    public float offsetX = 0f;
    public float offsetY = 15f;
    public float offsetz = -17f;
    // Update is called once per frame
    // Set these in the inspector to tune your bounds.
    private Vector3 MinCamPosition; //= new Vector3(negativeX,-20,negativeZ);
    private Vector3 MaxCamPosition; //= new Vector3(positiveX,20,positiveX);

    // Start is called before the first frame update
    void Start()
    {
        MinCamPosition = new Vector3(negativeX,-20,negativeZ);
        MaxCamPosition = new Vector3(positiveX,20,positiveX);
    }

    void Update()
    {
        

        transform.position = new Vector3(
            Mathf.Clamp(player.transform.position.x, MinCamPosition.x, MaxCamPosition.x),
            Mathf.Clamp(player.transform.position.y +15f, MinCamPosition.y, MaxCamPosition.y), 
            Mathf.Clamp(player.transform.position.z - 17f, MinCamPosition.z, MaxCamPosition.z)
        ); 
    }
    
        
    

}
