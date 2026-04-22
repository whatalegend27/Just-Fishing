using System.Collections.Generic;
using UnityEngine;

//dynamic binding is mainly implemeted in this script
//virtual: allows a method to be replaced by a child class
//virtual: w/o virt, child classes can't override method
//override: this can be replaced in a child class.
//override: parent method must be marked virt, and method signature must match
//protected: Only this class(and subclasses) can access that method.
//pretected: In my instance; prevents other scripts from calling spawn methods
//protected: but still allows subclasses to customize the behavior

//1:unity created the object to be spawned
//2:when start runs, can choose an override method to call instead of base
//3: this script calls the virtual methods


// This script is responsible for spawning fish into the scene
// Picks random prefabs, places them within defined spawn area
// then configs their movement bounds so stay inside that area
public class FishSpawner : MonoBehaviour
{
    // This private class groups the spawn boundary values together
    // so they appear as one section in the Inspector.
    // [System.Serializable] allows Unity to show this private class in the Inspector.
    [System.Serializable]
    private class SpawnBounds
    {
        public float minX = -8f;

        public float maxX = 8f;

        public float minY = -9f;

        public float maxY = 3f;
    }

    // ===== FISH SETUP =====
    [Header("Fish Setup")]

    // fish prefabs that can be randomly spawned
    public List<GameObject> fishPrefabs;

    // How many fish to create when started
    public int numberToSpawn = 10;

    // ===== SPAWN BOUNDS =====
    [Header("Spawn Bounds")]

    // Stores the boundary values in a grouped object
    // [SerializeField] makes this private variable visible in the Inspector
    [SerializeField] private SpawnBounds bounds = new SpawnBounds();


    // Marked virtual so child classes can override this behavior if needed
    protected virtual void Start()
    {
        // Spawn all fish when the scene starts
        SpawnFish();
    }

    // ===== MAIN SPAWN LOGIC =====
    // Creating multiple fish in the scene
    protected void SpawnFish()
    {
        // if no prefabs are assigned, stop and warn in Console
        if (fishPrefabs == null || fishPrefabs.Count == 0)
        {
            Debug.LogWarning("No fish prefabs assigned in FishSpawner.");
            return;
        }

        // Repeat until right number of fish has been spawned
        for (int i = 0; i < numberToSpawn; i++)
        {
            // random spawn position inside the bounds
            Vector2 spawnPos = GetSpawnPosition();

            // Pick fish prefab from the list
            GameObject selectedPrefab = GetFishPrefab();

            // check in case the selected prefab is null
            if (selectedPrefab == null)
            {
                Debug.LogWarning("Selected fish prefab was null.");
                continue;
            }

            // Create fish in scene at the chosen position
            GameObject fish = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);

            // Apply any setup/configuration to fish after spawning
            ConfigureFish(fish);
        }
    }

    // ===== RANDOM POS SELECTION =====
    // Chooses a random point inside the spawn area
    // Marked virtual so subclasses can change how positions are chosen
    protected virtual Vector2 GetSpawnPosition()
    {
        return new Vector2(
            Random.Range(bounds.minX, bounds.maxX),
            Random.Range(bounds.minY, bounds.maxY)
        );
    }

    // ===== PREFAB SELECTION =====
    // Picks random fish prefab from the list
    // Marked virtual so subclasses can override selection behavior
    protected virtual GameObject GetFishPrefab()
    {
        return fishPrefabs[Random.Range(0, fishPrefabs.Count)];
    }

    // ===== POST-SPAWN CONFIG =====
    // Sets up spawned fish after created
    //  passes the spawner's bounds into FishMovement
    protected virtual void ConfigureFish(GameObject fish)
    {
        // Try to find a FishMovement script on the spawned fish
        FishMovement movement = fish.GetComponent<FishMovement>();

        // Only configure movement if the script exists
        if (movement != null)
        {
            // Give fish same bounds as spawner
            // fish stays within allowed area
            movement.minX = bounds.minX;
            movement.maxX = bounds.maxX;
            movement.minY = bounds.minY;
            movement.maxY = bounds.maxY;
        }
    }

    // ===== SCENE VIEW VISUALIZATION =====
    // Draw spawn area in scene view when object selected
    // helps see boundaries w/o play mode
    protected virtual void OnDrawGizmosSelected()
    {
        // Set Gizmo color to cyan
        Gizmos.color = Color.cyan;

        // calc the center point of the boundary box
        Vector3 center = new Vector3(
            (bounds.minX + bounds.maxX) / 2f,
            (bounds.minY + bounds.maxY) / 2f,
            0f
        );

        // calc the width and height of the box
        Vector3 size = new Vector3(
            bounds.maxX - bounds.minX,
            bounds.maxY - bounds.minY,
            0f
        );

        // Draw a box in the Scene view
        Gizmos.DrawWireCube(center, size);
    }
}