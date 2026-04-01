/*using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

// public class FishBoundaryEdgeTests
// {
//     private GameObject fishPrefab;
//     private GameObject spawnerObject;
//     private FishSpawner spawner;

//     [SetUp]
//     public void Setup()
//     {
//         fishPrefab = new GameObject("FishPrefab");
//         fishPrefab.AddComponent<SpriteRenderer>();
//         fishPrefab.AddComponent<FishMovement>();

//         spawnerObject = new GameObject("FishSpawnerTest");
//         spawner = spawnerObject.AddComponent<FishSpawner>();

//         spawner.fishPrefab = fishPrefab;
//         spawner.numberToSpawn = 1;

//         spawner.minX = -8f;
//         spawner.maxX = -7.9f;
//         spawner.minY = -4f;
//         spawner.maxY = -3.9f;
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
//     public IEnumerator FishRemainInsideEdgeBounds()
//     {
//         yield return null;
//         yield return null;

//         FishMovement[] fishList =
//             Object.FindObjectsByType<FishMovement>(FindObjectsSortMode.None);

//         Assert.AreEqual(1, fishList.Length);

//         yield return new WaitForSeconds(2f);

//         Vector3 pos = fishList[0].transform.position;

        Assert.GreaterOrEqual(pos.x, -8f);
        Assert.LessOrEqual(pos.x, -7.9f);
        Assert.GreaterOrEqual(pos.y, -4f);
        Assert.LessOrEqual(pos.y, -3.9f);
    }
}
*/
