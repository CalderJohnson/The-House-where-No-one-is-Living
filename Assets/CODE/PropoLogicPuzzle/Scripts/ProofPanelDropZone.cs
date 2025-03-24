using UnityEngine;
using UnityEngine.EventSystems;

public class ProofPanelDropZone : MonoBehaviour, IDropHandler
{
    public ProofController proofController; // Reference to the proof system

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggedItem = eventData.pointerDrag?.GetComponent<DraggableItem>();

        if (draggedItem != null)
        {
            ProofLine proofLine = draggedItem.GetComponent<ProofLine>();

            if (proofLine != null)
            {
                Debug.Log("Dropped proof line: " + proofLine.logicText.text);
                proofController.AddProofLine(proofLine.logicText.text, "Added via drag and drop");
            }
        }
    }
}

