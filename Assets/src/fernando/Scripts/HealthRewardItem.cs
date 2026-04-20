using UnityEngine;

[CreateAssetMenu(fileName = "HealthRewardItem", menuName = "Rewards/HealthRewardItem")]
public class HealthRewardItem : StackableItem
{
    [SerializeField] private int healAmount = 20;
    public int HealAmount => healAmount;
}
