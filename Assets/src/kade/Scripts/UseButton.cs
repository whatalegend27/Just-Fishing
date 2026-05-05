using UnityEngine;

public class UseButton : MonoBehaviour
{
    [SerializeField] private HealthRewardItem healthItem;
    [SerializeField] private RiskReductionItem riskItem;
    private HealthStats mHealthStats;
    private ArrestStats mArrestStats;
    private GameObject inventoryDescription;

    void Awake()
    {
        mHealthStats = FindAnyObjectByType<HealthStats>();
        mArrestStats = FindAnyObjectByType<ArrestStats>();

        // Walk up the hierarchy to find the InventoryDescription panel
        Transform t = transform.parent;
        while (t != null)
        {
            if (t.name == "InventoryDescription") { inventoryDescription = t.gameObject; break; }
            t = t.parent;
        }
    }

    // Called by the button's OnClick event
    public void OnUse()
    {
        if (InventoryManager.Instance == null) return;

        StackableItem item = HasItem(healthItem) ? (StackableItem)healthItem
                           : HasItem(riskItem)   ? (StackableItem)riskItem
                           : null;

        if (item == null) return;

        InventoryManager.Instance.RemoveItem(item);

        switch (item)
        {
            case HealthRewardItem health:
                mHealthStats?.HealByAmount(health.HealAmount);
                break;
            case RiskReductionItem risk:
                mArrestStats?.ReduceRisk(risk.RiskReduction);
                break;
        }

        if (!HasItem(healthItem) && !HasItem(riskItem) && inventoryDescription != null)
            inventoryDescription.SetActive(false);
    }

    private bool HasItem(StackableItem item)
    {
        if (InventoryManager.Instance == null || item == null) return false;
        foreach (var slot in InventoryManager.Instance.slots)
            if (slot.item == item && slot.quantity > 0) return true;
        return false;
    }
}
