using UnityEngine;
using System.Collections.Generic;

public class ObjectSwapTrigger : MonoBehaviour
{
    [Tooltip("Name of the node to check.")]
    public string targetNodeID;

    [Tooltip("Objects to swap out when the node is reached.")]
    public List<GameObject> objectsToSwapIn;

    [Tooltip("Objects to disable when swapping.")]
    public List<GameObject> objectsToSwapOut;

    private bool hasTriggered = false;

    public void TryTriggerSwap()
    {
        if (hasTriggered) return; // Prevent triggering multiple times

        if (ShouldTriggerBasedOnNode(targetNodeID))
        {
            SwapObjects();
            hasTriggered = true;
        }
        else
        {
            Debug.Log($"Swap not triggered: Node {targetNodeID} is not reached.");
        }
    }

    private bool ShouldTriggerBasedOnNode(string nodeID)
    {
        // Check if the player has reached the node in the decision tree
        bool nodeReached = DecisionManager.Instance.HasPassedNode(nodeID);

        // Debugging output
        Debug.Log($"Checking if node {nodeID} has been reached...");
        Debug.Log($"Path taken: {string.Join(", ", DecisionManager.Instance.GetPathTaken())}");
        Debug.Log($"Node reached: {nodeReached}");

        return nodeReached;
    }

    private void SwapObjects()
    {
        if (objectsToSwapIn.Count != objectsToSwapOut.Count)
        {
            Debug.LogWarning("Mismatch in number of objects to swap in and out!");
            return;
        }

        for (int i = 0; i < objectsToSwapOut.Count; i++)
        {
            GameObject oldObject = objectsToSwapOut[i];
            GameObject newObject = objectsToSwapIn[i];

            if (oldObject != null && newObject != null)
            {
                // Move the new object
                newObject.transform.position = oldObject.transform.position;
                newObject.transform.rotation = oldObject.transform.rotation;

                // Enable the new object and disable the old one
                newObject.SetActive(true);
                oldObject.SetActive(false);
            }
        }

        Debug.Log("Object swap completed! Objects swapped in and out.");
    }
}
