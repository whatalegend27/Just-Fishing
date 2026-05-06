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
        if (!wasCaughtByHook && IsCaught())
        {
            wasCaughtByHook = true;
            Debug.Log($"{gameObject.name} was marked as caught.");
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
            Debug.LogWarning($"{gameObject.name} has no ItemData assigned.");
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("InventoryManager not found in scene.");
            return;
        }

        bool added = InventoryManager.Instance.AddItem(item);

        if (added)
        {
            Debug.Log($"{item.ItemName} added to inventory.");
        }
        else
        {
            Debug.Log("Inventory full.");
        }
    }

    private bool IsCaught()
    {
        return IsAttachedToFishingHook() || IsMovementDisabled();
    }

    private bool IsAttachedToFishingHook()
    {
        Transform current = transform.parent;

        while (current != null)
        {
            MonoBehaviour[] behaviours = current.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour != null &&
                    behaviour.GetType().Name.Contains("FishingHook"))
                {
                    return true;
                }
            }

            current = current.parent;
        }

        return false;
    }

    private bool IsMovementDisabled()
    {
        FishMovement movement = GetComponent<FishMovement>();
        return movement != null && movement.enabled == false;
    }
}