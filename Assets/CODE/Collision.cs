using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    public Material newMaterial; // Assign this in the Inspector
    private Material originalMaterial; // Store the original material
    bool isInteractable = false;
    Collider enemy;
    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Object"){
            Debug.Log("enter");
            isInteractable = true;
            enemy = other;
        }

    }

    void OnTriggerStay(Collider other){
        if(other.gameObject.tag == "Object"){
           
            
        }
        
    }

    void OnTriggerExit(Collider other){
        if(other.gameObject.tag == "Object"){
            Debug.Log("exit");
            isInteractable = false;
            enemy = null;
        }
    }

    void Update(){
        if(isInteractable && Input.GetKeyDown(KeyCode.E)){
            Debug.Log("interact");
            enemy.gameObject.SetActive(!enemy.gameObject.activeSelf);
            Destroy(enemy.gameObject);
            
        }
    }
}
