using UnityEngine;
using System.Collections.Generic;

public class DecisionManager : MonoBehaviour, IDataPersistence
{
    public static DecisionManager Instance;

    private Dictionary<string, DecisionNode> decisionTree = new Dictionary<string, DecisionNode>();
    private List<string> pathTaken = new List<string>(); // Stores nodes visited in order
    private string currentNode = "Start"; // Track where the player is

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
        DecisionNode startNode = new DecisionNode("Start", "You see a wardrobe and an NPC.");
        DecisionNode wardrobeOpened = new DecisionNode("WardrobeOpened", "You opened the wardrobe before talking to the NPC.");
        DecisionNode talkedToNPCFirst = new DecisionNode("TalkedToNPCFirst", "You talked to the NPC before opening the wardrobe.");

        startNode.AddChoice("Open the wardrobe", "WardrobeOpened");
        startNode.AddChoice("Talk to the NPC", "TalkedToNPCFirst");

        // Temp
        talkedToNPCFirst.AddRequiredBranch("WardrobeOpened");

        // Store nodes
        decisionTree[startNode.nodeID] = startNode;
        decisionTree[wardrobeOpened.nodeID] = wardrobeOpened;
        decisionTree[talkedToNPCFirst.nodeID] = talkedToNPCFirst;
    }

    public bool CanMoveToNode(string nodeID)
    {
        if (!decisionTree.ContainsKey(nodeID)) return false;

        DecisionNode node = decisionTree[nodeID];

        // If node has no restrictions, it can be accessed freely
        if (node.requiredBranches.Count == 0) return true;

        // Otherwise, only allow access if all required nodes were visited
        foreach (string requiredNode in node.requiredBranches)
        {
            if (!pathTaken.Contains(requiredNode))
            {
                return false;
            }
        }

        return true;
    }

    public void SetCurrentNode(string nodeID)
    {
        if (CanMoveToNode(nodeID))
        {
            if (!pathTaken.Contains(currentNode)) // Avoid duplicate history entries
            {
                pathTaken.Add(currentNode);
            }
            currentNode = nodeID;
            Debug.Log("Current node set to: " + currentNode);
        }
        else
        {
            Debug.LogWarning("Cannot move to node: " + nodeID + " due to past choices.");
        }
    }

    public DecisionNode GetCurrentNode()
    {
        return decisionTree.ContainsKey(currentNode) ? decisionTree[currentNode] : null;
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
            currentNode = data.currentDecisionNode;
        }
    }

    public void SaveData(ref GameData data)
    {
        data.currentDecisionNode = currentNode;
        data.decisionPath = new List<string>(pathTaken);
    }
}
