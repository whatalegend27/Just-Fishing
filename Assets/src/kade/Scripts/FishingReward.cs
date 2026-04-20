using UnityEngine;

// --- Static vs Dynamic Binding Demonstration ---

/* Super Class:    FishingReward
   Sub Class:      RareFishReward (and others below)
   Virtual Method: getXP()

   How to safely remove virtual:
     1. Remove the 'virtual' keyword from getXP() in this class
     2. Remove the 'override' keyword from each subclass
   Result: award() calls FishingReward.getXP() directly (static binding),
           which returns this.mXP — still set correctly per subclass via constructor.
           The game continues to run and award the correct XP. */
public class FishingReward
{
   // mXP is set by each subclass constructor — the safety net when virtual is removed
   protected int mXP;

   public FishingReward()            { mXP = 10; }
   protected FishingReward( int xp ) { mXP = xp; }

   // Protected virtual — subclasses override for dynamic dispatch.
   // If virtual (and override) are removed: this body still runs and returns mXP,
   // which is already correct for every subclass because the constructor set it.
   protected virtual int getXP() => mXP;

   // Public entry point — calls the virtual method so dispatch reaches the right subclass
   public int award() => getXP();
}

// Sub class — common fish, 15 XP
public class CommonFishReward : FishingReward
{
   public CommonFishReward() : base( 15 ) {}
   protected override int getXP() => mXP;
}

// Sub class — rare fish, 25 XP
public class RareFishReward : FishingReward
{
   public RareFishReward() : base( 25 ) {}
   protected override int getXP() => mXP;
}

// Sub class — legendary fish, 50 XP
public class LegendaryFishReward : FishingReward
{
   public LegendaryFishReward() : base( 50 ) {}
   protected override int getXP() => mXP;
}

// --- Binding Demo ---
public class FishingRewardBindingDemo
{
   public static void Run()
   {
      // -----------------------------------------------------------------------
      // DYNAMIC BINDING
      // Static type  = FishingReward   (declared type of the variable)
      // Dynamic type = RareFishReward  (actual runtime object)
      // -----------------------------------------------------------------------
      FishingReward reward = new RareFishReward();

      // getXP is virtual — C# dispatches to the DYNAMIC type at runtime.
      // RareFishReward.getXP() is called → returns mXP (25)
      int xp = reward.award();
      Debug.Log( $"[BindingDemo] Dynamic type RareFishReward — award() returned {xp} (expected 25)" );

      // -----------------------------------------------------------------------
      // CHANGE THE DYNAMIC TYPE
      // Static type stays:   FishingReward
      // Dynamic type is now: LegendaryFishReward
      // -----------------------------------------------------------------------
      reward = new LegendaryFishReward();

      // Dynamic type changed — LegendaryFishReward.getXP() is called now → returns mXP (50)
      xp = reward.award();
      Debug.Log( $"[BindingDemo] Dynamic type LegendaryFishReward — award() returned {xp} (expected 50)" );

      // -----------------------------------------------------------------------
      // STATIC BINDING
      // Static type = FishingReward (concrete, typed directly — no polymorphism)
      // Dynamic type = FishingReward (same)
      // -----------------------------------------------------------------------
      FishingReward staticReward = new FishingReward();

      // Resolved at compile time — FishingReward.getXP() is ALWAYS called → returns mXP (10)
      // This does not change regardless of which dynamic types were used above.
      xp = staticReward.award();
      Debug.Log( $"[BindingDemo] Static binding FishingReward — award() returned {xp} (expected 10)" );
   }
}
