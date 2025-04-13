using UnityEngine;

public class OutOfBoundsTeleporter : MonoBehaviour
{
    [Tooltip("Where the player will be teleported if they fall out of bounds.")]
    public Transform[] safePositions;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player fell out of bounds!");

            Transform closest = FindClosestSafePosition(other.transform.position);

            if (closest != null)
            {
                other.transform.position = closest.position;
                Debug.Log("Player teleported back to safety!");
            }
        }
    }

    private Transform FindClosestSafePosition(Vector3 playerPosition)
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform safeSpot in safePositions)
        {
            float distance = Vector3.Distance(playerPosition, safeSpot.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = safeSpot;
            }
        }

        return closest;
    }
}
