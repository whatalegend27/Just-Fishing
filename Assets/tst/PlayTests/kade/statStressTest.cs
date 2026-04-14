using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;

public class riskStressTest
{
    private const int INSTANCE_COUNT = 100;
    private List<GameObject> testObjects = new List<GameObject>();

    [UnityTest]
    public IEnumerator StressTest_MassStatUpdates()
    {
        for (int i = 0; i < INSTANCE_COUNT; i++)
        {
            GameObject obj = new GameObject($"StressObject_{i}");
            PlayerStats ps = obj.AddComponent<PlayerStats>();
            obj.AddComponent<ArrestStats>().ps = ps;
            testObjects.Add(obj);
        }

        float elapsed = 0f;
        while (elapsed < 5f)
        {
            foreach (GameObject obj in testObjects)
            {
                if (obj == null) continue;
                obj.GetComponent<ArrestStats>().calculateRisk("nightFish");
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        foreach (GameObject obj in testObjects)
            Object.Destroy(obj);

        Assert.Pass($"Stress test completed with {INSTANCE_COUNT} instances");
    }
}
