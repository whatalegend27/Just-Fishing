using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using System.Collections;

public class riskBarTest
{
    private GameObject m_Root;
    private ArrestStats m_ArrestStats;
    private HealthStats m_HealthStats;
    private UIUpdater m_UIUpdater;
    private RectTransform m_RiskBarFill;

    [SetUp]
    public void SetUp()
    {
        // Stats objects
        GameObject statsObj = new GameObject("Stats");
        PlayerStats ps      = statsObj.AddComponent<PlayerStats>();
        m_ArrestStats       = statsObj.AddComponent<ArrestStats>();
        m_ArrestStats.ps    = ps;
        m_HealthStats       = statsObj.AddComponent<HealthStats>();
        m_HealthStats.ps    = ps;

        // Risk bar fill rect
        m_Root = new GameObject("RiskBarFill", typeof(RectTransform));
        m_RiskBarFill = m_Root.GetComponent<RectTransform>();
        m_RiskBarFill.sizeDelta = new Vector2(200f, 20f);

        // UIUpdater wired up
        GameObject updaterObj   = new GameObject("UIUpdater");
        m_UIUpdater             = updaterObj.AddComponent<UIUpdater>();
        m_UIUpdater.arrestStats = m_ArrestStats;
        m_UIUpdater.healthStats = m_HealthStats;
        m_UIUpdater.riskBarFill      = m_RiskBarFill;
        m_UIUpdater.riskBarMaxWidth  = 200f;
        m_UIUpdater.healthBarMaxWidth = 200f;
        m_UIUpdater.hungerBarMaxWidth = 200f;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(m_Root);
        Object.DestroyImmediate(m_UIUpdater.gameObject);
        Object.DestroyImmediate(m_ArrestStats.gameObject);
    }

    // Risk starts at 0, bar fill width should be 0
    [UnityTest]
    public IEnumerator RiskBar_StartsAtZero()
    {
        yield return null; // let Start() run

        Assert.AreEqual(0f, m_RiskBarFill.sizeDelta.x,
            "Bar fill width should be 0 when risk is 0");
    }

    // After one steal action (+5), bar should reflect 5% of max width
    [UnityTest]
    public IEnumerator RiskBar_UpdatesAfterSteal()
    {
        yield return null; // Start()

        m_ArrestStats.CalculateRisk("steal");
        yield return null; // Update()

        float expected = 200f * (5f / 100f);
        Assert.AreEqual(expected, m_RiskBarFill.sizeDelta.x, 0.01f,
            "Bar fill should be 5% after one steal");
    }

    // After one nightFish action (+10), bar should reflect 10%
    [UnityTest]
    public IEnumerator RiskBar_UpdatesAfterNightFish()
    {
        yield return null;

        m_ArrestStats.CalculateRisk("nightFish");
        yield return null;

        float expected = 200f * (10f / 100f);
        Assert.AreEqual(expected, m_RiskBarFill.sizeDelta.x, 0.01f,
            "Bar fill should be 10% after one nightFish");
    }

    // Multiple actions stack — steal(5) + nightFish(10) + blackMarket(5) = 20
    [UnityTest]
    public IEnumerator RiskBar_StacksMultipleActions()
    {
        yield return null;

        m_ArrestStats.CalculateRisk("steal");
        m_ArrestStats.CalculateRisk("nightFish");
        m_ArrestStats.CalculateRisk("blackMarket");
        yield return null;

        float expected = 200f * (20f / 100f);
        Assert.AreEqual(expected, m_RiskBarFill.sizeDelta.x, 0.01f,
            "Bar fill should be 20% after steal + nightFish + blackMarket");
    }

    // Risk reaching 100 triggers game over and resets to 0, bar should go back to 0
    [UnityTest]
    public IEnumerator RiskBar_ResetsOnGameOver()
    {
        yield return null;

        // 10 nightFish actions = 100 risk
        for (int i = 0; i < 10; i++)
            m_ArrestStats.CalculateRisk("nightFish");

        yield return null; // ArrestStats.Update() fires game over and resets riskVal
        yield return null; // UIUpdater.Update() catches the reset

        Assert.AreEqual(0f, m_RiskBarFill.sizeDelta.x, 0.01f,
            "Bar fill should return to 0 after game over resets risk");
    }
}
