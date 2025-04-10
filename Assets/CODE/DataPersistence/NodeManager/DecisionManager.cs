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
        // Declare Nodes
        // Room 1
        DecisionNode startNode = new DecisionNode("StartLevel1", new string[] { "Level1KeyCollected" });
        DecisionNode key1Collected = new DecisionNode("Level1KeyCollected", new string[] { "Level1Room1Door" });
        DecisionNode level1Room1Door = new DecisionNode("Level1Room1Door", new string[] { "Level1KillSpider", "Level1Room2Door" });

        // Room 2
        DecisionNode level1KillSpider = new DecisionNode("Level1KillSpider", new string[] { "Level1Room2Door" });
        DecisionNode level1Room2Door = new DecisionNode("Level1Room2Door", new string[] { "Level1SavePrisoner", "Level1KillPrisoner", "Level1Boss" });

        // Prisoner Room
        DecisionNode level1SavePrisoner = new DecisionNode("Level1SavePrisoner", new string[] { "Level1Exit1" });
        DecisionNode level1KillPrisoner = new DecisionNode("Level1KillPrisoner", new string[] { "Level1Boss" });

        // Boss and ending
        DecisionNode level1Boss = new DecisionNode("Level1Boss", new string[] { "Level1Exit2" });
        DecisionNode level1Exit1 = new DecisionNode("Level1Exit1", new string[] { "" });
        DecisionNode level1Exit2 = new DecisionNode("Level1Exit2", new string[] { "" });


        // Store the nodes in the decision tree
        decisionTree[startNode.nodeID] = startNode;
        decisionTree[key1Collected.nodeID] = key1Collected;
        decisionTree[level1Room1Door.nodeID] = level1Room1Door;

        decisionTree[level1KillSpider.nodeID] = level1KillSpider;
        decisionTree[level1Room2Door.nodeID] = level1Room2Door;

        decisionTree[level1SavePrisoner.nodeID] = level1SavePrisoner;
        decisionTree[level1KillPrisoner.nodeID] = level1KillPrisoner;

        decisionTree[level1Boss.nodeID] = level1Boss;
        decisionTree[level1Exit1.nodeID] = level1Exit1;
        decisionTree[level1Exit2.nodeID] = level1Exit2;

        // Set the current node to the starting point
        currentNode = startNode;
    }

    public bool SetCurrentNode(string nodeID)
    {
        if (currentNode.connectedNodes.Contains(nodeID))
        {
            if (!pathTaken.Contains(currentNode.nodeID))
            {
                pathTaken.Add(currentNode.nodeID);  // Store the previous node
            }

            currentNode = decisionTree[nodeID]; // Change to the new node

            if (!pathTaken.Contains(currentNode.nodeID))
            {
                pathTaken.Add(currentNode.nodeID);  // Add the new node to the path immediately
            }

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

    public bool HasPassedNode(string nodeID)
    {
        return pathTaken.Contains(nodeID);
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
