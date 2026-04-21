using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FishRewardManager : MonoBehaviour
{
    private const int ITEM_REWARD_INTERVAL = 1;

    [SerializeField] private GoldManager goldManager;
    [SerializeField] private HealthRewardItem healthItem;
    [SerializeField] private RiskReductionItem riskItem;
    [SerializeField] private Button useButton;
    [SerializeField] private GameObject inventoryDescription;
    [SerializeField] private string sharkFishName = "BigBruce";
    [SerializeField] private string sharkSceneName = "SharkFight";
    private IHealable mHealthStats;
    private IRiskReducible mArrestStats;

    // Finds IHealable and IRiskReducible implementations in the scene at startup
    private void Awake()
    {
        foreach (var mb in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (mHealthStats == null && mb is IHealable h) mHealthStats = h;
            if (mArrestStats == null && mb is IRiskReducible r) mArrestStats = r;
            if (mHealthStats != null && mArrestStats != null) break;
        }
    }

    // Subscribes to fish registered event
    private void OnEnable()
    {
        FishDatabaseManager.OnFishRegistered += OnFishRegistered;
    }

    // Unsubscribes from fish registered event
    private void OnDisable()
    {
        FishDatabaseManager.OnFishRegistered -= OnFishRegistered;
    }

    // Returns true if the given item exists in inventory with quantity > 0
    private bool HasItem(StackableItem item)
    {
        if (InventoryManager.Instance == null || item == null) return false;
        foreach (InventorySlotData slot in InventoryManager.Instance.slots)
            if (slot.item == item && slot.quantity > 0) return true;
        return false;
    }

    // Consumes the first available item (health checked before risk) and applies its stat change
    public void UseItem()
    {
        if (InventoryManager.Instance == null) return;

        StackableItem item = HasItem(healthItem) ? healthItem
                           : HasItem(riskItem)   ? (StackableItem)riskItem
                           : null;

        if (item == null) return;

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

        if (!HasItem(healthItem) && !HasItem(riskItem) && inventoryDescription != null)
            inventoryDescription.SetActive(false);
    }

    // Awards gold based on rarity and grants a random item every ITEM_REWARD_INTERVAL catches
    private void OnFishRegistered(string fishName)
    {
        if (fishName == sharkFishName)
        {
            SceneManager.LoadScene(sharkSceneName);
            return;
        }

        FishData fish = GetFishData(fishName);

        if (fish == null)
            return;

        if (goldManager != null)
        {
            FishCatchReward reward = fish.rarity switch
            {
                FishRarity.Rare => new RareFishCatchReward(),
                FishRarity.Legendary => new LegendaryFishCatchReward(),
                _ => new CommonFishCatchReward()
            };
            goldManager.AddGold(reward.Award());
        }

        int total = GetTotalCatchCount();

        if (total % ITEM_REWARD_INTERVAL == 0)
            AwardRandomItem();
    }

    // Sums catch counts across all fish in the database
    private static int GetTotalCatchCount()
    {
        if (FishDatabaseManager.Instance == null) return 0;
        int total = 0;
        foreach (FishData fish in FishDatabaseManager.Instance.fishDatabase)
            total += fish.catchCount;
        return total;
    }

    // Picks a random item from the reward pool and adds it to the inventory
    private void AwardRandomItem()
    {
        ItemScript[] choices = { healthItem, riskItem };
        ItemScript chosen = choices[UnityEngine.Random.Range(0, choices.Length)];

        if (chosen == null)
            return;

        InventoryManager inventory = InventoryManager.Instance != null
            ? InventoryManager.Instance
            : goldManager != null ? goldManager.GetComponent<InventoryManager>() : null;

        if (inventory != null)
            inventory.AddItem(chosen);
    }

    // Looks up a FishData entry by name; returns null if not found
    private static FishData GetFishData(string fishName)
    {
        if (FishDatabaseManager.Instance == null) return null;
        return FishDatabaseManager.Instance.fishDatabase.Find(f => f.fishName == fishName);
    }
}
