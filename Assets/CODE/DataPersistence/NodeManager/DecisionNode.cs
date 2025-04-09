using System.Collections.Generic;

[System.Serializable]
public class DecisionNode
{
    public string nodeID; // Unique identifier
    public List<string> connectedNodes; // List of connected nodes

    // Constructor 
    public DecisionNode(string id, string[] connectedNodeIDs)
    {
        nodeID = id;
        connectedNodes = new List<string>(connectedNodeIDs);
    }

    public List<string> GetConnectedNodes()
    {
        return connectedNodes;
    }
}