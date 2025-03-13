using System;
using System.Collections.Generic; 

[System.Serializable]
public class GameData
{
    public List<string> inventoryItems;
    public List<WardrobeState> wardrobeStates; 
    public List<CollectedItem> collectedItems; 

    public GameData()
    {
        inventoryItems = new List<string>();
        wardrobeStates = new List<WardrobeState>(); 
        collectedItems = new List<CollectedItem>(); 
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
