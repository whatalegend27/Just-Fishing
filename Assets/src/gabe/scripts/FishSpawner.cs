using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [System.Serializable]
    private class SpawnBounds
    {
        public float minX = -8f;
        public float maxX = 8f;
        public float minY = -9f;
        public float maxY = 3f;
    }

    [Header("Fish Setup")]
    public List<GameObject> fishPrefabs;
    public int numberToSpawn = 10;

    [Header("Spawn Bounds")]
    [SerializeField] private SpawnBounds bounds = new SpawnBounds();

    protected virtual void Start()
    {
        SpawnFish();
    }

    protected void SpawnFish()
    {
        if (fishPrefabs == null || fishPrefabs.Count == 0)
        {
            Debug.LogWarning("No fish prefabs assigned in FishSpawner.");
            return;
        }

        for (int i = 0; i < numberToSpawn; i++)
        {
            Vector2 spawnPos = GetSpawnPosition();
            GameObject selectedPrefab = GetFishPrefab();

            if (selectedPrefab == null)
            {
                Debug.LogWarning("Selected fish prefab was null.");
                continue;
            }

            GameObject fish = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
            ConfigureFish(fish);
        }
    }

    protected virtual Vector2 GetSpawnPosition()
    {
        return new Vector2(
            Random.Range(bounds.minX, bounds.maxX),
            Random.Range(bounds.minY, bounds.maxY)
        );
    }

    protected virtual GameObject GetFishPrefab()
    {
        return fishPrefabs[Random.Range(0, fishPrefabs.Count)];
    }

    protected virtual void ConfigureFish(GameObject fish)
    {
        FishMovement movement = fish.GetComponent<FishMovement>();
        if (movement != null)
        {
            movement.minX = bounds.minX;
            movement.maxX = bounds.maxX;
            movement.minY = bounds.minY;
            movement.maxY = bounds.maxY;
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3(
            (bounds.minX + bounds.maxX) / 2f,
            (bounds.minY + bounds.maxY) / 2f,
            0f
        );

        Vector3 size = new Vector3(
            bounds.maxX - bounds.minX,
            bounds.maxY - bounds.minY,
            0f
        );

        Gizmos.DrawWireCube(center, size);
    }
}