using UnityEngine;
using Saif.GamePlay;

public class FishInventoryPickup : MonoBehaviour
{
    [Header("Inventory Item")]
    public ItemScript fishItem;

    private bool wasCaught = false;

    void Update()
    {
        // detect when hook grabs the fish
        if (!wasCaught && transform.parent != null)
        {
            if (transform.parent.GetComponent<FishingHook>() != null)
            {
                wasCaught = true;
            }
        }
    }

    private void OnDestroy()
    {
        // only add to inventory if fish was caught
        if (!wasCaught) return;

        if (InventoryManager.Instance == null) return;

        if (fishItem == null)
        {
            Debug.LogWarning(
                "FishInventoryPickup: No ItemScript assigned on "
                + gameObject.name
            );
            return;
        }

        bool added =
            InventoryManager.Instance.AddItem(fishItem);

        if (!added)
        {
            Debug.Log("Inventory full!");
        }
    }
}