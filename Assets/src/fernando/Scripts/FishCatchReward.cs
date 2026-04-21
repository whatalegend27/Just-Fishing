// Super Class:    FishCatchReward
// Sub Classes:    CommonFishCatchReward, RareFishCatchReward, LegendaryFishCatchReward
// Virtual Method: GetGold()

/* Static vs Dynamic Binding Demo
   Static type is always FishCatchReward (the declared variable type — never changes).
   Dynamic type is set at runtime depending on fish rarity.

   Setting static and dynamic type:
      FishCatchReward reward = new RareFishCatchReward();
      // static type  = FishCatchReward
      // dynamic type = RareFishCatchReward  →  GetGold() returns mGold + 15

      reward = new LegendaryFishCatchReward();
      // static type  = FishCatchReward  (unchanged)
      // dynamic type = LegendaryFishCatchReward  →  GetGold() returns mGold + 40

   Dynamically bound — GetGold() is virtual:
      reward = new RareFishCatchReward()      →  RareFishCatchReward.GetGold()      called
      reward = new LegendaryFishCatchReward() →  LegendaryFishCatchReward.GetGold() called

   Statically bound — Award() is NOT virtual:
      In both cases above, Award() always resolves to FishCatchReward.Award().
      Award() then internally calls GetGold(), where dynamic dispatch takes over. */

public class FishCatchReward
{
    protected int mGold;

    public FishCatchReward()           { mGold = 10; }
    protected FishCatchReward(int gold) { mGold = gold; }

    // Returns the gold amount; overridden by subclasses to add bonuses
    protected virtual int GetGold() => mGold;
    // Statically bound — always resolves to this base class method regardless of dynamic type
    public int Award() => GetGold();
}

// Common fish — base gold only
public class CommonFishCatchReward : FishCatchReward
{
    public CommonFishCatchReward() : base(10) {}
    // Returns base gold with no bonus
    protected override int GetGold() => mGold;
}

// Rare fish — base + 15 bonus
public class RareFishCatchReward : FishCatchReward
{
    public RareFishCatchReward() : base(25) {}
    // Returns base gold plus 15 rare bonus
    protected override int GetGold() => mGold + 15;
}

// Legendary fish — base + 40 bonus
public class LegendaryFishCatchReward : FishCatchReward
{
    public LegendaryFishCatchReward() : base(50) {}
    // Returns base gold plus 40 legendary bonus
    protected override int GetGold() => mGold + 40;
}
