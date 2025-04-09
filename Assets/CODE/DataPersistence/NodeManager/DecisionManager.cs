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

        // Store nodes
        decisionTree[startNode.nodeID] = startNode;
        decisionTree[wardrobeOpened.nodeID] = wardrobeOpened;
        decisionTree[talkedToNPCFirst.nodeID] = talkedToNPCFirst;
    }


    public void SetCurrentNode(string nodeID)
    {
            pathTaken.Add(currentNode);
          
            currentNode = nodeID;
            Debug.Log("Current node set to: " + currentNode);
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
