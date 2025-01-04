using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Interact : MonoBehaviour
{
    bool isInteractable = false;
    Collider Player;
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("enter");
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
            Debug.Log("exit");
            isInteractable = false;
            Player = null;
        }
    }

    void Update()
    {
        if (isInteractable && Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Teleport to: " + x + y + z);

            Player.gameObject.SetActive(false);
            Player.gameObject.transform.position = new Vector3(x, y, z);
            Player.gameObject.SetActive(true);

            isInteractable = false;
            Player = null;

        }
    }
}
