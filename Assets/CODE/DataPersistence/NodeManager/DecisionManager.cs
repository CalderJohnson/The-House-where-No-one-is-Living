using UnityEngine;
using System.Collections.Generic;

public class DecisionManager : MonoBehaviour, IDataPersistence
{
    public static DecisionManager Instance { get; private set; } 

    private Dictionary<string, DecisionNode> decisionTree = new Dictionary<string, DecisionNode>();
    private List<string> pathTaken = new List<string>();
    private DecisionNode currentNode;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            CreateDecisionTree(); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    private void CreateDecisionTree()
    {
        // Declare the nodes
        DecisionNode startNode = new DecisionNode("Start", new string[] { "WardrobeOpened", "TalkedToNPCFirst" });
        DecisionNode wardrobeOpened = new DecisionNode("WardrobeOpened", new string[] {  });
        DecisionNode talkedToNPCFirst = new DecisionNode("TalkedToNPCFirst", new string[] {  });

        // Store the nodes in the decision tree
        decisionTree[startNode.nodeID] = startNode;
        decisionTree[wardrobeOpened.nodeID] = wardrobeOpened;
        decisionTree[talkedToNPCFirst.nodeID] = talkedToNPCFirst;

        // Set the current node to the starting point
        currentNode = startNode;
    }

    public bool SetCurrentNode(string nodeID)
    {
        if (currentNode.connectedNodes.Contains(nodeID))  
        {
            pathTaken.Add(currentNode.nodeID);  // Store previous node
            currentNode = decisionTree[nodeID]; // change node
            Debug.Log("Current node set to: " + currentNode.nodeID);

            return true;
        }
        else
        {
            Debug.LogWarning("Node ID " + nodeID + " is not a valid option from " + currentNode.nodeID);
            return false; 
        }
    }

    public DecisionNode GetCurrentNode()
    {
        return currentNode;
    }

    public List<string> GetPathTaken()
    {
        return new List<string>(pathTaken);
    }

    public void LoadData(GameData data)
    {
        if (data.decisionPath.Count > 0)
        {
            pathTaken = new List<string>(data.decisionPath);
            currentNode = decisionTree[data.currentDecisionNode];
        }
    }

    public void SaveData(ref GameData data)
    {
        data.currentDecisionNode = currentNode.nodeID;
        data.decisionPath = new List<string>(pathTaken);
    }
}
