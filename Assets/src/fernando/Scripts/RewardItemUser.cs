using UnityEngine;
using UnityEngine.UI;

public class RewardItemUser : MonoBehaviour
{
    [SerializeField] private StackableItem item;
    [SerializeField] private Button useButton;

    private IHealable mHealthStats;
    private IRiskReducible mArrestStats;

    // Finds IHealable and IRiskReducible implementations in the scene at startup
    private void Awake()
    {
        foreach (var mb in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (mHealthStats == null && mb is IHealable h)   mHealthStats  = h;
            if (mArrestStats == null && mb is IRiskReducible r) mArrestStats = r;
            if (mHealthStats != null && mArrestStats != null) break;
        }
    }

    // Subscribes to inventory changes so the button visibility stays in sync
    private void Start()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.inventoryChanged += RefreshButton;

        RefreshButton();
    }

    // Unsubscribes from inventory changes on destroy
    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.inventoryChanged -= RefreshButton;
    }

    // Shows or hides the use button depending on whether the assigned item is in inventory
    private void RefreshButton()
    {
        if (useButton == null) return;

        bool hasItem = false;
        if (InventoryManager.Instance != null && item != null)
        {
            foreach (InventorySlotData slot in InventoryManager.Instance.slots)
            {
                if (slot.item == item && slot.quantity > 0) { hasItem = true; break; }
            }
        }

        useButton.gameObject.SetActive(hasItem);
    }

    // Consumes the item and applies the correct stat change based on its type
    public void UseItem()
    {
        if (InventoryManager.Instance == null || item == null) return;

        bool hasItem = false;
        foreach (InventorySlotData slot in InventoryManager.Instance.slots)
        {
            if (slot.item == item && slot.quantity > 0) { hasItem = true; break; }
        }
        if (!hasItem) return;

        InventoryManager.Instance.RemoveItem(item);

        switch (item)
        {
            case HealthRewardItem:
                mHealthStats?.Heal();
                break;
            case RiskReductionItem risk:
                mArrestStats?.ReduceRisk(risk.RiskReduction);
                break;
        }
    }
}
