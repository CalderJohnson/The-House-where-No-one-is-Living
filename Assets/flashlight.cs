using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flashlight : MonoBehaviour
{
    public void toggle(){
        if(gameObject.transform.GetChild(0).gameObject.activeSelf == true){
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        else{
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
        
    }
}
