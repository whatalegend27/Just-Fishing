using UnityEngine;
using UnityEngine.SceneManagement;

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
            SceneManager.LoadScene("GameOver");
        }
    }

    // CalculateRisk(action), more info in README
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
