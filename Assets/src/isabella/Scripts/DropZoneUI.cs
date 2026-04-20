using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZoneUI : MonoBehaviour, IDropHandler
{
    [SerializeField] private ItemScript.ItemType acceptedType; 
    [SerializeField] private Image icon;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItemUI dragged = eventData.pointerDrag.GetComponent<DraggableItemUI>();

        if (dragged == null || dragged.item == null)
            return;

        if (dragged.item.Type != acceptedType)
        {
            Debug.Log("Wrong item type!");
            return;
        }

        bool success = RodUpgradeManager.Instance.TryEquip(dragged.item);

        if (success)
        {
            icon.sprite = dragged.item.Icon;
            icon.color = Color.white;
            dragged.wasDropped = true;

            
        }
    }
}