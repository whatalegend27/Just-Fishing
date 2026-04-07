using UnityEngine;
using System.Collections.Generic;

public class FishDatabaseManager : MonoBehaviour
{
    public static FishDatabaseManager Instance;

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

    public bool RegisterFish(string fishName)
    {
        for (int i = 0; i < fishDatabase.Count; i++)
        {
            if (fishDatabase[i].fishName == fishName)
            {
                fishDatabase[i].fishKnown = true;
                return true;
            }
        }

        return false;
    }
}