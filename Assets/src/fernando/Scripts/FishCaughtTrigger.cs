using UnityEngine;

public class FishCatchTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Lure"))
        {
            Fish fish = GetComponent<Fish>();

            if (fish != null)
            {
                FishDatabaseManager.Instance.RegisterFish(fish.FishName);
            }
        }
    }
}
