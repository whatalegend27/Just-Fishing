using UnityEngine;

public class FishCatchTrigger : MonoBehaviour
{
    // Registers the fish with the database when the player collider enters the trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Fish fish = GetComponent<Fish>();

            if (fish != null)
            {
                FishDatabaseManager.Instance.RegisterFish(fish.FishName);
            }
        }
    }
}
