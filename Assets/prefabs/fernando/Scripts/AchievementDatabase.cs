using UnityEngine;
[CreateAssetMenu(fileName = "AchievementDatabase", menuName = "Achieve/Create New Achievement")]

public class AchievementDatabase : ScriptableObject
{
    public Achievment[] allAchievements;
}
