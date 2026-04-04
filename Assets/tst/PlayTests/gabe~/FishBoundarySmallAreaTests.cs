/* using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

<<<<<<< HEAD
public class FishBoundarySmallAreaTests
{
    private GameObject fishPrefab;
    private GameObject spawnerObject;
    private FishSpawner spawner;

    private const int FishCount = 5;

    [SetUp]
    public void Setup()
    {
        fishPrefab = new GameObject("FishPrefab");
        fishPrefab.AddComponent<SpriteRenderer>();
        fishPrefab.AddComponent<FishMovement>();

        spawnerObject = new GameObject("FishSpawnerTest");
        spawner = spawnerObject.AddComponent<FishSpawner>();

        spawner.fishPrefab = fishPrefab;
        spawner.numberToSpawn = FishCount;

        spawner.minX = 0f;
        spawner.maxX = 0.5f;
        spawner.minY = 0f;
        spawner.maxY = 0.5f;
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
    public IEnumerator FishStayInsideSmallBounds()
    {
        yield return null;
        yield return null;

        FishMovement[] fishList =
            Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

        Assert.AreEqual(FishCount, fishList.Length);

        foreach (FishMovement fish in fishList)
        {
            Vector3 pos = fish.transform.position;
=======
// public class FishBoundarySmallAreaTests
// {
//     private GameObject fishPrefab;
//     private GameObject spawnerObject;
//     private FishSpawner spawner;

//     private const int FishCount = 5;

//     [SetUp]
//     public void Setup()
//     {
//         fishPrefab = new GameObject("FishPrefab");
//         fishPrefab.AddComponent<SpriteRenderer>();
//         fishPrefab.AddComponent<FishMovement>();

//         spawnerObject = new GameObject("FishSpawnerTest");
//         spawner = spawnerObject.AddComponent<FishSpawner>();

//         spawner.fishPrefab = fishPrefab;
//         spawner.numberToSpawn = FishCount;

//         spawner.minX = 0f;
//         spawner.maxX = 0.5f;
//         spawner.minY = 0f;
//         spawner.maxY = 0.5f;
//     }

//     [TearDown]
//     public void Cleanup()
//     {
//         foreach (FishMovement fish in Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None))
//             Object.DestroyImmediate(fish.gameObject);

//         Object.DestroyImmediate(spawnerObject);
//         Object.DestroyImmediate(fishPrefab);
//     }

//     [UnityTest]
//     public IEnumerator FishStayInsideSmallBounds()
//     {
//         yield return null;
//         yield return null;

//         FishMovement[] fishList =
//             Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

//         Assert.AreEqual(FishCount, fishList.Length);

//         foreach (FishMovement fish in fishList)
//         {
//             Vector3 pos = fish.transform.position;
>>>>>>> 7261be63e579a9efe042cb09a404bedea04e94a0

            Assert.GreaterOrEqual(pos.x, 0f);
            Assert.LessOrEqual(pos.x, 0.5f);
            Assert.GreaterOrEqual(pos.y, 0f);
            Assert.LessOrEqual(pos.y, 0.5f);
        }
    }
}
*/