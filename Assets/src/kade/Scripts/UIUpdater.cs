using UnityEngine;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
   [Header( "Stats" )]
   public HealthStats healthStats;
   public ArrestStats arrestStats;
   public PlayerLevel playerLevel;

   [Header( "Bar GameObjects (2D squares in scene)" )]
   public Transform healthBarFill;
   public Transform hungerBarFill;
   public Transform riskBarFill;

   [Header( "Labels (optional)" )]
   public Text healthLabel;
   public Text hungerLabel;
   public Text riskLabel;

   [Header( "Level Blocks" )]
   // Assign all 6 rectangles in order in the Inspector
   public Transform[] levelBlocks;

   private int mCachedHealth;
   private int mCachedHunger;
   private int mCachedRisk;

   private float mHealthBarLeft, mHealthBarScaleX, mHealthBarWorldWidth;
   private float mHungerBarLeft, mHungerBarScaleX, mHungerBarWorldWidth;
   private float mRiskBarLeft,   mRiskBarScaleX,   mRiskBarWorldWidth;

   // Caches bar origins and initial stat values, subscribes to level up event
   void Start()
   {
      cacheBarOrigin( healthBarFill, out mHealthBarLeft, out mHealthBarScaleX, out mHealthBarWorldWidth );
      cacheBarOrigin( hungerBarFill, out mHungerBarLeft, out mHungerBarScaleX, out mHungerBarWorldWidth );
      cacheBarOrigin( riskBarFill,   out mRiskBarLeft,   out mRiskBarScaleX,   out mRiskBarWorldWidth );

      if ( healthStats != null )
      {
         mCachedHealth = healthStats.healthVal;
         mCachedHunger = healthStats.hungerVal;
      }

      if ( arrestStats != null )
      {
         mCachedRisk = arrestStats.riskVal;
      }

      if ( playerLevel != null )
      {
         playerLevel.OnLevelUp += handleLevelUp;
         refreshBlocks( playerLevel.level );
      }

      refreshUI();
   }

   // Unsubscribes from level up event on destroy
   void OnDestroy()
   {
      if ( playerLevel != null )
      {
         playerLevel.OnLevelUp -= handleLevelUp;
      }
   }

   /* Records the bar's left edge, original localScale.x, and original world width.
      Used to pin the left edge when scaling. */
   void cacheBarOrigin( Transform fill, out float leftEdge, out float originalScaleX, out float worldWidth )
   {
      if ( fill != null )
      {
         originalScaleX = fill.localScale.x;
         Renderer r = fill.GetComponent<Renderer>();
         worldWidth = r != null ? r.bounds.size.x : originalScaleX;
         leftEdge   = fill.position.x - worldWidth / 2f;
      }
      else
      {
         originalScaleX = 1f;
         worldWidth     = 1f;
         leftEdge       = 0f;
      }
   }

   // Checks for stat changes each frame and refreshes UI if any are found
   void Update()
   {
      bool changed = false;

      if ( healthStats != null )
      {
         if ( healthStats.healthVal != mCachedHealth )
         {
            mCachedHealth = healthStats.healthVal;
            changed = true;
         }

         if ( healthStats.hungerVal != mCachedHunger )
         {
            mCachedHunger = healthStats.hungerVal;
            changed = true;
         }
      }

      if ( arrestStats != null && arrestStats.riskVal != mCachedRisk )
      {
         mCachedRisk = arrestStats.riskVal;
         changed = true;
      }

      if ( changed )
      {
         refreshUI();
      }
   }

   // Refreshes all three stat bars
   void refreshUI()
   {
      setBar( healthBarFill, mHealthBarLeft, mHealthBarScaleX, mHealthBarWorldWidth, healthLabel, mCachedHealth );
      setBar( hungerBarFill, mHungerBarLeft, mHungerBarScaleX, mHungerBarWorldWidth, hungerLabel, mCachedHunger );
      setBar( riskBarFill,   mRiskBarLeft,   mRiskBarScaleX,   mRiskBarWorldWidth,   riskLabel,   mCachedRisk );
   }

   // Scales the fill rect and pins its left edge based on the stat value
   void setBar( Transform fill, float leftEdge, float originalScaleX, float worldWidth, Text label, int value )
   {
      if ( fill != null )
      {
         float t = Mathf.Clamp01( value / 100f );

         Vector3 s = fill.localScale;
         s.x = originalScaleX * t;
         fill.localScale = s;

         Vector3 p = fill.position;
         p.x = leftEdge + worldWidth * t / 2f;
         fill.position = p;
      }

      if ( label != null )
      {
         label.text = value.ToString();
      }
   }

   // Fills level blocks up to the current level
   void handleLevelUp( int newLevel )
   {
      refreshBlocks( newLevel );
   }

   // Sets each block to filled or empty based on current level
   void refreshBlocks( int currentLevel )
   {
      for ( int i = 0 ; i < levelBlocks.Length ; i++ )
      {
         if ( levelBlocks[i] == null ) continue;

         Vector3 s = levelBlocks[i].localScale;
         s.x = i < currentLevel ? 1f : 0f;
         levelBlocks[i].localScale = s;
      }
   }
}
