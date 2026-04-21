using NUnit.Framework;
using UnityEngine;

/* These tests are designed to validate that the boat's drift behavior is consistent with the expected physics model.

Each test case sets specific values for speed, current, and wind, and then runs a large number of drift calculations to ensure that the average drift over time is close to zero (within a reasonable variance).

The variance values are determined based on the expected intensity of the drift for each scenario, with more extreme conditions allowing for a larger variance due to the increased randomness in the drift. */

public class DriftTests
{
    [TestCase(0f, 0f, 0f, 0.1f)]    // Absolutely still, no weather
    [TestCase(0f, 1f, 0f, 0.1f)]    // Idle in mild current
    [TestCase(0f, 0f, 1f, 0.1f)]    // Idle in mild wind
    [TestCase(0f, 2f, 2f, 0.2f)]    // Idle with both mild current and wind
    [TestCase(0f, 5f, 5f, 0.3f)]    // Idle in strong weather
    [TestCase(0f, -5f, 5f, 0.3f)]   // Idle with opposing strong weather forces

    [TestCase(5f, 0f, 0f, 0.2f)]    // Low speed, calm waters
    [TestCase(5f, 1f, 0f, 0.2f)]    // Low speed, mild current
    [TestCase(5f, 0f, 1f, 0.2f)]    // Low speed, mild wind
    [TestCase(5f, 1f, 1f, 0.2f)]    // Low speed, mild weather combo
    [TestCase(5f, -2f, 0f, 0.2f)]   // Low speed, fighting a weak head-current
    [TestCase(5f, 0f, -2f, 0.2f)]   // Low speed, fighting a weak head-wind

    [TestCase(10f, 0f, 0f, 0.3f)]   // Cruising, calm waters
    [TestCase(10f, 2f, 0f, 0.3f)]   // Cruising, moderate current
    [TestCase(10f, 0f, 2f, 0.3f)]   // Cruising, moderate wind
    [TestCase(10f, 2f, 2f, 0.3f)]   // Cruising, moderate weather combo
    [TestCase(10f, -2f, 2f, 0.3f)]  // Cruising, head-current but tail-wind
    [TestCase(10f, 5f, 5f, 0.5f)]   // Cruising, strong weather

    [TestCase(20f, 0f, 0f, 0.5f)]   // High speed, calm waters
    [TestCase(20f, 5f, 0f, 0.5f)]   // High speed, strong current
    [TestCase(20f, 0f, 5f, 0.5f)]   // High speed, strong wind
    [TestCase(20f, 5f, 5f, 0.5f)]   // High speed, strong weather combo
    [TestCase(20f, 10f, 10f, 0.6f)] // High speed, extreme storm conditions
    [TestCase(50f, 0f, 0f, 0.6f)]   // Max speed, no weather
    [TestCase(50f, 20f, 20f, 0.8f)] // Max speed, hurricane conditions

    [TestCase(-5f, 0f, 0f, 0.2f)]   // Reverse speed, calm waters
    [TestCase(-5f, 2f, 0f, 0.2f)]   // Reverse speed, tail-current
    [TestCase(-10f, 5f, 5f, 0.5f)]  // Fast reverse, strong weather
    [TestCase(10f, 100f, 0f, 0.8f)] // Absurd current edge case
    [TestCase(10f, 0f, 100f, 0.8f)] // Absurd wind edge case
    public void BoatDrift_VariesCorrectly(float speed, float current, float wind, float variance)
    {
        GameObject testObject = new GameObject();
        BoatController testBoat = testObject.AddComponent<BoatController>();
        
        testBoat.Speed = speed;
        testBoat.CurrentForce = current;
        testBoat.WindForce = wind;

        int totalIterations = 10000;
        float sumOfDriftsX = 0f;
        float sumOfDriftsY = 0f;
        
        for (int i = 0; i < totalIterations; i++)
        {
            Vector2 drift = testBoat.CalculateDriftOffset(Vector2.zero);
            sumOfDriftsX += drift.x;
            sumOfDriftsY += drift.y;
        }

        Object.DestroyImmediate(testObject);

        float averageDriftX = sumOfDriftsX / totalIterations;
        float averageDriftY = sumOfDriftsY / totalIterations;

        Assert.AreEqual(0f, averageDriftX, variance, 
            $"X-axis failed: Speed={speed}, Current={current}, Wind={wind}");
            
        Assert.AreEqual(0f, averageDriftY, variance, 
            $"Y-axis failed: Speed={speed}, Current={current}, Wind={wind}");
    }
}