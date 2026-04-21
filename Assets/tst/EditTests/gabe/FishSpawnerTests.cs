using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class FishSpawnerTests
{
    private GameObject spawnerObject;
    private TestFishSpawner spawner;
    private GameObject fishPrefab;

    [SetUp]
    public void SetUp()
    {
        // Create spawner object
        spawnerObject = new GameObject("FishSpawner");
        spawner = spawnerObject.AddComponent<TestFishSpawner>();

        // Create fish prefab with FishMovement component
        // This simulates a real fish prefab used in the game
        fishPrefab = new GameObject("FishPrefab");
        fishPrefab.AddComponent<FishMovement>();

        // Set default spawn bounds using reflection
        // because bounds is stored in a private data class
        SetSpawnerBounds(spawner, -8f, 8f, -4f, 4f);
    }

    [TearDown]
    public void TearDown()
    {
        // Remove all FishMovement objects created during tests
        FishMovement[] allFish =
            Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

        foreach (FishMovement fish in allFish)
        {
            if (fish != null)
            {
                Object.DestroyImmediate(fish.gameObject);
            }
        }

        // Remove spawner object
        if (spawnerObject != null)
            Object.DestroyImmediate(spawnerObject);
    }

    // ------------------------------------------------------------
    // Test 1
    // Ensures xpawn fish creates the correct number of fish objects.
    // Confirms that the loop runs the expected number of times
    // and insatntate is being called correctly.
    // Expected result:
    // 5 fish created + 1 original prefab object = 6 total objects.
    // ------------------------------------------------------------
    [Test]
    public void SpawnFish_SpawnsCorrectNumberOfFish()
    {
        spawner.fishPrefabs = new List<GameObject> { fishPrefab };
        spawner.numberToSpawn = 5;

        spawner.CallSpawnFish();

        FishMovement[] spawnedFish =
            Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

        Assert.AreEqual(6, spawnedFish.Length);
    }

    // ------------------------------------------------------------
    // Test 2
    // Verifies that all spawned fish appear within the defined bounds.
    // Confirms GetSpawnPosition() correctly generates positions
    // inside minX, maxX, minY, maxY.
    // Prevents fish from spawning outside the playable area.
    // ------------------------------------------------------------
    [Test]
    public void SpawnFish_SpawnedFishAreWithinBounds()
    {
        spawner.fishPrefabs = new List<GameObject> { fishPrefab };
        spawner.numberToSpawn = 10;

        SetSpawnerBounds(spawner, -8f, 8f, -4f, 4f);

        spawner.CallSpawnFish();

        FishMovement[] spawnedFish =
            Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

        foreach (FishMovement fish in spawnedFish)
        {
            // ignore the original prefab object
            if (fish.gameObject == fishPrefab)
                continue;

            Vector3 pos = fish.transform.position;

            Assert.GreaterOrEqual(pos.x, -8f);
            Assert.LessOrEqual(pos.x, 8f);
            Assert.GreaterOrEqual(pos.y, -4f);
            Assert.LessOrEqual(pos.y, 4f);
        }
    }

    // ------------------------------------------------------------
    // Test 3
    // Ensures ConfigureFish() correctly transfers spawn bounds
    // to the FishMovement component on each spawned fish.
    // Confirms that each fish receives the correct movement limits.
    // Prevents bugs where fish move outside their allowed area.
    // ------------------------------------------------------------
    [Test]
    public void ConfigureFish_SetsFishMovementBoundsCorrectly()
    {
        spawner.fishPrefabs = new List<GameObject> { fishPrefab };
        spawner.numberToSpawn = 1;

        SetSpawnerBounds(spawner, -10f, 10f, -3f, 3f);

        spawner.CallSpawnFish();

        FishMovement[] spawnedFish =
            Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

        FishMovement spawned = null;

        foreach (FishMovement fish in spawnedFish)
        {
            if (fish.gameObject != fishPrefab)
            {
                spawned = fish;
                break;
            }
        }

        Assert.IsNotNull(spawned);

        Assert.AreEqual(-10f, spawned.minX);
        Assert.AreEqual(10f, spawned.maxX);
        Assert.AreEqual(-3f, spawned.minY);
        Assert.AreEqual(3f, spawned.maxY);
    }

    // ------------------------------------------------------------
    // Test 4
    // Ensures that if no prefabs are assigned,
    // SpawnFish() does not create any fish.
    // Confirms the safety check prevents errors
    // when fishPrefabs list is empty.
    // ------------------------------------------------------------
    [Test]
    public void SpawnFish_WithEmptyPrefabList_SpawnsNothing()
    {
        spawner.fishPrefabs = new List<GameObject>();
        spawner.numberToSpawn = 5;

        spawner.CallSpawnFish();

        FishMovement[] spawnedFish =
            Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

        Assert.AreEqual(1, spawnedFish.Length);
    }

    // ------------------------------------------------------------
    // Test 5
    // Ensures that if a null prefab exists in the list,
    // SpawnFish() safely skips that spawn attempt.
    // Confirms null checking prevents runtime errors.
    // ------------------------------------------------------------
    [Test]
    public void SpawnFish_WithNullPrefab_SkipsThatSpawn()
    {
        spawner.fishPrefabs = new List<GameObject> { null };
        spawner.numberToSpawn = 3;

        spawner.CallSpawnFish();

        FishMovement[] spawnedFish =
            Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

        Assert.AreEqual(1, spawnedFish.Length);
    }

    // ------------------------------------------------------------
    // Test 6
    // Verifies that when numberToSpawn = 0,
    // SpawnFish() does not create any objects.
    // Confirms the loop correctly handles zero iterations.
    // ------------------------------------------------------------
    [Test]
    public void SpawnFish_WithZeroNumberToSpawn_SpawnsNothing()
    {
        spawner.fishPrefabs = new List<GameObject> { fishPrefab };
        spawner.numberToSpawn = 0;

        spawner.CallSpawnFish();

        FishMovement[] spawnedFish =
            Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

        Assert.AreEqual(1, spawnedFish.Length);
    }

    // ------------------------------------------------------------
    // Test 7 (Stress Test)
    // This test repeatedly spawns a very large number of fish
    // to simulate heavy load on the game.
    //
    // Checks that FishSpawner can handle many objects
    // Helps identify performance limits
    // Ensures no crashes occur under stress
    // Can reveal memory or spawning inefficiencies
    //
    // NOTE:
    // Unity Test Runner does not directly expose FPS values,
    // If the editor slows down significantly, this indicates
    // the approximate limit where performance begins to drop.
    //
    // ------------------------------------------------------------
    [Test]
    public void StressTest_SpawnManyFish_UntilPerformanceDrops()
        {
            spawner.fishPrefabs = new List<GameObject> { fishPrefab };

            // very large spawn count to simulate heavy load
            spawner.numberToSpawn = 2000;

            // spawn fish multiple times
            for (int i = 0; i < 5; i++)
                {
                spawner.CallSpawnFish();
            }

            FishMovement[] spawnedFish =
            Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

            Debug.Log("Stress Test Fish Count: " + spawnedFish.Length);
        // ensure fish were actually created
    Assert.Greater(spawnedFish.Length, 0);
}

    // ------------------------------------------------------------
    // Helper Method
    // Uses reflection to modify the private spawn bounds data class.
    // Allow tests to control spawn boundaries even though
    // bounds is stored as a private field.
    // ------------------------------------------------------------
    private void SetSpawnerBounds(
        FishSpawner spawnerInstance,
        float minX,
        float maxX,
        float minY,
        float maxY)
    {
        FieldInfo boundsField =
            typeof(FishSpawner).GetField(
                "bounds",
                BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(boundsField,
            "Could not find private field 'bounds'.");

        object boundsObject = boundsField.GetValue(spawnerInstance);

        System.Type boundsType = boundsObject.GetType();

        FieldInfo minXField =
            boundsType.GetField("minX");

        FieldInfo maxXField =
            boundsType.GetField("maxX");

        FieldInfo minYField =
            boundsType.GetField("minY");

        FieldInfo maxYField =
            boundsType.GetField("maxY");

        minXField.SetValue(boundsObject, minX);
        maxXField.SetValue(boundsObject, maxX);
        minYField.SetValue(boundsObject, minY);
        maxYField.SetValue(boundsObject, maxY);
    }
}