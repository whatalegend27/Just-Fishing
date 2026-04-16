using UnityEngine;

public class FishRewardManager : MonoBehaviour
{
    private void OnEnable()
    {
        FishDatabaseManager.OnFishRegistered += OnFishRegistered;
    }

    private void OnDisable()
    {
        FishDatabaseManager.OnFishRegistered -= OnFishRegistered;
    }

    private void OnFishRegistered(string fishName)
    {
        FishData fish = GetFishData(fishName);

        if (fish != null && fish.catchCount == 1)
        {
            // TODO: Replace with GoldManager.Instance.AddGold(10) once Danny adds the method
            Debug.Log("[FishRewardManager] First catch reward: +10 gold (placeholder)");
        }
    }

    private static FishData GetFishData(string fishName)
    {
        foreach (FishData fish in FishDatabaseManager.Instance.fishDatabase)
        {
            if (fish.fishName == fishName)
                return fish;
        }
        return null;
    }
}
