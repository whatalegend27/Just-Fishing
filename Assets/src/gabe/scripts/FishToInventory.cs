using UnityEngine;

public class FishToInventory : MonoBehaviour
{
    [Header("Database of fish items")]
    [SerializeField] private ItemScript[] fishItems;

    private Fish fish;
    private bool wasCaughtByHook = false;

    void Awake()
    {
        fish = GetComponent<Fish>();

        if (fish == null)
        {
            Debug.LogError($"{gameObject.name} is missing Fish component.");
        }
    }

    void Update()
    {
        if (!wasCaughtByHook && IsAttachedToFishingHook())
        {
            wasCaughtByHook = true;
        }
    }

    void OnDestroy()
    {
        if (!Application.isPlaying) return;
        if (!wasCaughtByHook) return;
        if (fish == null) return;

        ItemScript item = FindMatchingItem(fish.FishName);

        if (item == null)
        {
            Debug.LogWarning($"No ItemScript found for fish name: {fish.FishName}");
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("InventoryManager not found in scene.");
            return;
        }

        bool added = InventoryManager.Instance.AddItem(item);

        if (!added)
        {
            Debug.Log("Inventory full.");
        }
        else
        {
            Debug.Log($"{fish.FishName} added to inventory.");
        }
    }

    private ItemScript FindMatchingItem(string fishName)
    {
        foreach (ItemScript item in fishItems)
        {
            if (item != null && item.ItemName == fishName)
            {
                return item;
            }
        }

        return null;
    }

    private bool IsAttachedToFishingHook()
    {
        Transform current = transform.parent;

        while (current != null)
        {
            MonoBehaviour[] behaviours = current.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour != null && behaviour.GetType().Name == "FishingHook")
                {
                    return true;
                }
            }

            current = current.parent;
        }

        return false;
    }
}