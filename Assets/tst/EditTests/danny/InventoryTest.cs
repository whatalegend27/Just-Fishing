using NUnit.Framework;
using UnityEngine;

//Boundary Test 1
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
    
}