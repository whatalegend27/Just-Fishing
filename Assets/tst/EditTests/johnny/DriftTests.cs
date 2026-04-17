using NUnit.Framework;
using UnityEngine;

public class DriftTests
{
    [Test]
    public void boatDrift_AveragesOutToZero()
    {
        GameObject testObject = new GameObject();
        BoatController testBoat = testObject.AddComponent<BoatController>();

        int totalIterations = 10000; 
        float sumOfDriftsX = 0f;
        float sumOfDriftsY = 0f;

        Vector2 startingPos = Vector2.zero; 

        for (int i = 0; i < totalIterations; i++)
        {
            Vector2 appliedDrift = testBoat.CalculateDriftOffset(startingPos);

            sumOfDriftsX += appliedDrift.x;
            sumOfDriftsY += appliedDrift.y;
        }

        Object.DestroyImmediate(testObject);

        float averageDriftX = sumOfDriftsX / totalIterations;
        float averageDriftY = sumOfDriftsY / totalIterations;
        float allowedVariance = 0.05f;

        Assert.AreEqual(0f, averageDriftX, allowedVariance, 
        $"X-axis drift average of {averageDriftX} was not approximately 0.");

        Assert.AreEqual(0f, averageDriftY, allowedVariance, 
        $"Y-axis drift average of {averageDriftY} was not approximately 0.");
    }
}