using NUnit.Framework;
using UnityEngine;

//Boundary Test 1 - Tests inventory to see if items can be bought when full
public class InventoryTest
{
    [TestCase(8, true)]
    [TestCase(9, false)]
    [TestCase(10, false)]
    public void AddItem_AtCapacityBoundary_ReturnsExpected(int itemsToAdd, bool expectedResult)
    {
        GameObject go = new GameObject("GameManagers");
        InventoryManager inventoryManager = go.AddComponent<InventoryManager>();
        inventoryManager.Awake();

        for (int i = 0; i < itemsToAdd; i++)
        {
            ItemScript filler = ScriptableObject.CreateInstance<ItemScript>();
            inventoryManager.AddItem(filler);
        }

        ItemScript testItem = ScriptableObject.CreateInstance<ItemScript>();
        bool result = inventoryManager.AddItem(testItem);

        Assert.AreEqual(expectedResult, result);

        InventoryManager.ResetInstance();
        Object.DestroyImmediate(go);
    }

    //Second Boundary Test - Tests if a null value can be added into the inventory
    [Test]
    public void AddItem_WhenItemIsNull_DoesNotCorruptInventory()
    {
        GameObject go = new GameObject("GameManagers");
        InventoryManager inventoryManager = go.AddComponent<InventoryManager>();
        inventoryManager.Awake();

        inventoryManager.AddItem(null);

        int filled = 0;
        foreach (var slot in inventoryManager.slots)
            if (slot.item != null) filled++;

        Assert.AreEqual(0, filled, "Null item should not be added to inventory.");

        InventoryManager.ResetInstance();
        Object.DestroyImmediate(go);
    }
}