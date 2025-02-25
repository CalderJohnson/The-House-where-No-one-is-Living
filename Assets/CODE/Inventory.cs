using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    private List<string> items = new List<string>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        Debug.Log("Added to inventory: " + itemName);
        DisplayInventory();
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
