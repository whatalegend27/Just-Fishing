using UnityEngine;

// Attach this to any GameObject in the scene alongside (or referencing) HealthStats and ArrestStats.
// It simulates time-based stat decay and lets you trigger actions with key presses
// so you can visually see the bars update in play mode.
//
// Key bindings:
//   F - fish      (exhaustion -5)
//   E - eat       (hunger +20)
//   S - steal     (risk +5, exhaustion -10)
//   N - nightFish (risk +10)
//   B - blackMarket (risk +5)
//   R - rest      (exhaustion +20)

public class StatDriver : MonoBehaviour
{
    public HealthStats healthStats;
    public ArrestStats arrestStats;

    [Tooltip("Seconds between each automatic hunger tick (-5)")]
    public float hungerTickInterval = 5f;

    private float m_HungerTimer;

    void Update()
    {
        // Time-based hunger drain
        m_HungerTimer += Time.deltaTime;
        if (m_HungerTimer >= hungerTickInterval)
        {
            m_HungerTimer = 0f;
            healthStats.CalculateHunger("time");
        }

        // Key-press actions
        if (Input.GetKeyDown(KeyCode.F))
            healthStats.CalculateExhaustion("fish");

        if (Input.GetKeyDown(KeyCode.E))
            healthStats.CalculateHunger("eat");

        if (Input.GetKeyDown(KeyCode.S))
        {
            arrestStats.CalculateRisk("steal");
            healthStats.CalculateExhaustion("steal");
        }

        if (Input.GetKeyDown(KeyCode.N))
            arrestStats.CalculateRisk("nightFish");

        if (Input.GetKeyDown(KeyCode.B))
            arrestStats.CalculateRisk("blackMarket");

        if (Input.GetKeyDown(KeyCode.R))
            healthStats.CalculateExhaustion("rest");
    }
}
