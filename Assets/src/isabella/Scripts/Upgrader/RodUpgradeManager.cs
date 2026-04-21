using UnityEngine;

// A singleton that manages a player's rod upgrades.
public class RodUpgradeManager : MonoBehaviour
{
    public static RodUpgradeManager Instance { get; private set; }

    public ItemScript equippedLure;
    public ItemScript equippedBait;
    public ItemScript equippedWeight;

    public System.Action onUpgradeChanged;

    // Ensure only one instance of the manager exists and persists across scenes.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Handles inventory management when equipping items and notifies listeners of changes.
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

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.RemoveItem(item);
        }

        onUpgradeChanged?.Invoke();
        return true;
    }

    // Handles unequipping items and item return.
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

    // This is where the decorator is implemented.
    public int GetFishAttraction()
    {
        IFishAttraction attraction = new BaseAttraction();

        if (equippedLure != null)
            attraction = new LureDecorator(attraction, equippedLure);

        if (equippedBait != null)
            attraction = new BaitDecorator(attraction, equippedBait);

        if (equippedWeight != null)
            attraction = new WeightDecorator(attraction, equippedWeight);

        // Apply combo bonus if it exists.
        attraction = new ComboDecorator(
            attraction,
            equippedLure != null,
            equippedBait != null,
            equippedWeight != null
        );

        return attraction.GetAttraction();
    }
}