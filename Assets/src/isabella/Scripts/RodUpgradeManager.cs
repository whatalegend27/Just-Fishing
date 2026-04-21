using UnityEngine;

public class RodUpgradeManager : MonoBehaviour
{
    public static RodUpgradeManager Instance { get; private set; }

    public ItemScript equippedLure;
    public ItemScript equippedBait;
    public ItemScript equippedWeight;

    public System.Action onUpgradeChanged;

    private void Awake()
    {
        // Simple singleton safety
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool TryEquip(ItemScript item)
    {
        if (item == null) return false;

        switch (item.Type)
        {
            case ItemScript.ItemType.Lure:
                equippedLure = item;
                break;

            case ItemScript.ItemType.Bait:
                equippedBait = item;
                break;

            case ItemScript.ItemType.Weight:
                equippedWeight = item;
                break;

            default:
                return false;
        }

        // Remove from inventory
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.RemoveItem(item);
        }

        onUpgradeChanged?.Invoke();
        return true;
    }

    public void Unequip(ItemScript.ItemType type)
    {
        ItemScript removedItem = null;

        switch (type)
        {
            case ItemScript.ItemType.Lure:
                removedItem = equippedLure;
                equippedLure = null;
                break;

            case ItemScript.ItemType.Bait:
                removedItem = equippedBait;
                equippedBait = null;
                break;

            case ItemScript.ItemType.Weight:
                removedItem = equippedWeight;
                equippedWeight = null;
                break;
        }

        if (removedItem != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(removedItem);
        }

        onUpgradeChanged?.Invoke();
    }
}