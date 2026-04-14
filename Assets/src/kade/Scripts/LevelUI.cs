using UnityEngine;

public class LevelUI : MonoBehaviour
{
    public PlayerLevel playerLevel;

    // Assign all 7 rectangles in order in the Inspector
    public Transform[] levelBlocks;

    void Start()
    {
        playerLevel.OnLevelUp += HandleLevelUp;
        RefreshBlocks(playerLevel.level);
    }

    void OnDestroy()
    {
        playerLevel.OnLevelUp -= HandleLevelUp;
    }

    void HandleLevelUp(int newLevel)
    {
        RefreshBlocks(newLevel);
    }

    // Fills all blocks up to the current level, empties the rest
    void RefreshBlocks(int currentLevel)
    {
        for (int i = 0; i < levelBlocks.Length; i++)
        {
            if (levelBlocks[i] == null) continue;

            Vector3 s = levelBlocks[i].localScale;
            s.x = i < currentLevel ? 1f : 0f;
            levelBlocks[i].localScale = s;
        }
    }
}
