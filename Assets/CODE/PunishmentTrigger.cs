using UnityEngine;
using System.Collections.Generic;

public class PunishmentTrigger : MonoBehaviour
{
    [Tooltip("Name of the node to check.")]
    public string targetNodeID;

    [Tooltip("Should the punishment trigger if the node is MISSING? (instead of present)")]
    public bool triggerIfNodeNotPassed = false;

    [Tooltip("Enemies to reveal when punishment is triggered.")]
    public List<GameObject> enemiesToReveal;

    private bool hasTriggered = false;

    public void TriggerPunishment()
    {
        if (hasTriggered) return; // Prevent triggering multiple times

        if (DecisionManager.Instance != null && ShouldTriggerBasedOnNode(targetNodeID))
        {
            RevealEnemies();
            hasTriggered = true;
        }
        else
        {
            Debug.Log($"Punishment not triggered: Condition not met for node {targetNodeID}.");
        }
    }

    private bool ShouldTriggerBasedOnNode(string nodeID)
    {
        bool passed = DecisionManager.Instance.HasPassedNode(nodeID);

        if (triggerIfNodeNotPassed)
        {
            return !passed; 
        }
        else
        {
            return passed;
        }
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
