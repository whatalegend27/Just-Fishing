using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Allows an item to be dragged and dropped in UI.
public class DraggableItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemScript item;
    private Transform originalParent;
    private Canvas canvas;
    private Image image;
    private CanvasGroup canvasGroup;

    public GridLayoutGroup grid;

    public bool wasDropped = false;

    // Disables raycast and allows the parent to be changed during dragging.
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Drag Started on: " + gameObject.name); // TEST 1
        if (item == null) return;

        wasDropped = false; // reset
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
        grid.enabled = false; // Disable grid to prevent snapping during drag
    }

    // Updates the position of the item to follow the pointer during dragging.
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (!wasDropped)
        {
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
        }
        grid.enabled = true; // Re-enable grid after drag
    }

    // Updates the position of the item to follow the pointer during dragging.
    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();

        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    // Changes the icon of the item based on the itemscript.
    public void SetItem(ItemScript newItem)
    {
        item = newItem;
        image.sprite = item != null ? item.Icon : null;
    }

    // Updates the position of the item to follow the pointer during dragging.
    public void OnDrag(PointerEventData eventData)
    {
        if (item == null) return;

        transform.position = eventData.position;
    }

}