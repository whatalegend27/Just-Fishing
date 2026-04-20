using UnityEngine;

public class FishRewardManager : MonoBehaviour
{
    private const int CATCH_GOLD = 10;
    private const int ITEM_REWARD_INTERVAL = 1;

    [SerializeField] private GoldManager goldManager;
    [SerializeField] private HealthRewardItem healthItem;
    [SerializeField] private RiskReductionItem riskItem;

    private void OnEnable()
    {
        FishDatabaseManager.OnFishRegistered += OnFishRegistered;
    }

    private void OnDisable()
    {
        FishDatabaseManager.OnFishRegistered -= OnFishRegistered;
    }

    private void OnFishRegistered(string fishName)
    {
        FishData fish = GetFishData(fishName);

        if (fish == null)
            return;

        if (goldManager != null)
            goldManager.AddGold(CATCH_GOLD);

        int total = GetTotalCatchCount();

        if (total % ITEM_REWARD_INTERVAL == 0)
            AwardRandomItem();
    }

    private static int GetTotalCatchCount()
    {
        if (FishDatabaseManager.Instance == null) return 0;
        int total = 0;
        foreach (FishData fish in FishDatabaseManager.Instance.fishDatabase)
            total += fish.catchCount;
        return total;
    }

    private void AwardRandomItem()
    {
        ItemScript[] choices = { healthItem, riskItem };
        ItemScript chosen = choices[UnityEngine.Random.Range(0, choices.Length)];

        if (chosen == null || goldManager == null)
            return;

        InventoryManager inventory = goldManager.GetComponent<InventoryManager>();
        if (inventory != null)
            inventory.AddItem(chosen);
    }

    private static FishData GetFishData(string fishName)
    {
        if (FishDatabaseManager.Instance == null) return null;
        foreach (FishData fish in FishDatabaseManager.Instance.fishDatabase)
        {
            if (fish.fishName == fishName)
                return fish;
        }
        return null;
    }
}
