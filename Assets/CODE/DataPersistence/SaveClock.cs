using UnityEngine;
using System;

public class SaveClock : MonoBehaviour
{
    public void Interact()
    {
        // Open a UI menu to choose Save or Load
        SaveLoadUI.Instance.ShowMenu();
    }
}
