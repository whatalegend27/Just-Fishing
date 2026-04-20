using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemScript item;
    private Transform originalParent;
    private Canvas canvas;
    private Image image;
    private CanvasGroup canvasGroup;

    public GridLayoutGroup grid;

    public bool wasDropped = false;

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

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();

        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void SetItem(ItemScript newItem)
    {
        item = newItem;
        image.sprite = item != null ? item.Icon : null;
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (item == null) return;

        transform.position = eventData.position;
    }

}