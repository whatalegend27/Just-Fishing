using NUnit.Framework;
using UnityEngine;

public class StatisticalTests
{
    private GameObject testObject;
    private weatherController testWeather;
    private terrainGenerator testTerrain;

    [SetUp]
    public void Setup()
    {
        testObject = new GameObject();
        testWeather = testObject.AddComponent<weatherController>();
        testTerrain = testObject.AddComponent<terrainGenerator>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(testObject);
    }

    [Test]
    public void WeatherDetermination_MatchesExpectedDistribution()
    {
        int totalIterations = 10000;
        int stormyCount = 0, rainyCount = 0, cloudyCount = 0, sunnyCount = 0;

        for (int i = 0; i < totalIterations; i++)
        {
            string weather = testWeather.DetermineWeather(); 

            switch (weather)
            {
                case "Stormy": stormyCount++; break;
                case "Rainy": rainyCount++; break;
                case "Cloudy": cloudyCount++; break;
                case "Sunny": sunnyCount++; break;
            }
        }

        double allowedVariance = 500; 

        Assert.AreEqual(1000, stormyCount, allowedVariance);
        Assert.AreEqual(2000, rainyCount, allowedVariance);
        Assert.AreEqual(3000, cloudyCount, allowedVariance);
        Assert.AreEqual(4000, sunnyCount, allowedVariance);
    }

    [Test]
    public void GlobalDifficulty_MatchesExpectedValue()
    {
        int totalIterations = 10000; 
        float sumOfDifficulties = 0f;

        // Act
        for (int i = 0; i < totalIterations; i++)
        {
            sumOfDifficulties += testTerrain.CalculateRandomDifficultyModifier();
        }

        float averageDifficulty = sumOfDifficulties / totalIterations; 
        Assert.AreEqual(0.125f, averageDifficulty, 0.05f,
            $"Average difficulty {averageDifficulty} fell outside the 0.05 margin of the expected 0.125");
    }
}