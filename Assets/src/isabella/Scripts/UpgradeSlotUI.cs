using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlotUI : MonoBehaviour
{
    [SerializeField] private ItemScript.ItemType slotType; // ✅ FIXED
    [SerializeField] private Image icon;

    private void OnEnable()
    {
        if (RodUpgradeManager.Instance != null)
        {
            RodUpgradeManager.Instance.onUpgradeChanged += Refresh;
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (RodUpgradeManager.Instance != null)
        {
            RodUpgradeManager.Instance.onUpgradeChanged -= Refresh;
        }
    }

    void Refresh()
    {
        if (RodUpgradeManager.Instance == null) 
        {
            return; 
        }

        ItemScript item = null;

        switch (slotType)
        {
            case ItemScript.ItemType.Lure:   // ✅ FIXED
                item = RodUpgradeManager.Instance.equippedLure;
                break;

            case ItemScript.ItemType.Bait:   // ✅ FIXED
                item = RodUpgradeManager.Instance.equippedBait;
                break;

            case ItemScript.ItemType.Weight: // ✅ FIXED
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

    public void OnClick()
    {
        RodUpgradeManager.Instance.Unequip(slotType); // ✅ now matches correct type
    }
}