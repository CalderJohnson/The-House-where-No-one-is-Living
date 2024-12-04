using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orthoscale : MonoBehaviour
{
    private Vector2 screenResolution;
    public Camera renderCamera;
    // Start is called before the first frame update
    void Start()
    {
        screenResolution = new Vector2(Screen.width, Screen.height);
        MatchPlane();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(screenResolution.x != Screen.width || screenResolution.y != Screen.height){
            MatchPlane();
            screenResolution.x = Screen.width;
            screenResolution.y = Screen.height;

        }
    }
    private void MatchPlane(){
        float planeheight = 2.0f * renderCamera.orthographicSize / 10.0f;
        float planewidth = planeheight* renderCamera.aspect;
        gameObject.transform.localScale = new Vector3(planewidth,1,planeheight); 
    }
}
