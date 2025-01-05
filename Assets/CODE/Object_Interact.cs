using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Object_Interact : MonoBehaviour
{
    bool isInteractable = false;
    Collider Player;
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;
    public UnityEvent Object_Action;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("enter");
            isInteractable = true;
            Player = other;
        }

    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {


        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("exit");
            isInteractable = false;
            Player = null;
        }
    }

    void Update()
    {
        if (isInteractable && Input.GetKeyDown(KeyCode.Q))
        {
            Object_Action.Invoke();

            isInteractable = false;
            Player = null;

        }
    }
}
