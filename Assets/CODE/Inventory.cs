using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour, IDataPersistence
{
    public static Inventory Instance;
    private List<string> items = new List<string>(); // Stores inventory items

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
        DisplayInventory();
    }

    public void SaveData(ref GameData data)
    {
        data.inventoryItems = new List<string>(items); // Save inventory
    }

    public void AddItem(string itemName)
    {
        if (!items.Contains(itemName)) // Prevent duplicate collection
        {
            items.Add(itemName);
            Debug.Log("Added to inventory: " + itemName);
            DisplayInventory();
        }
        else
        {
            Debug.Log("Item already in inventory: " + itemName);
        }
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    public void DisplayInventory()
    {
        Debug.Log("Inventory contains: " + string.Join(", ", items));
    }
}
