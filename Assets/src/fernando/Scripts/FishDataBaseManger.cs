using UnityEngine;
using System.Collections.Generic;

public class FishDatabaseManager : MonoBehaviour
{
    public static FishDatabaseManager Instance;

    public List<FishData> fishDatabase = new List<FishData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keeps it between scenes
        }
        else
        {
            Destroy(gameObject); // prevents duplicates
        }
    }

    public void RegisterFish(string fishName)
    {
        for (int i = 0; i < fishDatabase.Count; i++)
        {
            if (fishDatabase[i].fishName == fishName)
            {
                fishDatabase[i].fishKnown = true;
                return;
            }
        }
    }
}