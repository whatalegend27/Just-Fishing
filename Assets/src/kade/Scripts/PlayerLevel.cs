using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public int level = 1;
    public int currentXP = 0;

    public event System.Action<int> OnLevelUp;

    // XP required to reach the next level — increases by 100 each level
    public int XPToNextLevel => level * 100;

    // Call this when the player fishes
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
