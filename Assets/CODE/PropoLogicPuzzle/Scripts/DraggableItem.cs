using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector2 originalPosition;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvas = GetComponentInParent<Canvas>(); // Find the UI canvas
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;

        // Get the cursor position and immediately move item there
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform, eventData.position, eventData.pressEventCamera, out Vector2 localCursor);

        rectTransform.anchoredPosition = localCursor; // Snap item under cursor

        Debug.Log($"[BEGIN DRAG] Snapping item to cursor. New Position: {rectTransform.anchoredPosition}, Cursor: {localCursor}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move object so it follows the cursor exactly
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform, eventData.position, eventData.pressEventCamera, out Vector2 localCursor);

        rectTransform.anchoredPosition = localCursor;

        Debug.Log($"[DRAG] Moving with cursor. Position: {rectTransform.anchoredPosition}, Cursor: {localCursor}");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        bool droppedInValidZone = transform.parent != originalParent; // If parent changes, assume valid

        if (!droppedInValidZone)
        {
            rectTransform.anchoredPosition = originalPosition;
            Debug.Log($"[END DRAG] Dropped in INVALID zone. Resetting to {originalPosition}");
        }
        else
        {
            Debug.Log($"[END DRAG] Dropped in VALID zone at {rectTransform.anchoredPosition}");
        }
    }
}
