using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

//Stress test for spawning many shop items into the scene to see if it crashes
public class ItemStressTest
{
    [UnityTest]
    public IEnumerator Shop_StressTest_SpawnsMultipleShopItemButtons()
    {
        int itemsToSpawn = 100000;  //this amount will freeze and crash unity
        GameObject[] shopButtons = new GameObject[itemsToSpawn];

        for (int i = 0; i < itemsToSpawn; i++)
        {
            shopButtons[i] = new GameObject("ShopItem_" + i);
            shopButtons[i].AddComponent<Button>();
            shopButtons[i].AddComponent<Image>();
            ShopItemButton button = shopButtons[i].AddComponent<ShopItemButton>();

            ItemScript item = ScriptableObject.CreateInstance<ItemScript>();
            item.itemName = "ShopItem_" + i;
            item.itemDescription = "Description for item " + i;
            item.price = i;

            button.itemData = item;
        }

        yield return null;

        int spawnedCount = 0;
        foreach (var button in shopButtons)
            if (button != null) spawnedCount++;

        Assert.AreEqual(itemsToSpawn, spawnedCount,
            "All shop item buttons should exist in the hierarchy.");

        foreach (var button in shopButtons)
            Object.Destroy(button);
    }
}