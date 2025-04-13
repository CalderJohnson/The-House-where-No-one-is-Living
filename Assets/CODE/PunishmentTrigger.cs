using UnityEngine;
using System.Collections.Generic;

public class PunishmentTrigger : MonoBehaviour
{
    [Tooltip("Name of the node to check.")]
    public string targetNodeID;

    [Tooltip("Enemies to reveal when punishment is triggered.")]
    public List<GameObject> enemiesToReveal;

    private bool hasTriggered = false;

    public void TriggerPunishment()
    {
        if (hasTriggered) return; // Prevent triggering multiple times

        if (ShouldTriggerBasedOnNode(targetNodeID))
        {
            RevealEnemies();
            hasTriggered = true;
        }
        else
        {
            Debug.Log($"Punishment not triggered: Node {targetNodeID} is present.");
        }
    }

    private bool ShouldTriggerBasedOnNode(string nodeID)
    {
        // Check if the node is NOT in the path (i.e., the node is missing)
        bool nodeMissing = !DecisionManager.Instance.HasPassedNode(nodeID);

        // Debugging output
        Debug.Log($"Checking if node {nodeID} has been passed...");
        Debug.Log($"Path taken: {string.Join(", ", DecisionManager.Instance.GetPathTaken())}");
        Debug.Log($"Node missing: {nodeMissing}");

        return nodeMissing;
    }

    private void RevealEnemies()
    {
        foreach (var enemy in enemiesToReveal)
        {
            if (enemy != null)
            {
                enemy.SetActive(true);
            }
        }
        Debug.Log("Punishment triggered! Enemies revealed.");
    }
}
