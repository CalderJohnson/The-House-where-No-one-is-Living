using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, IDataPersistence
{
    public static Inventory Instance;
    private List<string> items = new List<string>(); // Stores inventory items
    private HashSet<string> collectedItems = new HashSet<string>(); // Tracks collected items

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void LoadData(GameData data)
    {
        items = new List<string>(data.inventoryItems); // Load inventory
        collectedItems = new HashSet<string>(data.collectedItems); // Load collected items
        DisplayInventory();
    }

    public void SaveData(ref GameData data)
    {
        data.inventoryItems = new List<string>(items); // Save inventory
        data.collectedItems = new HashSet<string>(collectedItems); // Save collected items correctly
    }

    public void AddItem(string itemName)
    {
        if (!collectedItems.Contains(itemName)) // Prevent duplicate collection
        {
            items.Add(itemName);
            collectedItems.Add(itemName); // Mark as collected
            Debug.Log("Added to inventory: " + itemName);
            DisplayInventory();
        }
        else
        {
            Debug.Log("Item already collected: " + itemName);
        }
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    public bool HasCollectedItem(string itemName)
    {
        return collectedItems.Contains(itemName);
    }

    public void DisplayInventory()
    {
        Debug.Log("Inventory contains: " + string.Join(", ", items));
    }
}
