using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateNode : MonoBehaviour
{
    [Header("Decision System")]
    public bool affectsDecisionTree = false;
    public List<string> possibleDecisionNodeIDs; 


    public void TryUpdateDecisionNode(int choiceIndex)
    {
        if (!affectsDecisionTree || possibleDecisionNodeIDs == null || possibleDecisionNodeIDs.Count == 0)
            return;

        if (DecisionManager.Instance == null)
        {
            Debug.LogError("DecisionManager instance not found!");
            return;
        }

        if (choiceIndex < 0 || choiceIndex >= possibleDecisionNodeIDs.Count)
        {
            Debug.LogError($"Invalid choice index: {choiceIndex}");
            return;
        }

        string selectedNodeID = possibleDecisionNodeIDs[choiceIndex];

        bool success = DecisionManager.Instance.SetCurrentNode(selectedNodeID);

        if (success)
        {
            if (DataPersistenceManager.Instance != null)
            {
                DataPersistenceManager.Instance.SaveGame();
            }
            Debug.Log($"[UpdateNode] Decision node updated to: {selectedNodeID}");
        }
        else
        {
            Debug.LogWarning($"[UpdateNode] Failed to update decision node to: {selectedNodeID}");
        }
    }

    public void TryUpdateDecisionNode()
    {
        TryUpdateDecisionNode(0); // Call the new version with choiceIndex = 0
    }
}
