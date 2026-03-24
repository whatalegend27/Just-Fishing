using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FishDatabaseTests
{
    private FishDatabaseManager db;
    private GameObject dbObject;

    [SetUp]
    public void Setup()
    {
        dbObject = new GameObject("FishDatabaseTestObject");
        db = dbObject.AddComponent<FishDatabaseManager>();

        db.fishDatabase = new List<FishData>
        {
            new FishData { fishName = "Salmon", fishKnown = false },
            new FishData { fishName = "Cod", fishKnown = false },
            new FishData { fishName = "Trout", fishKnown = false }
        };
    }

    [TearDown]
    public void TearDown()
    {
        // Reset the singleton so each test starts from a clean database manager.
        FishDatabaseManager.Instance = null;

        if (dbObject != null)
        {
            Object.DestroyImmediate(dbObject);
        }
    }

    [Test]
    // Boundary Test #1
    public void CatchFish_Boundary_FirstCatch_UpdatesFishKnown()
    {
        // The fish starts unknown before registration.
        Assert.IsFalse(db.fishDatabase[0].fishKnown);

        // A known fish should be registered successfully.
        bool result = db.RegisterFish("Salmon");

        Assert.IsTrue(result);
        Assert.IsTrue(db.fishDatabase[0].fishKnown);
    }

    [Test]
    // Boundary Test #2
    public void RegisterFish_Boundary_MissingFish_ReturnsFalse()
    {
        // A fish name that is not present should not be registered.
        bool result = db.RegisterFish("Shark");

        Assert.IsFalse(result);
        // No existing database entry should be changed by a failed lookup.
        Assert.IsFalse(db.fishDatabase[0].fishKnown);
        Assert.IsFalse(db.fishDatabase[1].fishKnown);
        Assert.IsFalse(db.fishDatabase[2].fishKnown);
    }
}
