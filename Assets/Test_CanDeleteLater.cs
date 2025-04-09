using UnityEngine;

public class NPCInteraction : MonoBehaviour, IInteractable
{
    [Header("NPC Settings")]
    public string defaultDialogue = "Hello, traveler!";
    public string afterWardrobeDialogue = "I see you've been rummaging through the wardrobe...";
    public string decisionNodeID = "TalkedToNPCFirst"; // Node to set if player talks first

    public bool affectsDecisionTree = true; // Set to false if this NPC doesn't affect the tree

    public void Interact()
    {
        // Get the current decision node from the DecisionManager
        string currentNode = DecisionManager.Instance.GetCurrentNode().nodeID;
        string dialogueToShow = defaultDialogue;

        // If the wardrobe has been opened, change the dialogue
        if (currentNode == "WardrobeOpened")
        {
            dialogueToShow = afterWardrobeDialogue;
        }

        // Display the dialogue (Replace this with your actual dialogue system)
        Debug.Log(dialogueToShow);


    }
}
