using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable{
    public void Interact();
}
public class interacter : MonoBehaviour
{
    //https://www.youtube.com/watch?v=K06lVKiY-sY
    public Transform InteracterSource;
    public float interactRange;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)){
            Ray r = new Ray(InteracterSource.position, InteracterSource.forward);
            if(Physics.Raycast(r,out RaycastHit hitInfo, interactRange))
                if(hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj)){
                    interactObj.Interact();
                }
        }
        
    }
}
