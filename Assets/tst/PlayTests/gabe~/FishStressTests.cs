/* using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class FishStressTests
{
    private GameObject fishPrefab;
    private GameObject spawnerObject;
    private FishSpawner spawner;

    private const int StressFishCount = 200;

    [SetUp]
    public void Setup()
    {
        fishPrefab = new GameObject("FishPrefab");
        fishPrefab.AddComponent<SpriteRenderer>();
        fishPrefab.AddComponent<FishMovement>();

        spawnerObject = new GameObject("FishSpawnerTest");
        spawner = spawnerObject.AddComponent<FishSpawner>();

        spawner.fishPrefab = fishPrefab;
        spawner.numberToSpawn = StressFishCount;

        spawner.minX = -8f;
        spawner.maxX = 8f;
        spawner.minY = -4f;
        spawner.maxY = 4f;
    }

    [TearDown]
    public void Cleanup()
    {
        foreach (FishMovement fish in Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None))
            Object.DestroyImmediate(fish.gameObject);

        Object.DestroyImmediate(spawnerObject);
        Object.DestroyImmediate(fishPrefab);
    }

    [UnityTest]
    public IEnumerator SpawnManyFishSuccessfully()
    {
        yield return null;
        yield return null;

        FishMovement[] fishList =
            Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

        Assert.AreEqual(StressFishCount, fishList.Length);

        foreach (FishMovement fish in fishList)
        {
            Vector3 pos = fish.transform.position;

            Assert.GreaterOrEqual(pos.x, -8f);
            Assert.LessOrEqual(pos.x, 8f);
            Assert.GreaterOrEqual(pos.y, -4f);
            Assert.LessOrEqual(pos.y, 4f);
        }
    }
}
*/
