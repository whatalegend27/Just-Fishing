using System.Collections.Generic;
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

        // Create a simple fish prefab
        fishPrefab = new GameObject("FishPrefab");
        fishPrefab.AddComponent<FishMovement>();

        // Default bounds
        spawner.minX = -8f;
        spawner.maxX = 8f;
        spawner.minY = -4f;
        spawner.maxY = 4f;
    }

    [TearDown]
    public void TearDown()
    {
        // Destroy all spawned fish
        foreach (var fish in Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None))
        {
            if (fish != null && fish.gameObject != fishPrefab)
            {
                Object.DestroyImmediate(fish.gameObject);
            }
        }

        // Destroy test objects
        if (spawnerObject != null)
            Object.DestroyImmediate(spawnerObject);

        if (fishPrefab != null)
            Object.DestroyImmediate(fishPrefab);
    }

    [Test]
    public void SpawnFish_SpawnsCorrectNumberOfFish()
    {
        spawner.fishPrefabs = new List<GameObject> { fishPrefab };
        spawner.numberToSpawn = 5;

        spawner.CallSpawnFish();

        FishMovement[] spawnedFish = Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

        // subtract 1 because prefab itself also has FishMovement
        Assert.AreEqual(6, spawnedFish.Length);
    }

    [Test]
    public void SpawnFish_SpawnedFishAreWithinBounds()
    {
        spawner.fishPrefabs = new List<GameObject> { fishPrefab };
        spawner.numberToSpawn = 10;

        spawner.CallSpawnFish();

        FishMovement[] spawnedFish = Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

        foreach (FishMovement fish in spawnedFish)
        {
            if (fish.gameObject == fishPrefab)
                continue;

            Vector3 pos = fish.transform.position;

            Assert.GreaterOrEqual(pos.x, spawner.minX);
            Assert.LessOrEqual(pos.x, spawner.maxX);
            Assert.GreaterOrEqual(pos.y, spawner.minY);
            Assert.LessOrEqual(pos.y, spawner.maxY);
        }
    }

    [Test]
    public void ConfigureFish_SetsFishMovementBoundsCorrectly()
    {
        spawner.fishPrefabs = new List<GameObject> { fishPrefab };
        spawner.numberToSpawn = 1;

        spawner.minX = -10f;
        spawner.maxX = 10f;
        spawner.minY = -3f;
        spawner.maxY = 3f;

        spawner.CallSpawnFish();

        FishMovement[] spawnedFish = Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

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

    [Test]
    public void SpawnFish_WithEmptyPrefabList_SpawnsNothing()
    {
        spawner.fishPrefabs = new List<GameObject>();
        spawner.numberToSpawn = 5;

        spawner.CallSpawnFish();

        FishMovement[] spawnedFish = Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

        // only the prefab exists in scene setup if it was created, but not spawned
        Assert.AreEqual(1, spawnedFish.Length);
    }

    [Test]
    public void SpawnFish_WithNullPrefab_SkipsThatSpawn()
    {
        spawner.fishPrefabs = new List<GameObject> { null };
        spawner.numberToSpawn = 3;

        spawner.CallSpawnFish();

        FishMovement[] spawnedFish = Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

        // only the prefab object created in setup exists
        Assert.AreEqual(1, spawnedFish.Length);
    }

    [Test]
    public void SpawnFish_WithZeroNumberToSpawn_SpawnsNothing()
    {
        spawner.fishPrefabs = new List<GameObject> { fishPrefab };
        spawner.numberToSpawn = 0;

        spawner.CallSpawnFish();

        FishMovement[] spawnedFish = Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

        // only the prefab object exists, no spawned copies
        Assert.AreEqual(1, spawnedFish.Length);
    }
}