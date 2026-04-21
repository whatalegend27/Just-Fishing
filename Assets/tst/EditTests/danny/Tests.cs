using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class InventoryTest
{
    private GameObject inventoryGO;
    private GameObject goldGO;
    private InventoryManager inventoryManager;
    private GoldManager goldManager;

    private List<ScriptableObject> createdObjects;

    [SetUp]
    public void SetUp()
    {
        createdObjects = new List<ScriptableObject>();

        inventoryGO = new GameObject("InventoryManager");
        inventoryManager = inventoryGO.AddComponent<InventoryManager>();

        goldGO = new GameObject("GoldManager");
        goldManager = goldGO.AddComponent<GoldManager>();

        if (inventoryManager.slots == null || inventoryManager.slots.Length != 9)
        {
            inventoryManager.slots = new InventorySlotData[9];
        }

        for (int i = 0; i < 9; i++)
        {
            inventoryManager.slots[i] = new InventorySlotData();
        }

        var flags = BindingFlags.Public | BindingFlags.Static;

        typeof(InventoryManager).GetProperty("Instance", flags)
            ?.SetValue(null, inventoryManager);

        typeof(GoldManager).GetProperty("Instance", flags)
            ?.SetValue(null, goldManager);
    }

    [TearDown]
    public void TearDown()
    {
        InventoryManager.ResetInstance();
        GoldManager.ResetInstance();
        Object.DestroyImmediate(inventoryGO);
        Object.DestroyImmediate(goldGO);

        foreach (var obj in createdObjects)
        {
            if (obj != null)
            {
                Object.DestroyImmediate(obj);
            }
        }
        createdObjects.Clear();
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private void SetItemPrice(ItemScript item, int price)
    {
        var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        string[] possibleFields = { "price", "_price", "<Price>k__BackingField" };
        foreach (var fieldName in possibleFields)
        {
            var field = typeof(ItemScript).GetField(fieldName, flags);
            if (field != null)
            {
                field.SetValue(item, price);
                return;
            }
        }

        var prop = typeof(ItemScript).GetProperty("Price", flags);
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(item, price);
        }
    }

    private ItemScript CreateItemWithPrice(int price, string itemName = "TestItem")
    {
        ItemScript item = ScriptableObject.CreateInstance<ItemScript>();
        item.name = itemName;
        createdObjects.Add(item);
        SetItemPrice(item, price);
        return item;
    }

    private StackableItem CreateStackableWithPrice(int price, string itemName = "StackableItem")
    {
        StackableItem item = ScriptableObject.CreateInstance<StackableItem>();
        item.name = itemName;
        createdObjects.Add(item);
        SetItemPrice(item, price);
        return item;
    }

    // ── Inventory Tests ───────────────────────────────────────────────────

    // Verifies that the AddItem method returns false and does nothing when passed a null reference
    [Test]
    public void Test01_AddItem_NullItem_ReturnsFalse()
    {
        bool result = inventoryManager.AddItem(null);
        Assert.IsFalse(result);
    }

    // Verifies that the inventory starts completely empty with all quantities at zero
    [Test]
    public void Test02_Inventory_InitialState_AllSlotsAreEmpty()
    {
        foreach (var slot in inventoryManager.slots)
        {
            Assert.IsNull(slot.item, "All slots should start with a null item.");
            Assert.AreEqual(0, slot.quantity, "All slot quantities should start at 0.");
        }
    }

    // Verifies that a standard non-stackable item is successfully added to the first slot
    [Test]
    public void Test03_AddItem_ValidNonStackableItem_ReturnsTrue()
    {
        ItemScript item = CreateItemWithPrice(0);
        bool result = inventoryManager.AddItem(item);
        Assert.IsTrue(result);
        Assert.AreEqual(item, inventoryManager.slots[0].item);
    }

    // Verifies that adding two of the same non-stackable items takes up two separate slots
    [Test]
    public void Test04_AddItem_NonStackableDuplicate_OccupiesSeparateSlot()
    {
        ItemScript item = CreateItemWithPrice(0, "Sword");
        inventoryManager.AddItem(item);
        inventoryManager.AddItem(item);

        Assert.AreEqual(item, inventoryManager.slots[0].item);
        Assert.AreEqual(item, inventoryManager.slots[1].item);
    }

    // Verifies that adding an item when all slots are filled returns false
    [Test]
    public void Test05_AddItem_AtCapacity_ReturnsFalse()
    {
        for (int i = 0; i < 9; i++)
        {
            inventoryManager.AddItem(CreateItemWithPrice(0, "Filler" + i));
        }

        bool result = inventoryManager.AddItem(CreateItemWithPrice(0, "Overflow"));
        Assert.IsFalse(result);
    }

    // Verifies that the inventoryChanged event is triggered when an item is added
    [Test]
    public void Test06_AddItem_ValidItem_FiresInventoryChangedEvent()
    {
        bool eventFired = false;
        inventoryManager.inventoryChanged += () => eventFired = true;

        inventoryManager.AddItem(CreateItemWithPrice(0));
        Assert.IsTrue(eventFired);
    }

    // Verifies that a stackable item is added to the first empty slot when not already present
    [Test]
    public void Test07_AddItem_Stackable_WhenNotPresent_AddsToFirstEmptySlot()
    {
        StackableItem item = CreateStackableWithPrice(0, "Potion");
        inventoryManager.AddItem(item);

        Assert.AreEqual(item, inventoryManager.slots[0].item);
        Assert.AreEqual(1, inventoryManager.slots[0].quantity);
    }

    // Verifies that adding a stackable item that already exists increases the quantity instead of using a new slot
    [Test]
    public void Test08_AddItem_Stackable_WhenAlreadyPresent_IncreasesQuantity()
    {
        StackableItem item = CreateStackableWithPrice(0, "Potion");
        inventoryManager.AddItem(item);
        inventoryManager.AddItem(item);

        Assert.AreEqual(2, inventoryManager.slots[0].quantity);
    }

    // Verifies that stacking an item does not incorrectly fill the next available slot
    [Test]
    public void Test09_AddItem_Stackable_DoesNotOccupyNewSlotWhenStacking()
    {
        StackableItem item = CreateStackableWithPrice(0, "Potion");
        inventoryManager.AddItem(item);
        inventoryManager.AddItem(item);

        Assert.IsNull(inventoryManager.slots[1].item, "Slot 1 should remain empty.");
    }

    // Verifies that two different stackable items (different names) occupy separate slots
    [Test]
    public void Test10_AddItem_Stackable_DifferentNames_DoNotStack()
    {
        StackableItem redPotion = CreateStackableWithPrice(0, "RedPotion");
        StackableItem bluePotion = CreateStackableWithPrice(0, "BluePotion");

        inventoryManager.AddItem(redPotion);
        inventoryManager.AddItem(bluePotion);

        Assert.AreEqual("RedPotion", inventoryManager.slots[0].item.name);
        Assert.AreEqual("BluePotion", inventoryManager.slots[1].item.name);
    }

    // Verifies that removing a non-stackable item results in an empty slot
    [Test]
    public void Test11_RemoveItem_NonStackableItem_ClearsSlot()
    {
        ItemScript item = CreateItemWithPrice(0);
        inventoryManager.AddItem(item);
        inventoryManager.RemoveItem(item);

        Assert.IsNull(inventoryManager.slots[0].item);
        Assert.AreEqual(0, inventoryManager.slots[0].quantity);
    }

    // Verifies that calling RemoveItem for an item not currently in the inventory does not cause a crash
    [Test]
    public void Test12_RemoveItem_NotInInventory_DoesNotThrow()
    {
        ItemScript item = CreateItemWithPrice(0);
        Assert.DoesNotThrow(() => inventoryManager.RemoveItem(item));
    }

    // Verifies that the inventoryChanged event is triggered when an item is removed
    [Test]
    public void Test13_RemoveItem_FiresInventoryChangedEvent()
    {
        ItemScript item = CreateItemWithPrice(0);
        inventoryManager.AddItem(item);

        bool eventFired = false;
        inventoryManager.inventoryChanged += () => eventFired = true;

        inventoryManager.RemoveItem(item);
        Assert.IsTrue(eventFired);
    }

    // Verifies that removing one stackable item from a stack of two reduces the quantity to one
    [Test]
    public void Test14_RemoveItem_Stackable_WithMultiple_DecreasesQuantity()
    {
        StackableItem item = CreateStackableWithPrice(0, "Potion");
        inventoryManager.AddItem(item);
        inventoryManager.AddItem(item);

        inventoryManager.RemoveItem(item);
        Assert.AreEqual(1, inventoryManager.slots[0].quantity);
    }

    // Verifies that removing the last item of a stackable group clears the slot
    [Test]
    public void Test15_RemoveItem_Stackable_LastCopy_ClearsSlot()
    {
        StackableItem item = CreateStackableWithPrice(0, "Potion");
        inventoryManager.AddItem(item);
        inventoryManager.RemoveItem(item);

        Assert.IsNull(inventoryManager.slots[0].item);
    }

    // ── GoldManager Tests ─────────────────────────────────────────────────

    // Verifies that the player starts with exactly 10 gold as the default value
    [Test]
    public void Test16_GoldManager_PlayerGold_InitialValue_IsTen()
    {
        Assert.AreEqual(10, goldManager.PlayerGold);
    }

    // Verifies that BuyItem returns true when the player has enough gold
    [Test]
    public void Test17_BuyItem_WhenAffordable_ReturnsTrue()
    {
        ItemScript item = CreateItemWithPrice(5);
        bool result = goldManager.BuyItem(item);
        Assert.IsTrue(result);
    }

    // Verifies that the player's gold is reduced by the correct amount after a purchase
    [Test]
    public void Test18_BuyItem_WhenAffordable_DeductsCorrectAmount()
    {
        ItemScript item = CreateItemWithPrice(5);
        goldManager.BuyItem(item);
        Assert.AreEqual(5, goldManager.PlayerGold);
    }

    // Verifies that a purchase succeeds when the item price is exactly equal to the player's gold
    [Test]
    public void Test19_BuyItem_ExactGoldAmount_Succeeds()
    {
        ItemScript item = CreateItemWithPrice(10);
        bool result = goldManager.BuyItem(item);
        Assert.IsTrue(result);
        Assert.AreEqual(0, goldManager.PlayerGold);
    }

    // Verifies that BuyItem returns false if the player's gold is less than the item price
    [Test]
    public void Test20_BuyItem_WhenNotAffordable_ReturnsFalse()
    {
        ItemScript item = CreateItemWithPrice(100);
        bool result = goldManager.BuyItem(item);
        Assert.IsFalse(result);
    }

    // Verifies that no gold is deducted if the player cannot afford the item
    [Test]
    public void Test21_BuyItem_WhenNotAffordable_GoldUnchanged()
    {
        ItemScript item = CreateItemWithPrice(100);
        goldManager.BuyItem(item);
        Assert.AreEqual(10, goldManager.PlayerGold);
    }

    // Verifies that BuyItem returns false if the inventory is already full, regardless of gold
    [Test]
    public void Test22_BuyItem_WhenInventoryFull_ReturnsFalse()
    {
        for (int i = 0; i < 9; i++)
        {
            inventoryManager.AddItem(CreateItemWithPrice(0, "Filler" + i));
        }

        ItemScript item = CreateItemWithPrice(1);
        bool result = goldManager.BuyItem(item);
        Assert.IsFalse(result);
    }

    // Verifies that no gold is deducted if the purchase fails due to a full inventory
    [Test]
    public void Test23_BuyItem_WhenInventoryFull_GoldNotDeducted()
    {
        for (int i = 0; i < 9; i++)
        {
            inventoryManager.AddItem(CreateItemWithPrice(0, "Filler" + i));
        }

        ItemScript item = CreateItemWithPrice(1);
        goldManager.BuyItem(item);
        Assert.AreEqual(10, goldManager.PlayerGold);
    }

    // Verifies that buying multiple stackable items correctly handles both gold deduction and inventory stacking
    [Test]
    public void Test24_BuyItem_StackableItem_MultipleTimes_StacksCorrectly()
    {
        StackableItem item = CreateStackableWithPrice(1, "Bait");
        goldManager.BuyItem(item);
        goldManager.BuyItem(item);

        Assert.AreEqual(2, inventoryManager.slots[0].quantity);
        Assert.AreEqual(8, goldManager.PlayerGold);
    }

    // Verifies that selling an item adds the item's price back to the player's gold total
    [Test]
    public void Test25_SellItem_IncreasesGoldByItemPrice()
    {
        ItemScript item = CreateItemWithPrice(7);
        inventoryManager.AddItem(item);

        goldManager.SellItem(item);
        Assert.AreEqual(17, goldManager.PlayerGold);
    }

    // Verifies that selling an item removes it from the inventory manager
    [Test]
    public void Test26_SellItem_RemovesItemFromInventory()
    {
        ItemScript item = CreateItemWithPrice(5);
        inventoryManager.AddItem(item);
        goldManager.SellItem(item);

        Assert.IsNull(inventoryManager.slots[0].item);
    }

    // Verifies that the AddGold method correctly increments the player's total gold
    [Test]
    public void Test27_AddGold_IncreasesPlayerGoldByAmount()
    {
        goldManager.AddGold(20);
        Assert.AreEqual(30, goldManager.PlayerGold);
    }

    // Verifies that the base ItemScript class returns false for CanStack by default
    [Test]
    public void Test28_ItemScript_CanStack_ReturnsFalseByDefault()
    {
        ItemScript item = CreateItemWithPrice(0);
        Assert.IsFalse(item.CanStack(), "Base ItemScript should return false for CanStack.");
    }

    // Verifies that stackable items find and merge with existing stacks even if lower-index slots are empty
    [Test]
    public void Test29_AddItem_Stackable_MergesWithExistingStackInNonZeroSlot()
    {
        StackableItem item = CreateStackableWithPrice(0, "Arrow");

        // Leave slot 0 empty, put Arrow in slot 1
        inventoryManager.slots[1].item = item;
        inventoryManager.slots[1].quantity = 1;

        // Add another Arrow
        inventoryManager.AddItem(item);

        Assert.AreEqual(2, inventoryManager.slots[1].quantity, "Item should have merged into slot 1.");
        Assert.IsNull(inventoryManager.slots[0].item, "Slot 0 should have remained empty.");
    }

    // Verifies that buying an item with a price of zero succeeds and does not change the gold balance
    [Test]
    public void Test30_BuyItem_ZeroPrice_SucceedsWithoutDeductingGold()
    {
        ItemScript freeItem = CreateItemWithPrice(0, "FreeBread");
        int initialGold = goldManager.PlayerGold;

        bool result = goldManager.BuyItem(freeItem);

        Assert.IsTrue(result);
        Assert.AreEqual(initialGold, goldManager.PlayerGold, "Gold should not change for a free item.");
        Assert.AreEqual(freeItem, inventoryManager.slots[0].item);
    }
}