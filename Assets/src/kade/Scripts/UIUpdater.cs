using UnityEngine;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
   // Resolved automatically at runtime — do not assign in Inspector
   private HealthStats healthStats;
   private ArrestStats arrestStats;
   private PlayerLevel playerLevel;

   [Header( "Bar Fills" )]
   public Transform healthBarFill;
   public Transform hungerBarFill;
   public Transform riskBarFill;

   [Header( "Labels (optional)" )]
   public Text healthLabel;
   public Text hungerLabel;
   public Text riskLabel;

   [Header( "Level Blocks" )]
   public Transform[] levelBlocks;

   private struct BarData
   {
      public float leftEdge;   // local-space left edge at zero value
      public float scaleX;     // original localScale.x
      public float halfSprite; // sprite half-width in local units
   }

   private BarData mHealth, mHunger, mRisk;
   private float[] mBlockScaleX;
   private int     mCachedHealth, mCachedHunger, mCachedRisk;

   /* Runs when the panel first becomes active — always before OnEnable.
      Safe to cache geometry here since transforms are valid even when disabled. */
   void Awake()
   {
      healthStats = FindAnyObjectByType<HealthStats>();
      arrestStats = FindAnyObjectByType<ArrestStats>();
      playerLevel = FindAnyObjectByType<PlayerLevel>();

      mHealth = cacheBar( healthBarFill );
      mHunger = cacheBar( hungerBarFill );
      mRisk   = cacheBar( riskBarFill );

      cacheBlockScales();

      if ( playerLevel != null ) playerLevel.OnLevelUp += refreshBlocks;
   }

   /* Runs every time the panel opens — reads fresh stat values and redraws.
      Because Awake always precedes OnEnable, no ready-flag is needed. */
   void OnEnable()
   {
      if ( healthStats != null ) { mCachedHealth = healthStats.healthVal; mCachedHunger = healthStats.hungerVal; }
      if ( arrestStats != null )   mCachedRisk   = arrestStats.riskVal;
      if ( playerLevel != null )   refreshBlocks( playerLevel.level );

      refreshUI();
   }

   void OnDestroy()
   {
      if ( playerLevel != null ) playerLevel.OnLevelUp -= refreshBlocks;
   }

   // Polls for stat changes each frame and redraws only when something changed
   void Update()
   {
      bool changed = false;

      if ( healthStats != null )
      {
         if ( healthStats.healthVal != mCachedHealth ) { mCachedHealth = healthStats.healthVal; changed = true; }
         if ( healthStats.hungerVal != mCachedHunger ) { mCachedHunger = healthStats.hungerVal; changed = true; }
      }

      if ( arrestStats != null && arrestStats.riskVal != mCachedRisk ) { mCachedRisk = arrestStats.riskVal; changed = true; }

      if ( changed ) refreshUI();
   }

   // Reads a fill's sprite half-width, original scale, and local right edge for later pinning
   BarData cacheBar( Transform fill )
   {
      if ( fill == null ) return new BarData { leftEdge = 0f, scaleX = 1f, halfSprite = 0.5f };

      var   sr         = fill.GetComponent<SpriteRenderer>();
      float halfSprite = ( sr != null && sr.sprite != null ) ? sr.sprite.bounds.extents.x : 0.5f;
      float scaleX     = fill.localScale.x;

      return new BarData
      {
         scaleX     = scaleX,
         halfSprite = halfSprite,
         leftEdge   = fill.localPosition.x - halfSprite * scaleX
      };
   }

   void refreshUI()
   {
      setBar( healthBarFill, mHealth, healthLabel, mCachedHealth );
      setBar( hungerBarFill, mHunger, hungerLabel, mCachedHunger );
      setBar( riskBarFill,   mRisk,   riskLabel,   mCachedRisk );
   }

   // Scales the fill and pins its left edge; works entirely in local space to avoid drift
   void setBar( Transform fill, BarData data, Text label, int value )
   {
      if ( fill != null )
      {
         float t = Mathf.Clamp01( value / 100f );

         Vector3 s = fill.localScale;
         s.x = data.scaleX * t;
         fill.localScale = s;

         Vector3 p = fill.localPosition;
         p.x = data.leftEdge + data.halfSprite * data.scaleX * t;
         p.z = 0f;
         fill.localPosition = p;
      }

      if ( label != null ) label.text = value.ToString();
   }

   // Records each block's current localScale.x so refreshBlocks can restore it when filling
   void cacheBlockScales()
   {
      if ( levelBlocks == null || levelBlocks.Length == 0 ) return;

      mBlockScaleX = new float[ levelBlocks.Length ];

      for ( int i = 0 ; i < levelBlocks.Length ; i++ )
      {
         if ( levelBlocks[i] == null ) continue;
         mBlockScaleX[i] = levelBlocks[i].localScale.x;
      }
   }

   // Shows or hides each block based on current level
   void refreshBlocks( int currentLevel )
   {
      if ( mBlockScaleX == null ) return;

      for ( int i = 0 ; i < levelBlocks.Length ; i++ )
      {
         if ( levelBlocks[i] == null ) continue;

         Vector3 s = levelBlocks[i].localScale;
         s.x = i < currentLevel ? mBlockScaleX[i] : 0f;
         levelBlocks[i].localScale = s;
      }
   }
}
