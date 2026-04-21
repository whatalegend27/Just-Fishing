using UnityEngine;

// This script adds a fish to the player's inventory
// when fish has been caught by the fishing hook and destroyed.
//
// main idea:
// When fish gets attached to hook, it eventually gets destroyed.
// When unity destroys fish GameObject, OnDestroy() runs.
// use that moment to add fish's item script to the inventory.

public class FishToInventory : MonoBehaviour
{
    // Reference to fish component on the same game object
    // to access fish name and item script data
    private Fish fish;

    // Tracks if fish was caught by the hook
    // Prevents adding random destroyed fish to inventory
    private bool wasCaughtByHook = false;


    // ===== AWAKE FUNCTION =====
    void Awake()
    {
        // Attempt to find the fish script attached to prefab
        fish = GetComponent<Fish>();

        // Safety check help debugging if fish script missing
        if (fish == null)
        {
            Debug.LogError($"{gameObject.name} is missing Fish component.");
        }
    }


    // ===== UPDATE FUNCTION =====
    void Update()
    {
        // If fish has not yet caught
        // check if currently attached to fishing hook object
        if (!wasCaughtByHook && IsAttachedToFishingHook())
        {
            // Mark fish caught
            // This ensures only caught fish added to inventory
            wasCaughtByHook = true;
        }
    }


    // ===== ONDESTROY FUNCTION =====
    void OnDestroy()
    {
        // Only run while game playing
        // prevents errors when exiting play mode in editor
        if (!Application.isPlaying) return;

        // If fish never attached to hook, do nothing
        if (!wasCaughtByHook) return;

        // Safety check in case fish component is missing
        if (fish == null) return;


        // Get the sciptable object that represents this fish as an inventory item
        ItemScript item = fish.ItemData;

        // If no item data assigned warn dev
        if (item == null)
        {
            Debug.LogWarning($"{gameObject.name} is missing ItemScript reference on Fish.");
            return;
        }

        // Ensure inventory manager in scene 
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("InventoryManager not found in scene.");
            return;
        }


        // Attempt add the fish item to the inventory
        bool added = InventoryManager.Instance.AddItem(item);

        // If inventory full
        if (!added)
        {
            Debug.Log("Inventory full.");
        }
        else
        {
            // Pickup message
            Debug.Log($"{fish.FishName} added to inventory.");
        }
    }


    // ===== HOOK DETECTION =====
    // Checks whether this fish is currently attached to an object
    // that contains a fishign hook script somewhere 
    private bool IsAttachedToFishingHook()
    {
        // checking this object's parent
        Transform current = transform.parent;

        // Loop upward through all parents in hierarchy
        while (current != null)
        {
            // Get all mono behav scripts attached to parent object
            MonoBehaviour[] behaviours = current.GetComponents<MonoBehaviour>();

            // Check scripts
            foreach (MonoBehaviour behaviour in behaviours)
            {
                // Compare script name as text to "FishingHook"
                // This allows compatibility even if fishing hook is in a different assembly
                if (behaviour != null && behaviour.GetType().Name == "FishingHook")
                {
                    return true;
                }
            }

            // Move to the next parent
            current = current.parent;
        }

        // If no fishing hook script found anywhere in parent chain
        return false;
    }
}