using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class FishRewardTests
{
    private GameObject dbObject;
    private GameObject inventoryObject;
    private GameObject rewardObject;
    private FishDatabaseManager db;
    private FishRewardManager rewardManager;
    private HealthRewardItem healthItem;
    private RiskReductionItem riskItem;

    [SetUp]
    public void Setup()
    {
        dbObject = new GameObject("FishDatabaseTest");
        db = dbObject.AddComponent<FishDatabaseManager>();
        db.fishDatabase = new List<FishData>();
        for (int i = 0; i < 20; i++)
            db.fishDatabase.Add(new FishData { fishName = $"Fish_{i}", fishKnown = false });
        SetStaticInstance(db);

        inventoryObject = new GameObject("InventoryManager");
        var im = inventoryObject.AddComponent<InventoryManager>();
        im.slots = new InventorySlotData[9];
        for (int i = 0; i < 9; i++) im.slots[i] = new InventorySlotData();
        SetStaticInstance(im);

        rewardObject = new GameObject("RewardManager");
        rewardManager = rewardObject.AddComponent<FishRewardManager>();
        SubscribeRewardManager(rewardManager);

        healthItem = ScriptableObject.CreateInstance<HealthRewardItem>();
        riskItem = ScriptableObject.CreateInstance<RiskReductionItem>();

        SetPrivateField(rewardManager, "healthItem", healthItem);
        SetPrivateField(rewardManager, "riskItem", riskItem);
    }

    [TearDown]
    public void TearDown()
    {
        UnsubscribeRewardManager(rewardManager);
        FishDatabaseManager.ResetInstance();
        ResetSingletonInstance<InventoryManager>();
        Object.DestroyImmediate(dbObject);
        Object.DestroyImmediate(inventoryObject);
        Object.DestroyImmediate(rewardObject);
        Object.DestroyImmediate(healthItem);
        Object.DestroyImmediate(riskItem);
    }

    // --- HealthRewardItem ---

    [Test]
    public void HealthRewardItem_DefaultHealAmount_IsTwenty()
    {
        var item = ScriptableObject.CreateInstance<HealthRewardItem>();
        Assert.AreEqual(20, item.HealAmount);
        Object.DestroyImmediate(item);
    }

    [Test]
    public void HealthRewardItem_CanStack_ReturnsTrue()
    {
        var item = ScriptableObject.CreateInstance<HealthRewardItem>();
        Assert.IsTrue(item.CanStack());
        Object.DestroyImmediate(item);
    }

    // --- RiskReductionItem ---

    [Test]
    public void RiskReductionItem_DefaultRiskReduction_IsTen()
    {
        var item = ScriptableObject.CreateInstance<RiskReductionItem>();
        Assert.AreEqual(10, item.RiskReduction);
        Object.DestroyImmediate(item);
    }

    [Test]
    public void RiskReductionItem_CanStack_ReturnsTrue()
    {
        var item = ScriptableObject.CreateInstance<RiskReductionItem>();
        Assert.IsTrue(item.CanStack());
        Object.DestroyImmediate(item);
    }

    // --- FishRewardManager: total catch count ---

    [Test]
    public void FishRewardManager_TotalCatchCount_StartsAtZero()
    {
        int total = GetTotalCatchCount();
        Assert.AreEqual(0, total);
    }

    [Test]
    public void FishRewardManager_TotalCatchCount_SumsAcrossAllFish()
    {
        db.RegisterFish("Fish_0");
        db.RegisterFish("Fish_1");
        db.RegisterFish("Fish_2");

        Assert.AreEqual(3, GetTotalCatchCount());
    }

    [Test]
    public void FishRewardManager_TotalCatchCount_IncludesMultipleCatchesSameFish()
    {
        db.RegisterFish("Fish_0");
        db.RegisterFish("Fish_0");
        db.RegisterFish("Fish_0");

        Assert.AreEqual(3, GetTotalCatchCount());
    }

    // --- FishRewardManager: item reward interval ---

    [Test]
    public void FishRewardManager_ItemNotAwarded_BeforeTenCatches()
    {
        for (int i = 0; i < 9; i++)
            db.RegisterFish($"Fish_{i}");

        Assert.IsTrue(InventoryIsEmpty(), "No item should be awarded before 10 total catches.");
    }

    [Test]
    public void FishRewardManager_ItemAwarded_AtExactlyTenCatches()
    {
        for (int i = 0; i < 10; i++)
            db.RegisterFish($"Fish_{i}");

        Assert.IsFalse(InventoryIsEmpty(), "An item should be awarded at exactly 10 total catches.");
    }

    [Test]
    public void FishRewardManager_ItemNotAwarded_AfterEleventhCatch()
    {
        for (int i = 0; i < 10; i++)
            db.RegisterFish($"Fish_{i}");

        ClearInventory();

        db.RegisterFish("Fish_10");

        Assert.IsTrue(InventoryIsEmpty(), "No item should be awarded on the 11th catch.");
    }

    [Test]
    public void FishRewardManager_ItemAwarded_AgainAtTwentyCatches()
    {
        for (int i = 0; i < 20; i++)
            db.RegisterFish($"Fish_{i % 20}");

        int itemCount = CountInventoryItems();
        Assert.GreaterOrEqual(itemCount, 2, "An item should be awarded at both 10 and 20 total catches.");
    }

    // --- FishRewardManager: item validity ---

    [Test]
    public void FishRewardManager_ItemAwarded_IsOneOfTheTwoChoices()
    {
        for (int i = 0; i < 10; i++)
            db.RegisterFish($"Fish_{i}");

        ItemScript awardedItem = null;
        foreach (var slot in InventoryManager.Instance.slots)
        {
            if (slot.item != null) { awardedItem = slot.item; break; }
        }

        Assert.IsTrue(
            awardedItem == healthItem || awardedItem == riskItem,
            "Awarded item must be either the health item or the risk item.");
    }

    [Test]
    public void FishRewardManager_NoItemAwarded_WhenItemsNotAssigned()
    {
        SetPrivateField(rewardManager, "healthItem", null);
        SetPrivateField(rewardManager, "riskItem", null);

        for (int i = 0; i < 10; i++)
            db.RegisterFish($"Fish_{i}");

        Assert.IsTrue(InventoryIsEmpty(), "No item should be awarded when reward items are not assigned.");
    }

    [Test]
    public void FishRewardManager_GoldAwarded_IncreasesPlayerGold()
    {
        var goldObject = new GameObject("GoldManager");
        var gm = goldObject.AddComponent<GoldManager>();
        SetStaticInstance(gm);
        SetPrivateField(rewardManager, "goldManager", gm);

        int goldBefore = (int)typeof(GoldManager)
            .GetField("playerGold", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(gm);

        db.RegisterFish("Fish_0");

        int goldAfter = (int)typeof(GoldManager)
            .GetField("playerGold", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(gm);

        Assert.AreEqual(goldBefore + 10, goldAfter, "Gold should increase by 10 on each catch.");

        ResetSingletonInstance<GoldManager>();
        Object.DestroyImmediate(goldObject);
    }

    // --- Helpers ---

    private static int GetTotalCatchCount()
    {
        int total = 0;
        foreach (FishData fish in FishDatabaseManager.Instance.fishDatabase)
            total += fish.catchCount;
        return total;
    }

    private static bool InventoryIsEmpty()
    {
        foreach (var slot in InventoryManager.Instance.slots)
            if (slot.item != null) return false;
        return true;
    }

    private static int CountInventoryItems()
    {
        int count = 0;
        foreach (var slot in InventoryManager.Instance.slots)
            if (slot.item != null) count += slot.quantity;
        return count;
    }

    private static void ClearInventory()
    {
        foreach (var slot in InventoryManager.Instance.slots)
            slot.item = null;
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(target, value);
    }

    private static void ResetSingletonInstance<T>()
    {
        FieldInfo backing = typeof(T).GetField(
            "<Instance>k__BackingField",
            BindingFlags.NonPublic | BindingFlags.Static);
        backing?.SetValue(null, null);
    }

    private static void SetStaticInstance<T>(T value)
    {
        FieldInfo backing = typeof(T).GetField(
            "<Instance>k__BackingField",
            BindingFlags.NonPublic | BindingFlags.Static);
        backing?.SetValue(null, value);
    }

    private static void SubscribeRewardManager(FishRewardManager manager)
    {
        typeof(FishRewardManager)
            .GetMethod("OnEnable", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(manager, null);
    }

    private static void UnsubscribeRewardManager(FishRewardManager manager)
    {
        typeof(FishRewardManager)
            .GetMethod("OnDisable", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(manager, null);
    }
}
