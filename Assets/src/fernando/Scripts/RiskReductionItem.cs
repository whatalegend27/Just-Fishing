using UnityEngine;

[CreateAssetMenu(fileName = "RiskReductionItem", menuName = "Rewards/RiskReductionItem")]
public class RiskReductionItem : StackableItem
{
    [SerializeField] private int riskReduction = 10;
    public int RiskReduction => riskReduction;
}
