using UnityEngine;
using System;

public class SaveClock : MonoBehaviour
{
    public void Interact()
    {
        if (SaveLoadUI.Instance != null)
        {
            SaveLoadUI.Instance.ShowMenu();
        }
        else
        {
            Debug.LogError("SaveLoadUI Instance is missing!");
        }
    }
}
