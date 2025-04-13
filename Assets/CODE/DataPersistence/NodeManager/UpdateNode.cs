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

        if (choiceIndex < 0 || choiceIndex >= possibleDecisionNodeIDs.Count)
        {
            Debug.LogError($"Invalid choice index: {choiceIndex}");
            return;
        }

        string selectedNodeID = possibleDecisionNodeIDs[choiceIndex];

        bool success = DecisionManager.Instance.SetCurrentNode(selectedNodeID);
    }

    public void TryUpdateDecisionNode()
    {
        TryUpdateDecisionNode(0); 
    }
}
