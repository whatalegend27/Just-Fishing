using UnityEngine;
using NUnit.Framework;

// Tests for the Upgrade Decorator pattern implementation in the fishing game
public class UpgradeDecoratorEditTests : MonoBehaviour
{
    // Tests that Bait adds attraction value correctly
    [Test]
    public void BaitDecorator_Adds_AttractionValue()
    {
        var bait = ScriptableObject.CreateInstance<ItemScript>();
        bait.attractionValue = 2;

        var baseAttraction = new BaseAttraction();

        var decorator = new BaitDecorator(baseAttraction, bait);

        Assert.AreEqual(2, decorator.GetAttraction());
    }

    // Tests that Lure adds attraction value correctly
    [Test]
    public void LureDecorator_Adds_AttractionValue()
    {
        var lure = ScriptableObject.CreateInstance<ItemScript>();
        lure.attractionValue = 3;

        var baseAttraction = new BaseAttraction();

        var decorator = new LureDecorator(baseAttraction, lure);

        Assert.AreEqual(3, decorator.GetAttraction());
    }

    // Tests that Weight adds attraction value correctly
    [Test]
    public void WeightDecorator_Adds_AttractionValue()
    {
        var weight = ScriptableObject.CreateInstance<ItemScript>();
        weight.attractionValue = 4;

        var baseAttraction = new BaseAttraction();

        var decorator = new WeightDecorator(baseAttraction, weight);

        Assert.AreEqual(4, decorator.GetAttraction());
    }

    // Tests that ComboDecorator does not add bonus if not all are equipped
    [Test]
    public void ComboDecorator_NoBonus_If_Not_All_Equipped()
    {
        var baseAttraction = new BaseAttraction();

        var combo = new ComboDecorator(baseAttraction, true, false, true);

        Assert.AreEqual(0, combo.GetAttraction());
    }

    // Tests that ComboDecorator adds bonus if all are equipped
    [Test]
    public void ComboDecorator_Adds_Bonus_When_All_Equipped()
    {
        var baseAttraction = new BaseAttraction();

        var combo = new ComboDecorator(baseAttraction, true, true, true);

        Assert.AreEqual(1, combo.GetAttraction());
    }

    // Tests that multiple decorators stack correctly
    [Test]
    public void Full_Decorator_Stack_Works()
    {
        var bait = ScriptableObject.CreateInstance<ItemScript>();
        bait.attractionValue = 1;

        var lure = ScriptableObject.CreateInstance<ItemScript>();
        lure.attractionValue = 2;

        var weight = ScriptableObject.CreateInstance<ItemScript>();
        weight.attractionValue = 3;

        IFishAttraction attraction = new BaseAttraction();
        attraction = new BaitDecorator(attraction, bait);
        attraction = new LureDecorator(attraction, lure);
        attraction = new WeightDecorator(attraction, weight);
        attraction = new ComboDecorator(attraction, true, true, true);

        Assert.AreEqual(7 + 1, attraction.GetAttraction());
    }

    // Tests that BaseAttraction returns 0 by default
    [Test]
    public void BaseAttraction_Returns_Zero_By_Default()
    {
        var baseAttraction = new BaseAttraction();

        int result = baseAttraction.GetAttraction();

        Assert.AreEqual(0, result);
    }

    // Tests that ComboDecorator does not add bonus if only one item is equipped
    [Test]
    public void ComboDecorator_NoBonus_With_Single_Item()
    {
        var baseAttraction = new BaseAttraction();

        var combo = new ComboDecorator(baseAttraction, true, false, false);

        Assert.AreEqual(0, combo.GetAttraction());
    }
}
