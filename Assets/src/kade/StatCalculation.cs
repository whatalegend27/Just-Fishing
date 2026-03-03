using UnityEngine;

public class RiskCalculation : MonoBehaviour
{
    public int riskVal;
    public int hungerVal;
    public int exhaustionVal;
    void Start()
    {
        riskVal=0;
    }

    void Update()
    {
        if (riskVal<=100)
        {
            //triggerGameOver();
        }
    }
    public int CalculateRisk(string action)
    {
        if (action=="steal")
        {
            riskVal=riskVal+5;
        }
        if (action=="nightFish")
        {
            riskVal=riskVal+10;
        }
        if (action=="blackMarket")
        {
            riskVal=riskVal+5;
        }
    }
}
