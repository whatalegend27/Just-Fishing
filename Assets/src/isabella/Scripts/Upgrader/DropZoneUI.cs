using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// A drop zone where UI can be dragged then dropped.
public class DropZoneUI : MonoBehaviour, IDropHandler
{
    [SerializeField] private ItemScript.ItemType acceptedType; 
    [SerializeField] private Image icon;

    // Logic for when an item is dropped.
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

        // Grab the item that is in this slot before we overwrite it
        ItemScript oldItem = GetCurrentlyEquipped();

        // Equip new item
        bool success = RodUpgradeManager.Instance.TryEquip(dragged.item);

        if (success)
        {
            // Put old item back in inventory if it exists
            if (oldItem != null)
            {
                InventoryManager.Instance.AddItem(oldItem);
                Debug.Log($"Returned {oldItem.name} to inventory.");
            }

            icon.sprite = dragged.item.Icon;
            icon.color = Color.white;
            dragged.wasDropped = true;
            UpgradeInventoryUI.Instance.Refresh();
            
        }
    }

    // Helper method to see what the manager is currently holding for this slot type
    private ItemScript GetCurrentlyEquipped()
    {
        if (RodUpgradeManager.Instance == null) return null;

        return acceptedType switch
        {
            ItemScript.ItemType.Lure => RodUpgradeManager.Instance.equippedLure,
            ItemScript.ItemType.Bait => RodUpgradeManager.Instance.equippedBait,
            ItemScript.ItemType.Weight => RodUpgradeManager.Instance.equippedWeight,
            _ => null
        };
    }
}