// Super Class:    FishCatchReward
// Sub Classes:    CommonFishCatchReward, RareFishCatchReward, LegendaryFishCatchReward
// Virtual Method: GetGold()

public class FishCatchReward
{
    protected int mGold;

    public FishCatchReward()           { mGold = 10; }
    protected FishCatchReward(int gold) { mGold = gold; }

    // Returns the gold amount; overridden by subclasses to add bonuses
    protected virtual int GetGold() => mGold;
    // Computes and returns the final gold reward
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
