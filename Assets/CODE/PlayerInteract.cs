using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactionRange = 3f; // Maximum distance to interact
    public LayerMask interactableLayer;
    private ObjectInteract closestObject;
    
    void Update()
    {
        FindClosestInteractable();

        if (closestObject != null && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(closestObject.Interact());
        }
    }

    void FindClosestInteractable()
    {
        ObjectInteract[] interactables = FindObjectsOfType<ObjectInteract>();
        float minDistance = interactionRange;
        closestObject = null;

        foreach (var obj in interactables)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            obj.UpdateIconVisibility(distance, interactionRange);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestObject = obj;
            }
        }
    }
}