using UnityEngine;
using System.Collections.Generic;

public class PlayerLevel : MonoBehaviour
{
    public int level = 1;
    public int currentXP = 0;

    public event System.Action<int> OnLevelUp;

    // XP required to reach the next level — increases by 100 each level
    public int XPToNextLevel => level * 100;

    // XP awarded per fish type
    private static readonly Dictionary<string, int> fishXP = new()
    {
        { "CatFish",      10 },
        { "Nemo",         20 },
        { "OrangeFish",   15 },
        { "ButterflyFish",25 },
        { "SilverFish",   20 },
        { "SkellyFish",   30 },
        { "BigBruce",     50 },
    };

    void OnEnable()
    {
        FishDatabaseManager.OnFishRegistered += HandleFishCaught;
    }

    void OnDisable()
    {
        FishDatabaseManager.OnFishRegistered -= HandleFishCaught;
    }

    private void HandleFishCaught(string fishName)
    {
        int xp = fishXP.TryGetValue(fishName, out int val) ? val : 10;
        Debug.Log($"[PlayerLevel] Caught {fishName} — +{xp} XP");
        AddXP(xp);
    }

    public void AddXP(int amount)
    {
        currentXP += amount;

        while (currentXP >= XPToNextLevel)
        {
            currentXP -= XPToNextLevel;
            level++;
            OnLevelUp?.Invoke(level);
            Debug.Log($"[PlayerLevel] Leveled up! Now level {level}");
        }
    }
}
