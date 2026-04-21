using UnityEngine;
using UnityEngine.UI;

// Manages the UI for the equipped upgrade slots in the upgrader.
public class UpgradeSlotUI : MonoBehaviour
{
    [SerializeField] private ItemScript.ItemType slotType; 
    [SerializeField] private Image icon;

    //Refresh the UI when enabled, and subscribe to changes in the upgrade manager to keep it updated.
    private void OnEnable()
    {
        if (RodUpgradeManager.Instance != null)
        {
            RodUpgradeManager.Instance.onUpgradeChanged += Refresh;
        }

        Refresh();
    }

    // Don't allow memory leaks by unsubscribing when disabled.
    private void OnDisable()
    {
        if (RodUpgradeManager.Instance != null)
        {
            RodUpgradeManager.Instance.onUpgradeChanged -= Refresh;
        }
    }


    // Refresh the UI based on the currently equipped item for this slot type, or clear it if none is equipped.
    void Refresh()
    {
        if (RodUpgradeManager.Instance == null) 
        {
            return; 
        }

        ItemScript item = null;

        switch (slotType)
        {
            case ItemScript.ItemType.Lure:
                item = RodUpgradeManager.Instance.equippedLure;
                break;

            case ItemScript.ItemType.Bait:  
                item = RodUpgradeManager.Instance.equippedBait;
                break;

            case ItemScript.ItemType.Weight: 
                item = RodUpgradeManager.Instance.equippedWeight;
                break;
        }

        if (item == null)
        {
            icon.sprite = null;
            icon.color = Color.gray;
        }
        else
        {
            icon.sprite = item.Icon;
            icon.color = Color.white;
        }
    }

    // Logic for when the slot is clicked, trying to unequip the item in this slot.
    public void OnClick()
    {
        RodUpgradeManager.Instance.Unequip(slotType); 
    }
}