using System;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public List<string> inventoryItems;
    public List<WardrobeState> wardrobeStates;
    public List<CollectedItem> collectedItems;
    public List<ObjectInteractionState> objectInteractions; // Stores interaction counts
    public string currentDecisionNode; // Track where the player is in the decision tree
    public List<string> decisionPath; // Stores the full route the player took

    public GameData()
    {
        inventoryItems = new List<string>();
        wardrobeStates = new List<WardrobeState>();
        collectedItems = new List<CollectedItem>();
        objectInteractions = new List<ObjectInteractionState>(); // Initialize list
        currentDecisionNode = "Start"; // Default starting node
        decisionPath = new List<string>(); // Stores the full route taken
    }
}

[System.Serializable]
public class WardrobeState
{
    public string wardrobeID;
    public bool isOpen;

    public WardrobeState(string id, bool open)
    {
        wardrobeID = id;
        isOpen = open;
    }
}

[System.Serializable]
public class CollectedItem
{
    public string wardrobeID;
    public bool itemCollected;

    public CollectedItem(string id, bool collected)
    {
        wardrobeID = id;
        itemCollected = collected;
    }
}

[System.Serializable]
public class ObjectInteractionState
{
    public string objectID;
    public int interactionCount;

    public ObjectInteractionState(string id, int count)
    {
        objectID = id;
        interactionCount = count;
    }
}
