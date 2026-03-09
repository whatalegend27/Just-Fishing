using UnityEngine;

[CreateAssetMenu(fileName = "NewAchievement", menuName = "Achievement/Achievement Title")]
public class Achievment : ScriptableObject
{
    public string AchievementID;
    public string Name;
    [TextArea]
    public string AchievementDescp;
}
