using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    // Declare variables to store here. 
    public List<string> inventoryItems; // Store inventory as a list of item names
    public HashSet<string> collectedItems;


    public GameData()
    {
        // Set default values here
        inventoryItems = new List<string>(); // Initialize list
        collectedItems = new HashSet<string>();
    }
}
