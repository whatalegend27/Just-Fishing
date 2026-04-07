using UnityEngine;

public class HealthStats : MonoBehaviour
{
    public int healthVal;
    public int hungerVal;
    public PlayerStats ps;
    public inGameTime gameTime;

    private int m_LastHour;

    void Start()
    {
        healthVal = 100;
        hungerVal = 100;
        m_LastHour = gameTime != null ? gameTime.hours : -1;
    }

    void Update()
    {
        if (hungerVal <= 0 || healthVal <= 0)
            ps.gameOver = true;

        // Reset health at 6:00 (new day)
        if (gameTime != null && gameTime.hours == 6 && m_LastHour != 6)
            healthVal = 100;

        if (gameTime != null)
            m_LastHour = gameTime.hours;
    }

    // CalculateHealth(action), valid values: "hurt"
    public void CalculateHealth(string action)
    {
        IStatCalculator calculator = new BaseStatCalculator();
        switch (action)
        {
            case "hurt": calculator = new HurtHealthDecorator(calculator); break;
        }
        healthVal = Mathf.Clamp(calculator.Calculate(healthVal), 0, 100);
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
}
