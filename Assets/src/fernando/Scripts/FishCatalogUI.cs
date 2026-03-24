using UnityEngine;

public class FishCatalogUI : MonoBehaviour
{
    public GameObject RT;

    void Start()
    {
        RT.SetActive(false);

        for (int i = 0; i < FishDatabaseManager.Instance.fishDatabase.Count; i++)
        {
            FishData fish = FishDatabaseManager.Instance.fishDatabase[i];

            if (fish.fishKnown)
            {
                RT.SetActive(true);
            }
        }
    }
}