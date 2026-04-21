using UnityEngine;

// Bait decorator for the attractionDecorator pattern.
public class BaitDecorator : AttractionDecorator
{
    private ItemScript bait;

    public BaitDecorator(IFishAttraction attraction, ItemScript bait)
        : base(attraction)
    {
        this.bait = bait;
    }

    public override int GetAttraction()
    {
        return base.GetAttraction() + bait.attractionValue;
    }
}

// Lure decorator for the attractionDecorator pattern.
public class LureDecorator : AttractionDecorator
{
    private ItemScript lure;

    public LureDecorator(IFishAttraction attraction, ItemScript lure)
        : base(attraction)
    {
        this.lure = lure;
    }

    public override int GetAttraction()
    {
        return base.GetAttraction() + lure.attractionValue;
    }
}

// Weight decorator for the attractionDecorator pattern.
public class WeightDecorator : AttractionDecorator
{
    private ItemScript weight;

    public WeightDecorator(IFishAttraction attraction, ItemScript weight)
        : base(attraction)
    {
        this.weight = weight;
    }

    public override int GetAttraction()
    {
        return base.GetAttraction() + weight.attractionValue;
    }
}

// Combo decorator that adds a bonus if all three item types are equipped.
public class ComboDecorator : AttractionDecorator
{
    private bool hasLure, hasBait, hasWeight;

    public ComboDecorator(IFishAttraction attraction, bool lure, bool bait, bool weight)
        : base(attraction)
    {
        hasLure = lure;
        hasBait = bait;
        hasWeight = weight;
    }

    // Adds a combo bonus if all three item types are equipped, or just returns the calculated attraction if not.
    public override int GetAttraction()
    {
        int total = base.GetAttraction();

        if (hasLure && hasBait && hasWeight)
        {
            total += 1; 
        }

        return total;
    }
}