using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<string> items = new List<string>();

    public void AddItem(string item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
            Debug.Log("Added item: " + item);
        }
    }

    public bool HasItem(string item)
    {
        return items.Contains(item);
    }
}
