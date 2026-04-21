using UnityEngine;
using NUnit.Framework;
using System.Reflection;

// Tests for the RodUpgradeManager's equipment management and attraction calculation
public class RodUpgradeManagerTests
{
    private GameObject managerObj;
    private RodUpgradeManager manager;
    // Setup method to initialize the RodUpgradeManager and InventoryManager before each test
    [SetUp]
    public void SetUp()
    {
        managerObj = new GameObject();
        manager = managerObj.AddComponent<RodUpgradeManager>();

        // Ensure InventoryManager exists (prevents null issues in TryEquip)
        var invObj = new GameObject();
        invObj.AddComponent<InventoryManager>();
    }

    // Clean up after each test
    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(managerObj);
    }

    // Helper method to create an ItemScript with a specific type
    private ItemScript CreateItem(ItemScript.ItemType type)
    {
        var item = ScriptableObject.CreateInstance<ItemScript>();

        typeof(ItemScript)
            .GetField("itemType", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(item, type);

        return item;
    }

    // Tests that equipping a lure sets the equippedLure field
    [Test]
    public void Equip_Lure_Sets_Lure()
    {
        var item = CreateItem(ItemScript.ItemType.Lure);

        manager.TryEquip(item);

        Assert.AreEqual(item, manager.equippedLure);
    }

    // Tests that equipping a bait sets the equippedBait field
    [Test]
    public void Equip_Bait_Sets_Bait()
    {
        var item = CreateItem(ItemScript.ItemType.Bait);

        manager.TryEquip(item);

        Assert.AreEqual(item, manager.equippedBait);
    }

    // Tests that equipping a weight sets the equippedWeight field
    [Test]
    public void Equip_Weight_Sets_Weight()
    {
        var item = CreateItem(ItemScript.ItemType.Weight);

        manager.TryEquip(item);

        Assert.AreEqual(item, manager.equippedWeight);
    }

    // Tests that equipping an item of the wrong type does not equip it
    [Test]
    public void Equip_Null_Returns_False()
    {
        bool result = manager.TryEquip(null);

        Assert.IsFalse(result);
    }

    // Tests that unequipping an item removes it from the equipped slot
    [Test]
    public void Unequip_Lure_Removes_It()
    {
        var item = CreateItem(ItemScript.ItemType.Lure);

        manager.equippedLure = item;

        manager.Unequip(ItemScript.ItemType.Lure);

        Assert.IsNull(manager.equippedLure);
    }

    // Tests that unequipping an item type that isn't equipped does nothing
    [Test]
    public void Attraction_Default_Is_Zero()
    {
        int value = manager.GetFishAttraction();

        Assert.AreEqual(0, value);
    }

    // Tests that equipping a single item adds its attraction value
    [Test]
    public void Attraction_All_Equipped_Adds_Combo_Bonus()
    {
        var lure = CreateItem(ItemScript.ItemType.Lure);
        var bait = CreateItem(ItemScript.ItemType.Bait);
        var weight = CreateItem(ItemScript.ItemType.Weight);

        lure.attractionValue = 1;
        bait.attractionValue = 1;
        weight.attractionValue = 1;

        manager.TryEquip(lure);
        manager.TryEquip(bait);
        manager.TryEquip(weight);

        int result = manager.GetFishAttraction();

        Assert.AreEqual(4, result);
    }

    // Tests that the onUpgradeChanged event is triggered when an upgrade is equipped
    [Test]
    public void OnUpgradeChanged_Is_Triggered()
    {
        var obj = new GameObject();
        var manager = obj.AddComponent<RodUpgradeManager>();

        bool triggered = false;
        manager.onUpgradeChanged += () => triggered = true;

        var item = ScriptableObject.CreateInstance<ItemScript>();

        typeof(ItemScript)
            .GetField("itemType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(item, ItemScript.ItemType.Lure);

        manager.TryEquip(item);

        Assert.IsTrue(triggered);
    }
}