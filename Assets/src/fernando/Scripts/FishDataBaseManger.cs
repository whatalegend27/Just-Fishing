using UnityEngine;
using System.Collections.Generic;
using System;

public class FishDatabaseManager : MonoBehaviour
{
    public static event Action<string> OnFishRegistered;
    public static event Action OnAllFishCaught;
    public static FishDatabaseManager Instance { get; private set; }

    public List<FishData> fishDatabase = new List<FishData>
    {
        new FishData { fishName = "CatFish",       fishKnown = false },
        new FishData { fishName = "Nemo",           fishKnown = false },
        new FishData { fishName = "OrangeFish",     fishKnown = false },
        new FishData { fishName = "ButterflyFish",  fishKnown = false },
        new FishData { fishName = "SilverFish",     fishKnown = false },
        new FishData { fishName = "SkellyFish",     fishKnown = false },
        new FishData { fishName = "BigBruce",       fishKnown = false },
    };

    // Enforces singleton and persists across scene loads
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Clears the singleton reference, used in tests to reset state between runs
    public static void ResetInstance()
    {
        Instance = null;
    }

    // Marks a fish as known, increments its catch count, and fires events; returns false if not found
    public bool RegisterFish(string fishName)
    {
        for (int i = 0; i < fishDatabase.Count; i++)
        {
            if (fishDatabase[i].fishName == fishName)
            {
                fishDatabase[i].fishKnown = true;
                fishDatabase[i].catchCount++;
                OnFishRegistered?.Invoke(fishName);

                if (fishDatabase.TrueForAll(f => f.fishKnown))
                    OnAllFishCaught?.Invoke();

                return true;
            }
        }

        return false;
    }
}
