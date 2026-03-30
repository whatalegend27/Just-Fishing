using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [Header("Fish Setup")]
    public GameObject fishPrefab;
    public List<Sprite> fishSprites;
    public int numberToSpawn = 10;

    [Header("Spawn Bounds")]
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 4f;

    void Start()
    {
        SpawnFish();
    }

    void SpawnFish()
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            Vector2 spawnPos = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            GameObject fish = Instantiate(fishPrefab, spawnPos, Quaternion.identity);

            if (fishSprites != null && fishSprites.Count > 0)
{
    SpriteRenderer sr = fish.GetComponent<SpriteRenderer>();
    if (sr != null)
    {
        sr.sprite = fishSprites[Random.Range(0, fishSprites.Count)];
    }
}

            FishMovement movement = fish.GetComponent<FishMovement>();
            if (movement != null)
            {
                movement.minX = minX;
                movement.maxX = maxX;
                movement.minY = minY;
                movement.maxY = maxY;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0f);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0f);
        Gizmos.DrawWireCube(center, size);
    }
}