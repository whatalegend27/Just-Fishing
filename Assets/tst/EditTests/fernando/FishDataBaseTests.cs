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
            new FishData { fishName = "Cod",    fishKnown = false },
            new FishData { fishName = "Trout",  fishKnown = false }
        };
    }

    [TearDown]
    public void TearDown()
    {
        // Reset the singleton so each test starts from a clean database manager.
        FishDatabaseManager.ResetInstance();

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

    [Test]
    // Boundary Test #3
    public void RegisterFish_Boundary_CatchCountIncrementsOnFirstCatch()
    {
        // catchCount should start at 0 and increment to 1 after first registration.
        Assert.AreEqual(0, db.fishDatabase[0].catchCount);

        db.RegisterFish("Salmon");

        Assert.AreEqual(1, db.fishDatabase[0].catchCount);
    }

    [Test]
    // Boundary Test #4
    public void RegisterFish_Boundary_SameFishTwice_CatchCountIsTwo()
    {
        // Registering the same fish twice should increment catchCount to 2.
        db.RegisterFish("Salmon");
        db.RegisterFish("Salmon");

        Assert.AreEqual(2, db.fishDatabase[0].catchCount);
    }

    [Test]
    // Boundary Test #5
    public void RegisterFish_Boundary_EmptyString_ReturnsFalse()
    {
        // An empty string should not match any fish in the database.
        bool result = db.RegisterFish("");

        Assert.IsFalse(result);
    }

    [Test]
    // Boundary Test #6
    public void RegisterFish_FiresEvent_WhenFishFound()
    {
        // OnFishRegistered should fire with the correct fish name when a valid fish is registered.
        string receivedName = null;
        FishDatabaseManager.OnFishRegistered += name => receivedName = name;

        db.RegisterFish("Cod");

        FishDatabaseManager.OnFishRegistered -= name => receivedName = name;

        Assert.AreEqual("Cod", receivedName);
    }

    [Test]
    // Boundary Test #7
    public void RegisterFish_DoesNotFireEvent_WhenFishNotFound()
    {
        // OnFishRegistered should NOT fire when the fish name is not in the database.
        bool eventFired = false;
        FishDatabaseManager.OnFishRegistered += _ => eventFired = true;

        db.RegisterFish("Shark");

        FishDatabaseManager.OnFishRegistered -= _ => eventFired = true;

        Assert.IsFalse(eventFired);
    }

    [Test]
    // Boundary Test #8
    public void RegisterFish_AllFishCaught_TriggersGameComplete()
    {
        // Catching every fish in the database should fire the OnAllFishCaught event.
        bool gameCompleteFired = false;
        FishDatabaseManager.OnAllFishCaught += () => gameCompleteFired = true;

        foreach (FishData fish in db.fishDatabase)
            db.RegisterFish(fish.fishName);

        FishDatabaseManager.OnAllFishCaught -= () => gameCompleteFired = true;

        Assert.IsTrue(gameCompleteFired, "OnAllFishCaught should fire when all fish are registered");
    }

    [Test]
    // Boundary Test #9
    public void ResetInstance_ClearsSingleton()
    {
        // After ResetInstance, the singleton should be null.
        FishDatabaseManager.ResetInstance();

        Assert.IsNull(FishDatabaseManager.Instance);
    }

    [Test]
    // Boundary Test #11
    public void InitialState_AllFish_AreUnknownWithZeroCatchCount()
    {
        // Before any registration, every fish should be unknown with a catchCount of 0.
        foreach (FishData fish in db.fishDatabase)
        {
            Assert.IsFalse(fish.fishKnown, $"{fish.fishName} should start unknown");
            Assert.AreEqual(0, fish.catchCount, $"{fish.fishName} should start with catchCount 0");
        }
    }

    [Test]
    // Boundary Test #12
    public void RegisterFish_Isolation_OnlyTargetFishIsUpdated()
    {
        // Registering one fish should not affect the others.
        db.RegisterFish("Salmon");

        Assert.IsTrue(db.fishDatabase[0].fishKnown);
        Assert.IsFalse(db.fishDatabase[1].fishKnown, "Cod should remain unknown");
        Assert.IsFalse(db.fishDatabase[2].fishKnown, "Trout should remain unknown");
        Assert.AreEqual(0, db.fishDatabase[1].catchCount, "Cod catchCount should stay 0");
        Assert.AreEqual(0, db.fishDatabase[2].catchCount, "Trout catchCount should stay 0");
    }

    [Test]
    // Boundary Test #13
    public void RegisterFish_CaseSensitive_LowercaseReturnsFalse()
    {
        // The lookup is case-sensitive; "salmon" should not match "Salmon".
        bool result = db.RegisterFish("salmon");

        Assert.IsFalse(result);
        Assert.IsFalse(db.fishDatabase[0].fishKnown);
    }

    [Test]
    // Boundary Test #14
    public void RegisterFish_NullInput_ReturnsFalse()
    {
        // Passing null should return false without throwing an exception.
        bool result = false;
        Assert.DoesNotThrow(() => result = db.RegisterFish(null));
        Assert.IsFalse(result);
    }

    [Test]
    // Boundary Test #15
    public void RegisterFish_AllFish_BecomeKnown()
    {
        // Registering every fish in the database should mark all of them as known.
        foreach (FishData fish in db.fishDatabase)
            db.RegisterFish(fish.fishName);

        foreach (FishData fish in db.fishDatabase)
            Assert.IsTrue(fish.fishKnown, $"{fish.fishName} should be known after registration");
    }

    [Test]
    // Boundary Test #16
    public void RegisterFish_FailedLookup_CatchCountsRemainZero()
    {
        // A failed registration should leave all catchCounts unchanged at 0.
        db.RegisterFish("Shark");

        foreach (FishData fish in db.fishDatabase)
            Assert.AreEqual(0, fish.catchCount, $"{fish.fishName} catchCount should remain 0");
    }

    [Test]
    // Boundary Test #17
    public void RegisterFish_KnownFish_CatchCountIsAtLeastOne()
    {
        // If a fish is known, its catchCount must be >= 1.
        db.RegisterFish("Trout");

        Assert.IsTrue(db.fishDatabase[2].fishKnown);
        Assert.GreaterOrEqual(db.fishDatabase[2].catchCount, 1);
    }
}
