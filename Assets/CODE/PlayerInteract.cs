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
            StartCoroutine(closestObject.Interact(transform));
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
            obj.UpdateIconVisibility(distance, interactionRange, transform);

            // Get direction from player to object
            Vector3 directionToObject = (obj.transform.position - transform.position).normalized;

            // Use the object's interaction direction for the facing check
            Vector3 requiredDirection = obj.transform.TransformDirection(obj.interactionDirection).normalized;
            float dotProduct = Vector3.Dot(requiredDirection, directionToObject);
            bool isFacingCorrectly = dotProduct > 0.5f; // Adjusted to support any interaction direction

            if (distance < minDistance && isFacingCorrectly)
            {
                minDistance = distance;
                closestObject = obj;
            }
        }
    }
}