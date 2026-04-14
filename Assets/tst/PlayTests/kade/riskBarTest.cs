using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class riskBarTest
{
    private GameObject  m_RiskBarFill;
    private GameObject  m_StatsObj;
    private GameObject  m_UpdaterObj;
    private ArrestStats m_ArrestStats;
    private HealthStats m_HealthStats;
    private UIUpdater   m_UIUpdater;

    [SetUp]
    public void SetUp()
    {
        // Stats objects
        m_StatsObj          = new GameObject("Stats");
        PlayerStats ps      = m_StatsObj.AddComponent<PlayerStats>();
        m_ArrestStats       = m_StatsObj.AddComponent<ArrestStats>();
        m_ArrestStats.ps    = ps;
        m_HealthStats       = m_StatsObj.AddComponent<HealthStats>();
        m_HealthStats.ps    = ps;

        // 2D fill square
        m_RiskBarFill = new GameObject("RiskBarFill");
        m_RiskBarFill.transform.localScale = new Vector3(1f, 1f, 1f);

        // UIUpdater wired up
        m_UpdaterObj             = new GameObject("UIUpdater");
        m_UIUpdater              = m_UpdaterObj.AddComponent<UIUpdater>();
        m_UIUpdater.arrestStats  = m_ArrestStats;
        m_UIUpdater.healthStats  = m_HealthStats;
        m_UIUpdater.riskBarFill  = m_RiskBarFill.transform;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(m_RiskBarFill);
        Object.DestroyImmediate(m_StatsObj);
        Object.DestroyImmediate(m_UpdaterObj);
    }

    // Risk starts at 0, bar scale should be 0
    [UnityTest]
    public IEnumerator RiskBar_StartsAtZero()
    {
        yield return null;

        Assert.AreEqual(0f, m_RiskBarFill.transform.localScale.x,
            "Bar scale should be 0 when risk is 0");
    }

    // After one steal (+5), scale should be 0.05
    [UnityTest]
    public IEnumerator RiskBar_UpdatesAfterSteal()
    {
        yield return null;

        m_ArrestStats.calculateRisk("steal");
        yield return null;

        Assert.AreEqual(0.05f, m_RiskBarFill.transform.localScale.x, 0.001f,
            "Bar scale should be 0.05 after one steal");
    }

    // After one nightFish (+10), scale should be 0.10
    [UnityTest]
    public IEnumerator RiskBar_UpdatesAfterNightFish()
    {
        yield return null;

        m_ArrestStats.calculateRisk("nightFish");
        yield return null;

        Assert.AreEqual(0.10f, m_RiskBarFill.transform.localScale.x, 0.001f,
            "Bar scale should be 0.10 after one nightFish");
    }

    // steal(5) + nightFish(10) + blackMarket(5) = 20, scale 0.20
    [UnityTest]
    public IEnumerator RiskBar_StacksMultipleActions()
    {
        yield return null;

        m_ArrestStats.calculateRisk("steal");
        m_ArrestStats.calculateRisk("nightFish");
        m_ArrestStats.calculateRisk("blackMarket");
        yield return null;

        Assert.AreEqual(0.20f, m_RiskBarFill.transform.localScale.x, 0.001f,
            "Bar scale should be 0.20 after steal + nightFish + blackMarket");
    }

    // 10x nightFish = 100 risk, game over resets to 0, bar should return to 0
    [UnityTest]
    public IEnumerator RiskBar_ResetsOnGameOver()
    {
        yield return null;

        for (int i = 0; i < 10; i++)
            m_ArrestStats.calculateRisk("nightFish");

        yield return null;
        yield return null;

        Assert.AreEqual(0f, m_RiskBarFill.transform.localScale.x, 0.001f,
            "Bar scale should return to 0 after game over resets risk");
    }
}
