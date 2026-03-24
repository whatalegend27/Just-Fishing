using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class FishDatabaseTests
{
    private FishDatabaseManager db;

    [SetUp]
    public void Setup()
    {
        GameObject obj = new GameObject();
        db = obj.AddComponent<FishDatabaseManager>();

        db.fishDatabase = new List<FishData>
        {
            new FishData { fishName = "Salmon", fishKnown = false },
            new FishData { fishName = "Cod", fishKnown = false },
            new FishData { fishName = "Trout", fishKnown = false }
        };
    }

    [Test]
    // Boundary Test #1
    public void CatchFish_Boundary_FirstCatch_UpdatesFishKnown()
    {
        Assert.IsFalse(db.fishDatabase[0].fishKnown);

        db.RegisterFish("Salmon");

        Assert.IsTrue(db.fishDatabase[0].fishKnown);
    }

    [Test]
    // Boundary Test #2
    public void Only_Target_Fish_Is_Updated()
    {
        db.RegisterFish("Cod");

        Assert.IsFalse(db.fishDatabase[0].fishKnown);
        Assert.IsTrue(db.fishDatabase[1].fishKnown);
        Assert.IsFalse(db.fishDatabase[2].fishKnown);
    }

    [Test]
    // Stress Test #1
    public void RegisterFish_Stress_Test_MultipleCalls()
    {
        for (int i = 0; i < 1000; i++)
        {
            db.RegisterFish("Trout");
        }

        Assert.IsTrue(db.fishDatabase[2].fishKnown);
    }
}