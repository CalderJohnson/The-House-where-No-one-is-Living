using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateNode : MonoBehaviour
{
    [Header("Decision System")]
    public bool affectsDecisionTree = false;
    public string decisionNodeID;

    // Public method you can call from any other script
    public void TryUpdateDecisionNode()
    {
        if (!affectsDecisionTree || string.IsNullOrEmpty(decisionNodeID))
            return;

        if (DecisionManager.Instance == null)
        {
            Debug.LogError("DecisionManager instance not found!");
            return;
        }

        bool success = DecisionManager.Instance.SetCurrentNode(decisionNodeID);

        if (success)
        {
            if (DataPersistenceManager.Instance != null)
            {
                DataPersistenceManager.Instance.SaveGame();
            }
            Debug.Log($"[UpdateNode] Decision node updated to: {decisionNodeID}");
        }
        else
        {
            Debug.LogWarning($"[UpdateNode] Failed to update decision node to: {decisionNodeID}");
        }
    }
}

