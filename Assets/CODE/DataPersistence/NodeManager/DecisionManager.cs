using UnityEngine;
using System.Collections.Generic;

public class DecisionManager : MonoBehaviour
{
    public static DecisionManager Instance;

    private Dictionary<string, DecisionNode> decisionTree = new Dictionary<string, DecisionNode>();
    private string currentNode = "Start"; // Track where the player is

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateDecisionTree(); // Set up all choices
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CreateDecisionTree()
    {
        // Create Nodes
        DecisionNode startNode = new DecisionNode("Start", "You see a wardrobe and an NPC.");
        DecisionNode wardrobeOpened = new DecisionNode("WardrobeOpened", "You opened the wardrobe before talking to the NPC.");
        DecisionNode talkedToNPCFirst = new DecisionNode("TalkedToNPCFirst", "You talked to the NPC before opening the wardrobe.");

        // Add Choices
        startNode.AddChoice("Open the wardrobe", "WardrobeOpened");
        startNode.AddChoice("Talk to the NPC", "TalkedToNPCFirst");

        // Store Nodes
        decisionTree[startNode.nodeID] = startNode;
        decisionTree[wardrobeOpened.nodeID] = wardrobeOpened;
        decisionTree[talkedToNPCFirst.nodeID] = talkedToNPCFirst;
    }

    public void SetCurrentNode(string nodeID)
    {
        if (decisionTree.ContainsKey(nodeID))
        {
            currentNode = nodeID;
            Debug.Log("Current node set to: " + currentNode);
        }
        else
        {
            Debug.LogWarning("Node not found: " + nodeID);
        }
    }

    public DecisionNode GetCurrentNode()
    {
        return decisionTree.ContainsKey(currentNode) ? decisionTree[currentNode] : null;
    }
}
