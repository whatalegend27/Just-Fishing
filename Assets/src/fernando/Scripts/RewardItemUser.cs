using UnityEngine;
using UnityEngine.UI;

public class RewardItemUser : MonoBehaviour
{
    [SerializeField] private HealthRewardItem healthItem;
    [SerializeField] private Button useButton;

    private Component mHealthStats;

    private void Start()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.inventoryChanged += RefreshButton;

        RefreshButton();
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.inventoryChanged -= RefreshButton;
    }

    private void RefreshButton()
    {
        if (useButton == null) return;

        bool hasItem = false;
        if (InventoryManager.Instance != null)
        {
            foreach (InventorySlotData slot in InventoryManager.Instance.slots)
            {
                if (slot.item == healthItem && slot.quantity > 0)
                {
                    hasItem = true;
                    break;
                }
            }
        }

        useButton.gameObject.SetActive(hasItem);
    }

    private Component GetHealthStats()
    {
        if (mHealthStats != null) return mHealthStats;
        foreach (MonoBehaviour mb in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (mb.GetType().Name == "HealthStats")
            {
                mHealthStats = mb;
                return mHealthStats;
            }
        }
        return null;
    }

    public void UseHealthItem()
    {
        Component healthStats = GetHealthStats();
if (InventoryManager.Instance == null || healthStats == null) return;

        bool hasItem = false;
        foreach (InventorySlotData slot in InventoryManager.Instance.slots)
        {
            if (slot.item == healthItem && slot.quantity > 0) { hasItem = true; break; }
        }
        if (!hasItem) return;

        InventoryManager.Instance.RemoveItem(healthItem);
        healthStats.GetType().GetMethod("Heal")?.Invoke(healthStats, null);
    }

}
