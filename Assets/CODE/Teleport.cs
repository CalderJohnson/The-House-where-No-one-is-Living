using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public GameObject Player;
    public float x;
    public float y;
    public float z;
    // Start is called before the first frame update
    public void TeleportPlayer(){
        Player.gameObject.SetActive(false);
        Player.gameObject.transform.position = new Vector3(x, y, z);
        Player.gameObject.SetActive(true);
    }
}
