using UnityEngine;
using UnityEngine.UI;

// Manages the UI individual slots in upgrader. 
public class UpgradeItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private DraggableItemUI draggable;
    private ItemScript currentItem;

    // Set up the slot with the given inventory data, or clear it if the data is null or empty.
    public void SetUp(InventorySlotData data)
    {
        // If the data or the actual item is missing, clear the slot and stop
        if (data == null || data.item == null)
        {
            Clear();
            return;
        }

        currentItem = data.item;

        // Check if the Icon Image component was actually dragged into the Inspector
        if (icon != null)
        {
            icon.sprite = currentItem.Icon;
            icon.color = Color.white;
        }

        // Ensure the draggable script gets the item data
        if (draggable != null)
        {
            draggable.SetItem(currentItem);
        }
    }

    // Clears the slot, removing any item data and resetting the icon to a default state.
    public void Clear()
    {
        currentItem = null;
        icon.sprite = null;
        icon.color = Color.gray;
    }

    // Logic for when the slot is clicked, trying to equip the current item in this slot.
    void OnClick()
    {
        RodUpgradeManager.Instance.TryEquip(currentItem);
    }
}