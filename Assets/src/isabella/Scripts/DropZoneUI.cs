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

<<<<<<< HEAD
        // 1. Grab the item that is CURRENTLY in this slot before we overwrite it
        // We'll use a helper method (added below) to find it in the Manager
        ItemScript oldItem = GetCurrentlyEquipped();

        // 2. Try to equip the new one
=======
>>>>>>> e854fd2 (Initial upgrader feature in plus game stats in main menu)
        bool success = RodUpgradeManager.Instance.TryEquip(dragged.item);

        if (success)
        {
<<<<<<< HEAD
            // 3. If there was an old item, put it back in the inventory!
            if (oldItem != null)
            {
                InventoryManager.Instance.AddItem(oldItem);
                Debug.Log($"Returned {oldItem.name} to inventory.");
            }

            icon.sprite = dragged.item.Icon;
            icon.color = Color.white;
            dragged.wasDropped = true;
            UpgradeInventoryUI.Instance.Refresh();
            
            // Note: Make sure RodUpgradeManager.TryEquip handles 
            // InventoryManager.Instance.RemoveItem(dragged.item) inside its logic!
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
=======
            icon.sprite = dragged.item.Icon;
            icon.color = Color.white;
            dragged.wasDropped = true;

            
        }
    }
>>>>>>> e854fd2 (Initial upgrader feature in plus game stats in main menu)
}