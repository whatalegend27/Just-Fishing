using NUnit.Framework;
using UnityEngine;

public class ToolboxTests //Class to handle all of the toolbox feature testing
{
    private HandleToolbox toolbox;

    [SetUp]
    public void Setup() //Setup the toolbox and call the HandleToolbox script.
    {
        GameObject obj = new GameObject();
        toolbox = obj.AddComponent<HandleToolbox>();

        toolbox.Toolboxes = new GameObject[]
        {
            new GameObject(),
            new GameObject(),
            new GameObject()
        };

        toolbox.TbToShow = toolbox.Toolboxes[0]; //start at index 0 toolbox
    }

    [Test]
    //Boundary Test #1
    //Correctly done if the toolbox correctly closes and opens when called. (LEAVING NO OPEN TOOLBOXES).
    public void Toggle_Boundary_OpenClose() 
    {
        Assert.IsFalse(toolbox.TBActive);

        toolbox.ToggleToolbox();
        Assert.IsTrue(toolbox.TBActive);

        toolbox.ToggleToolbox();
        Assert.IsFalse(toolbox.TBActive);
    } //Break point: the toolbox doesn't open or close when called to do so.

    [Test]
    //Boundary Test #2
    // Correctly done if only one toolbox instance appears at a time.
    public void Only_One_Toolbox_Active()
    {
        toolbox.ToggleToolbox();

        int activeCount = 0;
        foreach (GameObject tb in toolbox.Toolboxes) //Go through all toolbox instances
        {
            if (tb.activeSelf) 
            {
                activeCount++;
            }
        }

        Assert.AreEqual(1, activeCount);
    } //Break point: More than one toolbox is active at the same time.

    [Test]
    //Stress Test #1
    //Correctly done if the toolbox can open and close over 1000 times correctly. (Even numbers = closed, Odd numbers = open).
    public void Toggle_Stress_Test()
    {
        for (int i = 0; i < 1000; i++)
        {
            toolbox.ToggleToolbox();
        }

        Assert.IsFalse(toolbox.TBActive);
    } //Break point: Even number of toggles = open toolbox, Odd number of toggles = closed toolbox.
}