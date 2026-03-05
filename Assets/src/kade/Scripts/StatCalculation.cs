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

    void Update() //checks if any game over conditions are met
    {
        if (riskVal<=100)
        {
            //triggerGameOver();
        }
    }

    /*
    CalculateRisk(action)
    action: The illegal action that the player is performing, valid values are "steal", "nightFish", and "blackMarket"
    Make sure that if a player is performing these actions that this function is called
    */
    public void CalculateRisk(string action)
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
