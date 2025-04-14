using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    
    public void SelfDelete() {
        Destroy(gameObject); // Destroys the GameObject this script is attached to
    }

}
