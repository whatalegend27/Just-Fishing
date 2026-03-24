using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class FishDatabaseStressTests
{
    private const int StressFishCount = 100;
    private const float StressDurationSeconds = 5f;

    private FishDatabaseManager db;
    private GameObject dbObject;

    [SetUp]
    public void Setup()
    {
        // Create a fresh database manager for each stress test run.
        dbObject = new GameObject("FishDatabaseTestObject");
        db = dbObject.AddComponent<FishDatabaseManager>();
    }

    [TearDown]
    public void TearDown()
    {
        // Reset shared state so later tests do not reuse this singleton.
        FishDatabaseManager.Instance = null;

        if (dbObject != null)
        {
            Object.DestroyImmediate(dbObject);
        }
    }

    [UnityTest]
    [Category("Stress")]
    public IEnumerator RegisterFish_Stress_Test_MultipleCalls()
    {
        // Build a larger fish list than the boundary tests use.
        db.fishDatabase = new List<FishData>();
        for (int i = 0; i < StressFishCount; i++)
        {
            db.fishDatabase.Add(new FishData
            {
                fishName = $"StressFish_{i}",
                fishKnown = false
            });
        }

        float elapsed = 0f;
        int registrationCount = 0;

        // Repeatedly register every fish over multiple frames to simulate sustained load.
        while (elapsed < StressDurationSeconds)
        {
            foreach (FishData fish in db.fishDatabase)
            {
                bool result = db.RegisterFish(fish.fishName);
                Assert.IsTrue(result);
                registrationCount++;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Every fish should remain marked as known after the stress loop finishes.
        foreach (FishData fish in db.fishDatabase)
        {
            Assert.IsTrue(fish.fishKnown);
        }

        Assert.Pass($"Stress test completed with {StressFishCount} fish entries and {registrationCount} registrations.");
    }
}
