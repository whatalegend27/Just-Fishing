using UnityEngine;

public class terrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    [Tooltip("Current weather condition affecting the terrain generation.")]
    [SerializeField] private string current_weather = "Sunny"; // Default weather condition
        
    [Tooltip("The difficulty set by the player, influencing the terrain complexity.")]
    [SerializeField] private int playerDifficulty = 1; // Default difficulty level (0: BC, 1: Casual, 2: Hard)



}