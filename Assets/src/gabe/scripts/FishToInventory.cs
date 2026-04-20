using UnityEngine;

public class FishToInventory : MonoBehaviour
{
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

        ItemScript item = fish.ItemData;

        if (item == null)
        {
            Debug.LogWarning($"{gameObject.name} is missing ItemScript reference on Fish.");
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