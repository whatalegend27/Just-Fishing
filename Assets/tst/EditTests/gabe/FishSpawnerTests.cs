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

        // Create fish prefab
        fishPrefab = new GameObject("FishPrefab");
        fishPrefab.AddComponent<FishMovement>();

        // Set default bounds through reflection
        SetSpawnerBounds(spawner, -8f, 8f, -4f, 4f);
    }

    [TearDown]
    public void TearDown()
    {
        FishMovement[] allFish = Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

        foreach (FishMovement fish in allFish)
        {
            if (fish != null)
            {
                Object.DestroyImmediate(fish.gameObject);
            }
        }

        if (spawnerObject != null)
            Object.DestroyImmediate(spawnerObject);
    }

    [Test]
    public void SpawnFish_SpawnsCorrectNumberOfFish()
    {
        spawner.fishPrefabs = new List<GameObject> { fishPrefab };
        spawner.numberToSpawn = 5;

        spawner.CallSpawnFish();

        FishMovement[] spawnedFish = Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

        // 1 prefab + 5 spawned copies = 6
        Assert.AreEqual(6, spawnedFish.Length);
    }

    [Test]
    public void SpawnFish_SpawnedFishAreWithinBounds()
    {
        spawner.fishPrefabs = new List<GameObject> { fishPrefab };
        spawner.numberToSpawn = 10;

        SetSpawnerBounds(spawner, -8f, 8f, -4f, 4f);

        spawner.CallSpawnFish();

        FishMovement[] spawnedFish = Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

        foreach (FishMovement fish in spawnedFish)
        {
            if (fish.gameObject == fishPrefab)
                continue;

            Vector3 pos = fish.transform.position;

            Assert.GreaterOrEqual(pos.x, -8f);
            Assert.LessOrEqual(pos.x, 8f);
            Assert.GreaterOrEqual(pos.y, -4f);
            Assert.LessOrEqual(pos.y, 4f);
        }
    }

    [Test]
    public void ConfigureFish_SetsFishMovementBoundsCorrectly()
    {
        spawner.fishPrefabs = new List<GameObject> { fishPrefab };
        spawner.numberToSpawn = 1;

        SetSpawnerBounds(spawner, -10f, 10f, -3f, 3f);

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

        // only the prefab from setup exists
        Assert.AreEqual(1, spawnedFish.Length);
    }

    [Test]
    public void SpawnFish_WithNullPrefab_SkipsThatSpawn()
    {
        spawner.fishPrefabs = new List<GameObject> { null };
        spawner.numberToSpawn = 3;

        spawner.CallSpawnFish();

        FishMovement[] spawnedFish = Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

        // only the prefab from setup exists
        Assert.AreEqual(1, spawnedFish.Length);
    }

    [Test]
    public void SpawnFish_WithZeroNumberToSpawn_SpawnsNothing()
    {
        spawner.fishPrefabs = new List<GameObject> { fishPrefab };
        spawner.numberToSpawn = 0;

        spawner.CallSpawnFish();

        FishMovement[] spawnedFish = Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

        // only the prefab from setup exists
        Assert.AreEqual(1, spawnedFish.Length);
    }

    private void SetSpawnerBounds(FishSpawner spawnerInstance, float minX, float maxX, float minY, float maxY)
    {
        // Get the private field "bounds"
        FieldInfo boundsField = typeof(FishSpawner).GetField("bounds", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(boundsField, "Could not find private field 'bounds' on FishSpawner.");

        object boundsObject = boundsField.GetValue(spawnerInstance);
        Assert.IsNotNull(boundsObject, "The 'bounds' object is null.");

        // Get the private nested class type
        System.Type boundsType = boundsObject.GetType();

        // Set its fields
        FieldInfo minXField = boundsType.GetField("minX", BindingFlags.Public | BindingFlags.Instance);
        FieldInfo maxXField = boundsType.GetField("maxX", BindingFlags.Public | BindingFlags.Instance);
        FieldInfo minYField = boundsType.GetField("minY", BindingFlags.Public | BindingFlags.Instance);
        FieldInfo maxYField = boundsType.GetField("maxY", BindingFlags.Public | BindingFlags.Instance);

        Assert.IsNotNull(minXField);
        Assert.IsNotNull(maxXField);
        Assert.IsNotNull(minYField);
        Assert.IsNotNull(maxYField);

        minXField.SetValue(boundsObject, minX);
        maxXField.SetValue(boundsObject, maxX);
        minYField.SetValue(boundsObject, minY);
        maxYField.SetValue(boundsObject, maxY);
    }
}