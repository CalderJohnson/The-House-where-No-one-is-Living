using System.Collections.Generic;

[System.Serializable]
public class DecisionNode
{
    public string nodeID; // Unique identifier for this node
    public string description; // What happens at this node
    public Dictionary<string, string> choices; // Key = choice text, Value = next node ID

    public DecisionNode(string id, string desc)
    {
        nodeID = id;
        description = desc;
        choices = new Dictionary<string, string>();
    }

    public void AddChoice(string choiceText, string nextNodeID)
    {
        choices[choiceText] = nextNodeID;
    }
}

