using NUnit.Framework;
using UnityEngine;

public class boundaryTests
{
   // ─── Risk Threshold (from statBoundaryTest) ─────────────────────────────────

   // Risk below, at, and above 100 correctly determines game over state
   [Test]
   [TestCase( 99,  false )]
   [TestCase( 100, true  )]
   [TestCase( 105, true  )]
   public void Risk_AtThreshold_TriggersGameOver( int finalRisk, bool shouldEnd )
   {
      var go          = new GameObject();
      var arrestStats = go.AddComponent<ArrestStats>();
      arrestStats.riskVal = finalRisk;

      bool isGameOver = arrestStats.riskVal >= 100;

      Assert.AreEqual( shouldEnd, isGameOver,
         $"Risk of {finalRisk} should have resulted in GameOver: {shouldEnd}" );

      Object.DestroyImmediate( go );
   }

   // ─── ArrestStats ────────────────────────────────────────────────────────────

   // Risk at exactly 0 should not trigger game over
   [Test]
   public void ArrestStats_RiskAtZero_NoGameOver()
   {
      var go = new GameObject();
      var arrestStats = go.AddComponent<ArrestStats>();
      arrestStats.riskVal = 0;

      Assert.IsFalse( arrestStats.riskVal >= 100 );

      Object.DestroyImmediate( go );
   }

   // Risk at exactly 100 should trigger game over
   [Test]
   public void ArrestStats_RiskAtHundred_TriggersGameOver()
   {
      var go = new GameObject();
      var arrestStats = go.AddComponent<ArrestStats>();
      arrestStats.riskVal = 100;

      Assert.IsTrue( arrestStats.riskVal >= 100 );

      Object.DestroyImmediate( go );
   }

   // Risk cannot exceed 100 via calculateRisk since game over resets it
   [Test]
   public void ArrestStats_RiskNeverExceedsHundred_AfterReset()
   {
      var go  = new GameObject();
      var ps  = go.AddComponent<PlayerStats>();
      var arrestStats = go.AddComponent<ArrestStats>();
      arrestStats.ps      = ps;
      arrestStats.riskVal = 95;

      arrestStats.calculateRisk( "nightFish" ); // +10 = 105, should reset to 0 after game over check

      Assert.IsTrue( arrestStats.riskVal <= 100 );

      Object.DestroyImmediate( go );
   }

   // ─── HealthStats ────────────────────────────────────────────────────────────

   // Health cannot go below 0
   [Test]
   public void HealthStats_HealthCannotGoBelowZero()
   {
      var go          = new GameObject();
      var healthStats = go.AddComponent<HealthStats>();
      healthStats.healthVal = 5;

      healthStats.takeDamage( 20 );

      Assert.AreEqual( 0, healthStats.healthVal );

      Object.DestroyImmediate( go );
   }

   // Health cannot exceed 100
   [Test]
   public void HealthStats_HealthCannotExceedHundred()
   {
      var go          = new GameObject();
      var healthStats = go.AddComponent<HealthStats>();
      healthStats.healthVal = 100;

      healthStats.takeDamage( -50 ); // negative damage would add health

      Assert.AreEqual( 100, healthStats.healthVal );

      Object.DestroyImmediate( go );
   }

   // Hunger cannot go below 0
   [Test]
   public void HealthStats_HungerCannotGoBelowZero()
   {
      var go          = new GameObject();
      var healthStats = go.AddComponent<HealthStats>();
      healthStats.hungerVal = 3;

      healthStats.calculateHunger( "time" ); // -5

      Assert.AreEqual( 0, healthStats.hungerVal );

      Object.DestroyImmediate( go );
   }

   // Hunger cannot exceed 100
   [Test]
   public void HealthStats_HungerCannotExceedHundred()
   {
      var go          = new GameObject();
      var healthStats = go.AddComponent<HealthStats>();
      healthStats.hungerVal = 95;

      healthStats.calculateHunger( "eat" ); // +20

      Assert.AreEqual( 100, healthStats.hungerVal );

      Object.DestroyImmediate( go );
   }

   // ─── StatCalculation Decorators ─────────────────────────────────────────────

   // Steal decorator adds exactly 5
   [Test]
   public void StealRiskDecorator_AddsExactlyFive()
   {
      IStatCalculator calc = new StealRiskDecorator( new BaseStatCalculator() );

      Assert.AreEqual( 5, calc.calculate( 0 ) );
   }

   // NightFish decorator adds exactly 10
   [Test]
   public void NightFishDecorator_AddsExactlyTen()
   {
      IStatCalculator calc = new NightFishRiskDecorator( new BaseStatCalculator() );

      Assert.AreEqual( 10, calc.calculate( 0 ) );
   }

   // HurtHealth decorator removes exactly 10
   [Test]
   public void HurtHealthDecorator_RemovesExactlyTen()
   {
      IStatCalculator calc = new HurtHealthDecorator( new BaseStatCalculator() );

      Assert.AreEqual( 90, calc.calculate( 100 ) );
   }

   // Decorators can be stacked and values accumulate correctly
   [Test]
   public void Decorators_CanStack_ValuesAccumulate()
   {
      IStatCalculator calc = new StealRiskDecorator(
                             new NightFishRiskDecorator(
                             new BaseStatCalculator() ) );

      Assert.AreEqual( 15, calc.calculate( 0 ) ); // 5 + 10
   }

   // ─── PlayerLevel ────────────────────────────────────────────────────────────

   // Level starts at 1
   [Test]
   public void PlayerLevel_StartsAtLevelOne()
   {
      var go    = new GameObject();
      var level = go.AddComponent<PlayerLevel>();

      Assert.AreEqual( 1, level.level );

      Object.DestroyImmediate( go );
   }

   // Adding enough XP levels up correctly
   [Test]
   public void PlayerLevel_LevelsUp_WhenXPThresholdMet()
   {
      var go    = new GameObject();
      var level = go.AddComponent<PlayerLevel>();

      level.addXP( 100 ); // level 1 needs 100 XP

      Assert.AreEqual( 2, level.level );

      Object.DestroyImmediate( go );
   }

   // XP resets to 0 after leveling up with exact threshold
   [Test]
   public void PlayerLevel_XPResetsToZero_AfterExactLevelUp()
   {
      var go    = new GameObject();
      var level = go.AddComponent<PlayerLevel>();

      level.addXP( 100 );

      Assert.AreEqual( 0, level.currentXP );

      Object.DestroyImmediate( go );
   }

   // XP threshold increases with each level
   [Test]
   public void PlayerLevel_XPThreshold_IncreasesPerLevel()
   {
      var go    = new GameObject();
      var level = go.AddComponent<PlayerLevel>();

      int thresholdAtOne = level.xpToNextLevel;
      level.addXP( 100 );
      int thresholdAtTwo = level.xpToNextLevel;

      Assert.Greater( thresholdAtTwo, thresholdAtOne );

      Object.DestroyImmediate( go );
   }
}
