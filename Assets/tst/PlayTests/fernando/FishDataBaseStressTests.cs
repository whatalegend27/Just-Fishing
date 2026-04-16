using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.TestTools;

public class FishDatabaseStressTests
{
    private const int MinStressFishCount = 10; //@100
    private const int MaxStressFishCount = 100; //@1000 no response  
    private const int StressIncrement = 10; //1000


    private GameObject itemPrefab;
    private GameObject dbObject;
    private FishDatabaseManager db;

    [SetUp]
    public void Setup()
    {
        itemPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/src/fernando/Scripts/item.prefab");
        Assert.IsNotNull(itemPrefab, "Expected item prefab at Assets/src/fernando/Scripts/item.prefab.");

        dbObject = new GameObject("FishDatabaseTest");
        db = dbObject.AddComponent<FishDatabaseManager>();
        db.fishDatabase = new List<FishData>();
    }

    [TearDown]
    public void TearDown()
    {
        foreach (GameObject item in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (item != null && item.name == "item(Clone)")
            {
                Object.DestroyImmediate(item);
            }
        }

        FishDatabaseManager.ResetInstance();
        Object.DestroyImmediate(dbObject);
    }

    [UnityTest]
    public IEnumerator RegisterManyFishAndCreateItemsSuccessfully()
    {
        var stageResults = new StringBuilder();
        int lastSuccessfulStage = 0;
        int totalCreatedItems = 0;

        for (int fishCount = MinStressFishCount; fishCount <= MaxStressFishCount; fishCount += StressIncrement)
        {
            db.fishDatabase.Clear();
            for (int i = 0; i < fishCount; i++)
            {
                db.fishDatabase.Add(new FishData
                {
                    fishName = $"StressFish_{i}",
                    fishKnown = false
                });
            }

            int stageRegisteredCount = 0;

            foreach (FishData fish in db.fishDatabase)
            {
                if (db.RegisterFish(fish.fishName))
                {
                    Object.Instantiate(itemPrefab);
                    stageRegisteredCount++;
                    totalCreatedItems++;
                }
            }

            yield return null;
            yield return null;

            GameObject[] stageItems = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            int createdItemCount = 0;

            foreach (GameObject item in stageItems)
            {
                if (item != null && item.name == "item(Clone)")
                {
                    createdItemCount++;
                }
            }

            Assert.AreEqual(fishCount, stageRegisteredCount, $"Registration failed at stage size {fishCount}.");
            Assert.AreEqual(totalCreatedItems, createdItemCount, $"Accumulated item creation count mismatch at stage size {fishCount}.");

            stageResults.AppendLine(
                $"Stage {fishCount}: registered={stageRegisteredCount}, accumulatedItems={createdItemCount}");
            lastSuccessfulStage = fishCount;

            yield return null;
        }

        Debug.Log(
            "Fish database item stress test complete.\n" +
            $"Last successful stage: {lastSuccessfulStage}\n" +
            stageResults);
    }
}
