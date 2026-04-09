using UnityEngine;
using System.Collections.Generic;

public class terrainGenerator : MonoBehaviour
{
    [SerializeField] private weatherController weatherControllerScript; // Reference to the weather controller script
        
    [Tooltip("The difficulty set by the player, influencing the terrain complexity.")]
    [SerializeField] private int playerDifficulty = 1; // Default difficulty level (0: BC, 1: Casual, 2: Hard)

    private void Start()
    {
        float globalDifficulty = CalculateDifficulty();
        GenerateTerrain(globalDifficulty);
    }

    public float CalculateRandomDifficultyModifier()
    {
        return Random.Range(0.00f, 0.25f);
    }

    private float CalculateDifficulty()
    {
        // Base difficulty multiplier
        float difficultyMultiplier = 1.0f;

        if (playerDifficulty == 0)
        {
            Debug.Log("Player is in BC difficulty level. No terrain generation will occur.");
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

    private void GenerateTerrain(float difficulty)
    {
        if (difficulty <= 0.0f)
        {
            Debug.Log("Difficulty is zero or negative. No terrain will be generated.");
            return;
        }

        int minRocks = 0;
        int maxRocks = Mathf.FloorToInt(difficulty * 10f);
        int numRocksToSpawn = Random.Range(minRocks, maxRocks + 1);

        HashSet<Vector2> occupiedLocations = new HashSet<Vector2>();

        for (int i = 0; i < numRocksToSpawn; i++)
        {
            Vector2 newLocation;
            int failsafe = 0;

            do
            {
                float x = Mathf.Round(Random.Range(-20f, 20f));
                float y = Mathf.Round(Random.Range(-20f, 20f));
                
                newLocation = new Vector2(x, y);
                
                failsafe++;
                if (failsafe > 1000) 
                {
                    Debug.LogWarning("Could not find an empty spot for the rock!");
                    break; 
                }

            } while (occupiedLocations.Contains(newLocation));

        }

        boatController boat = FindAnyObjectByType<boatController>();

        if (boat != null) boat.oceanRocksLocation = occupiedLocations;
        else Debug.LogWarning("Terrain generated, but no BoatController was found in the scene.");
    }


}