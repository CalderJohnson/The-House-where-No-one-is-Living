using System;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public List<string> inventoryItems;
    public List<WardrobeState> wardrobeStates;
    public List<CollectedItem> collectedItems;
    public List<ObjectInteractionState> objectInteractions; // Stores interaction counts

    public GameData()
    {
        inventoryItems = new List<string>();
        wardrobeStates = new List<WardrobeState>();
        collectedItems = new List<CollectedItem>();
        objectInteractions = new List<ObjectInteractionState>(); // Initialize list
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
