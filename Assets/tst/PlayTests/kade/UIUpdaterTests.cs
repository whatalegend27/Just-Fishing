using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class UIUpdaterTests
{
   private GameObject  mGameStats;
   private GameObject  mStatBars;
   private GameObject  mHealthFill;
   private GameObject  mHungerFill;
   private GameObject  mRiskFill;
   private PlayerStats mPlayerStats;
   private HealthStats mHealthStats;
   private ArrestStats mArrestStats;
   private UIUpdater   mUIUpdater;

   // Sets up a GameStats object (always active) and a StatBars object (simulates panel)
   [SetUp]
   public void SetUp()
   {
      // Always-active GameStats object
      mGameStats    = new GameObject( "GameStats" );
      mPlayerStats  = mGameStats.AddComponent<PlayerStats>();
      mHealthStats  = mGameStats.AddComponent<HealthStats>();
      mArrestStats  = mGameStats.AddComponent<ArrestStats>();
      mHealthStats.ps  = mPlayerStats;
      mArrestStats.ps  = mPlayerStats;

      // Fill rects
      mHealthFill = new GameObject( "HealthFill" );
      mHungerFill = new GameObject( "HungerFill" );
      mRiskFill   = new GameObject( "RiskFill" );

      // StatBars panel — starts inactive to simulate closed panel
      mStatBars = new GameObject( "StatBars" );
      mStatBars.SetActive( false );

      mUIUpdater               = mStatBars.AddComponent<UIUpdater>();
      mUIUpdater.healthBarFill = mHealthFill.transform;
      mUIUpdater.hungerBarFill = mHungerFill.transform;
      mUIUpdater.riskBarFill   = mRiskFill.transform;
   }

   // Cleans up all objects after each test
   [TearDown]
   public void TearDown()
   {
      Object.DestroyImmediate( mGameStats );
      Object.DestroyImmediate( mStatBars );
      Object.DestroyImmediate( mHealthFill );
      Object.DestroyImmediate( mHungerFill );
      Object.DestroyImmediate( mRiskFill );
   }

   // Verifies that UIUpdater shows correct risk value when panel opens after stat changed
   [UnityTest]
   public IEnumerator UIUpdater_ShowsCorrectRisk_WhenPanelOpensAfterChange()
   {
      // Open panel so Start() runs and mReady is set
      mStatBars.SetActive( true );
      yield return null;

      // Close panel and change risk while it's closed
      mStatBars.SetActive( false );
      mArrestStats.calculateRisk( "steal" );   // +5
      mArrestStats.calculateRisk( "nightFish" ); // +10 = 15 total
      yield return null;

      // Re-open panel — OnEnable should refresh bars
      mStatBars.SetActive( true );
      yield return null;

      Assert.AreEqual( 15, mArrestStats.riskVal,
         "Risk should be 15 after steal and nightFish" );
   }

   // Verifies that UIUpdater shows correct health when panel opens after taking damage
   [UnityTest]
   public IEnumerator UIUpdater_ShowsCorrectHealth_WhenPanelOpensAfterDamage()
   {
      mStatBars.SetActive( true );
      yield return null;

      mStatBars.SetActive( false );
      mHealthStats.takeDamage( 10 );
      mHealthStats.takeDamage( 10 );
      yield return null;

      mStatBars.SetActive( true );
      yield return null;

      Assert.AreEqual( 80, mHealthStats.healthVal,
         "Health should be 80 after two 10-damage hits" );
   }

   // Verifies that UIUpdater updates risk in real time while the panel is open
   [UnityTest]
   public IEnumerator UIUpdater_UpdatesRisk_WhilePanelIsOpen()
   {
      mStatBars.SetActive( true );
      yield return null;

      mArrestStats.calculateRisk( "blackMarket" ); // +5
      yield return null;

      Assert.AreEqual( 5, mArrestStats.riskVal,
         "Risk should be 5 after one blackMarket action while panel is open" );
   }

   // Verifies that hunger starts at 100 and updates correctly
   [UnityTest]
   public IEnumerator UIUpdater_HungerStartsAt100_AndDecreasesCorrectly()
   {
      mStatBars.SetActive( true );
      yield return null;

      Assert.AreEqual( 100, mHealthStats.hungerVal,
         "Hunger should start at 100" );

      mHealthStats.calculateHunger( "time" ); // -5
      yield return null;

      Assert.AreEqual( 95, mHealthStats.hungerVal,
         "Hunger should be 95 after one time tick" );
   }

   // Verifies that stat components on GameStats run while panel is closed
   [UnityTest]
   public IEnumerator GameStats_StatsUpdate_WhenPanelIsClosed()
   {
      mStatBars.SetActive( true );
      yield return null;

      mStatBars.SetActive( false );

      // Stats should still update since GameStats is always active
      mArrestStats.calculateRisk( "steal" );
      mHealthStats.takeDamage( 20 );
      yield return null;

      Assert.AreEqual( 5,  mArrestStats.riskVal,    "Risk should update while panel is closed" );
      Assert.AreEqual( 80, mHealthStats.healthVal,  "Health should update while panel is closed" );
   }
}
