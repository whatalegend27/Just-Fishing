using UnityEngine;
using UnityEngine.SceneManagement;

// --- Decorator Pattern ---

public interface IStatCalculator
{
    int Calculate(int currentValue);
}

public class BaseStatCalculator : IStatCalculator
{
    public int Calculate(int currentValue) => currentValue;
}

public abstract class StatCalculatorDecorator : IStatCalculator
{
    protected IStatCalculator inner;
    public StatCalculatorDecorator(IStatCalculator inner) { this.inner = inner; }
    public abstract int Calculate(int currentValue);
}

public class StealRiskDecorator : StatCalculatorDecorator
{
    public StealRiskDecorator(IStatCalculator inner) : base(inner) {}
    public override int Calculate(int currentValue) => inner.Calculate(currentValue) + 5;
}

public class NightFishRiskDecorator : StatCalculatorDecorator
{
    public NightFishRiskDecorator(IStatCalculator inner) : base(inner) {}
    public override int Calculate(int currentValue) => inner.Calculate(currentValue) + 10;
}

public class BlackMarketRiskDecorator : StatCalculatorDecorator
{
    public BlackMarketRiskDecorator(IStatCalculator inner) : base(inner) {}
    public override int Calculate(int currentValue) => inner.Calculate(currentValue) + 5;
}

public class EatHungerDecorator : StatCalculatorDecorator
{
    public EatHungerDecorator(IStatCalculator inner) : base(inner) {}
    public override int Calculate(int currentValue) => inner.Calculate(currentValue) + 20;
}

public class TimeHungerDecorator : StatCalculatorDecorator
{
    public TimeHungerDecorator(IStatCalculator inner) : base(inner) {}
    public override int Calculate(int currentValue) => inner.Calculate(currentValue) - 5;
}

public class RestExhaustionDecorator : StatCalculatorDecorator
{
    public RestExhaustionDecorator(IStatCalculator inner) : base(inner) {}
    public override int Calculate(int currentValue) => inner.Calculate(currentValue) + 20;
}

public class FishExhaustionDecorator : StatCalculatorDecorator
{
    public FishExhaustionDecorator(IStatCalculator inner) : base(inner) {}
    public override int Calculate(int currentValue) => inner.Calculate(currentValue) - 5;
}

public class StealExhaustionDecorator : StatCalculatorDecorator
{
    public StealExhaustionDecorator(IStatCalculator inner) : base(inner) {}
    public override int Calculate(int currentValue) => inner.Calculate(currentValue) - 10;
}

// --- MonoBehaviours ---

public class PlayerStats : MonoBehaviour
{

    public bool gameOver;
    void Start()
    {
        gameOver=false;
    }

    void Update() //checks if any game over conditions are met
    {
        if (gameOver==true)
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}

public class ArrestStats: MonoBehaviour
{
    public PlayerStats ps;
    public int riskVal;
    void Start()
    {
        riskVal=0;
    }

    void Update()
    {
        if (riskVal>=100)
        {
            ps.gameOver=true;
            riskVal=0;
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

public class HealthStats: MonoBehaviour
{
    public int hungerVal;
    public int exhaustionVal;
    public PlayerStats ps;

    void Start()
    {
        hungerVal=100;
        exhaustionVal=100;
    }

    void Update()
    {
        if (hungerVal<=0 || exhaustionVal<=0)
        {
            ps.gameOver=true;
        }
    }

    // CalculateHunger(action), valid values: "eat", "time"
    public void CalculateHunger(string action)
    {
        IStatCalculator calculator = new BaseStatCalculator();
        switch (action)
        {
            case "eat":  calculator = new EatHungerDecorator(calculator); break;
            case "time": calculator = new TimeHungerDecorator(calculator); break;
        }
        hungerVal = Mathf.Clamp(calculator.Calculate(hungerVal), 0, 100);
    }

    // CalculateExhaustion(action), valid values: "rest", "fish", "steal"
    public void CalculateExhaustion(string action)
    {
        IStatCalculator calculator = new BaseStatCalculator();
        switch (action)
        {
            case "rest":  calculator = new RestExhaustionDecorator(calculator); break;
            case "fish":  calculator = new FishExhaustionDecorator(calculator); break;
            case "steal": calculator = new StealExhaustionDecorator(calculator); break;
        }
        exhaustionVal = Mathf.Clamp(calculator.Calculate(exhaustionVal), 0, 100);
    }
}
