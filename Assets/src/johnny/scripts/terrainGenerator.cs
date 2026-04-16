using UnityEngine;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private WeatherController weatherControllerScript; // Reference to the weather controller script
        
    [Tooltip("The difficulty set by the player, influencing the terrain complexity.")]
    [SerializeField] private int playerDifficulty = 1; // Default difficulty level (0: BC, 1: Normal)

    public static TerrainGenerator Instance { get; private set; }

    private void Awake()
    {
        // Implement singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        float globalDifficulty = CalculateDifficulty();
    }

    public float CalculateRandomDifficultyModifier()
    {
        return Random.Range(0.8f, 1.2f);
    }

    private float CalculateDifficulty()
    {
        // Base difficulty multiplier
        float difficultyMultiplier = 1.0f;

        if (playerDifficulty == 0)
        {
            Debug.Log("Player is in BC mode. No terrain generation will occur.");
            return 0.0f; // Return zero difficulty for BC level
        }

        // Adjust difficulty based on weather conditions
        switch (weatherControllerScript != null ? weatherControllerScript.GetCurrentWeather() : "Sunny")
        {
            case "Sunny":
                break; // No change for sunny weather
            case "Cloudy":
                difficultyMultiplier *= 1.2f; // Slightly increase difficulty for cloudy weather
                break;
            case "Rainy":
                difficultyMultiplier *= 1.5f; // Moderately increase difficulty for rainy weather
                break;
            case "Stormy":
                difficultyMultiplier *= 2.0f; // Significantly increase difficulty for stormy weather
                break;
            default:
                Debug.LogWarning("Unknown weather condition: " + (weatherControllerScript != null ? weatherControllerScript.GetCurrentWeather() : "NULL"));
                break;
        }

        // Adjust difficulty based on player-selected difficulty level
        switch (playerDifficulty)
        {
            case 0: // BC
                difficultyMultiplier = 0.0f; // Decrease difficulty for BC mode
                return difficultyMultiplier; // Return early since BC mode has no terrain generation
            case 1: // Casual
                break;
            case 2: // Hard
                difficultyMultiplier *= 1.5f; // Increase difficulty for Hard level
                break;
            default:
                Debug.LogWarning("Unknown player difficulty level: " + playerDifficulty);
                break;
        }

        Debug.Log("Weather condition: " + (weatherControllerScript != null ? weatherControllerScript.GetCurrentWeather() : "NULL"));
        Debug.Log("Player difficulty level: " + playerDifficulty);
        Debug.Log("Calculated difficulty multiplier: " + difficultyMultiplier);
        
        float randomMultiplier = CalculateRandomDifficultyModifier();
        difficultyMultiplier *= randomMultiplier; // Apply random multiplier to add variability
        Debug.Log("Random multiplier applied: " + randomMultiplier);
        Debug.Log("Final difficulty multiplier: " + difficultyMultiplier);

        return difficultyMultiplier;
    }

}