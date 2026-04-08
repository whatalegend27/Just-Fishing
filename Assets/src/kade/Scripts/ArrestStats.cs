using UnityEngine;

public class ArrestStats : MonoBehaviour
{
    public PlayerStats ps;
    public int riskVal;

    void Start()
    {
        riskVal = 0;
    }

    void Update()
    {
        if (riskVal >= 100)
        {
            ps.gameOver = true;
            riskVal = 0;
        }
    }

    // CalculateRisk(action), more info in README
    public void CalculateRisk(string action)
    {
        IStatCalculator calculator = new BaseStatCalculator();
        switch (action)
        {
            case "steal":       calculator = new StealRiskDecorator(calculator); break;
            case "nightFish":   calculator = new NightFishRiskDecorator(calculator); break;
            case "blackMarket": calculator = new BlackMarketRiskDecorator(calculator); break;
        }
        riskVal = calculator.Calculate(riskVal);
    }
}
