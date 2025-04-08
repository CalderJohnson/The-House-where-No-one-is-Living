using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactionRange; // Maximum distance to interact
    private ObjectInteract closestObject; // The closest interactable object

    void Update()
    {
        FindClosestInteractable();

        if (closestObject != null && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(closestObject.Interact(transform));
        }
    }

    void FindClosestInteractable()
    {
        ObjectInteract[] interactables = FindObjectsOfType<ObjectInteract>();
        closestObject = null; 
        float minDistance = interactionRange;

        foreach (var obj in interactables)
        {
            obj.UpdateIconVisibility(0f, interactionRange, transform, false);  
        }

        foreach (var obj in interactables)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);

            if (distance <= minDistance && (!obj.useDirectionalInteraction || obj.IsPlayerFacingCorrectDirection(transform)))
            {
                minDistance = distance;
                closestObject = obj; 
            }
        }

        if (closestObject != null)
        {
            closestObject.UpdateIconVisibility(0f, interactionRange, transform, true); 
        }
    }
}
