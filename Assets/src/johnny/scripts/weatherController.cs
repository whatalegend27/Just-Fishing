using UnityEngine;
using System.Collections.Generic;

public class weatherController : MonoBehaviour
{
    private readonly Dictionary<string, float> weatherEffects = new Dictionary<string, float>
    {
        { "Sunny", 0.4f },
        { "Cloudy", 0.3f },
        { "Rainy", 0.2f },
        { "Stormy", 0.1f }
    };

    public string currentWeather;

    private void Start()
    {
        ChangeWeather();
    }

    // Public for testing purposes, but could be private in the final version
    public string DetermineWeather()
    {
        float randomWeather = Random.Range(0.0f, 1.0f);
        float cumulativeProbability = 0.0f;

        foreach (var kvp in weatherEffects)
        {
            cumulativeProbability += kvp.Value;
            if (randomWeather <= cumulativeProbability)
            {
                return kvp.Key;
            }
        }
        return "Sunny"; // Fallback
    }

    public void ChangeWeather()
    {
        currentWeather = DetermineWeather();
        Debug.Log("Weather changed to: " + currentWeather);
    }
}