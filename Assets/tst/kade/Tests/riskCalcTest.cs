using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class riskTest
{
    // A Test behaves as an ordinary method
    [Test]
    [TestCase(99, false)]
    [TestCase(100, true)]
    [TestCase(105, true)]
    public void Risk_atThreshold_TriggerGameOver(int finalRisk, bool shouldEnd)
    {
        GameObject go = new GameObject();
        var riskCalc = go.AddComponent<RiskCalculation>();
        riskCalc.riskVal = finalRisk;

        bool isGameOver = riskCalc.riskVal >= 100;

        Assert.AreEqual(shouldEnd, isGameOver, $"Risk of {finalRisk} should have resulted in GameOver: {shouldEnd}");

    }
}
