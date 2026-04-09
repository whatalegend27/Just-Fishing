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

    [SerializeField] private string currentWeather = "Sunny"; // Default weather condition

    private void Start()
    {
        ChangeWeather();
    }
    public string GetCurrentWeather()
    {
        return currentWeather;
    }

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
        return "Sunny";
    }

    public void ChangeWeather()
    {
        currentWeather = DetermineWeather();
        Debug.Log("Weather changed to: " + currentWeather);
    }
}