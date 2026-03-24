using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void CalculateHunger()
    {
        
    }
}
