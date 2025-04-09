using System.Collections.Generic;

[System.Serializable]
public class DecisionNode
{
    public string nodeID; // Unique identifier for this node
    public string description; // What happens at this node
    public Dictionary<string, string> choices; // Key = choice text, Value = next node ID
    public List<string> children; // Nodes that this node can lead to
    public string parent; // The node that led to this one

    public DecisionNode(string id, string desc)
    {
        nodeID = id;
        description = desc;
        choices = new Dictionary<string, string>();
        children = new List<string>();
        parent = null; // Default to no parent
    }

    public void AddChoice(string choiceText, string nextNodeID)
    {
        choices[choiceText] = nextNodeID;
        children.Add(nextNodeID);
    }

    public void SetParent(string parentNodeID)
    {
        parent = parentNodeID;
    }
}
