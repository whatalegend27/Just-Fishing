// Super Class:    FishCatchReward
// Sub Classes:    CommonFishCatchReward, RareFishCatchReward, LegendaryFishCatchReward
// Virtual Method: GetGold()

public class FishCatchReward
{
    protected int mGold;

    public FishCatchReward()           { mGold = 10; }
    protected FishCatchReward(int gold) { mGold = gold; }

    protected virtual int GetGold() => mGold;
    public int Award() => GetGold();
}

// Common fish — base gold only
public class CommonFishCatchReward : FishCatchReward
{
    public CommonFishCatchReward() : base(10) {}
    protected override int GetGold() => mGold;
}

// Rare fish — base + 15 bonus
public class RareFishCatchReward : FishCatchReward
{
    public RareFishCatchReward() : base(25) {}
    protected override int GetGold() => mGold + 15;
}

// Legendary fish — base + 40 bonus
public class LegendaryFishCatchReward : FishCatchReward
{
    public LegendaryFishCatchReward() : base(50) {}
    protected override int GetGold() => mGold + 40;
}
