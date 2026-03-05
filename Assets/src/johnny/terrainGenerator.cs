using UnityEngine;
using Debug = UnityEngine.Debug;

public class terrainGenerator : MonoBehaviour
{
    [Header("World Settings")]
    [Tooltip("Current weather condition affecting the terrain generation.")]
    [SerializeField] private string currentWeather = "Sunny"; // Default weather condition
        
    [Tooltip("The difficulty set by the player, influencing the terrain complexity.")]
    [SerializeField] private int playerDifficulty = 1; // Default difficulty level (0: BC, 1: Casual, 2: Hard)

    [Header("Terrain Generation Settings")]
    [SerializeField] private int numRocks = 10; // Number of rocks to generate
    [SerializeField] private GameObject rockPrefab; // Prefab for the rocks

    private void Start()
    {
        float globalDifficulty = CalculateDifficulty();
        GenerateTerrain(globalDifficulty);
    }

    private float CalculateDifficulty()
    {
        // Base difficulty multiplier
        float difficultyMultiplier = 1.0f;

        if (rockPrefab == null)
        {
            Debug.LogError("Rock prefab is not assigned.");
            return 0.0f; // Return zero difficulty if prefab is missing
        }
        else if (playerDifficulty == 0)
        {
            Debug.Log("Player is in BC difficulty level. No terrain generation will occur.");
            return 0.0f; // Return zero difficulty for BC level
        }

        // Adjust difficulty based on weather conditions
        switch (currentWeather)
        {
            case "Sunny":
                difficultyMultiplier *= 1.0f; // No change for sunny weather
                break;
            case "Rainy":
                difficultyMultiplier *= 1.5f; // Increase difficulty for rainy weather
                break;
            case "Stormy":
                difficultyMultiplier *= 2.0f; // Significantly increase difficulty for stormy weather
                break;
            default:
                Debug.LogWarning("Unknown weather condition: " + currentWeather);
                break;
        }

        // Adjust difficulty based on player-selected difficulty level
        switch (playerDifficulty)
        {
            case 0: // BC
                difficultyMultiplier = 0.0f; // Decrease difficulty for BC level
                return difficultyMultiplier; // Return early since BC level has no terrain generation
            case 1: // Casual
                difficultyMultiplier *= 1.0f; // No change for Casual level
                break;
            case 2: // Hard
                difficultyMultiplier *= 1.5f; // Increase difficulty for Hard level
                break;
            default:
                Debug.LogWarning("Unknown player difficulty level: " + playerDifficulty);
                break;
        }

        Debug.Log("Calculated difficulty multiplier: " + difficultyMultiplier);
        
        int randomMultiplier = Random.Range(1, 4); // Random multiplier between 1 and 3
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
            return; // Exit early if difficulty is zero or negative
        }

        for (int i = 0; i < numRocks; i++)
        {
            // Randomly position rocks within a certain range, influenced by difficulty
            float x = UnityEngine.Random.Range(-10f, 10f) * difficulty;
            float z = UnityEngine.Random.Range(-10f, 10f) * difficulty;
            Vector3 position = new Vector3(x, 0, z);

            // Instantiate the rock prefab at the calculated position
            Instantiate(rockPrefab, position, Quaternion.identity);
        }
    }


}