using NUnit.Framework;
using UnityEngine;

public class ToolboxTests
{
    private HandleToolbox toolbox;

    [SetUp]
    public void Setup()
    {
        GameObject obj = new GameObject();
        toolbox = obj.AddComponent<HandleToolbox>();

        toolbox.Toolboxes = new GameObject[]
        {
            new GameObject(),
            new GameObject(),
            new GameObject()
        };

        toolbox.TbToShow = toolbox.Toolboxes[0];
    }

    [Test]
    //Boundary Test #1
    public void Toggle_Boundary_OpenClose()
    {
        Assert.IsFalse(toolbox.TBActive);

        toolbox.ToggleToolbox();
        Assert.IsTrue(toolbox.TBActive);

        toolbox.ToggleToolbox();
        Assert.IsFalse(toolbox.TBActive);
    }

    [Test]
    //Boundary Test #2
    public void Only_One_Toolbox_Active()
    {
        toolbox.ToggleToolbox();

        int activeCount = 0;
        foreach (GameObject tb in toolbox.Toolboxes)
        {
            if (tb.activeSelf) activeCount++;
        }

        Assert.AreEqual(1, activeCount);
    }

    [Test]
    //Stress Test #1
    public void Toggle_Stress_Test()
    {
        for (int i = 0; i < 1000; i++)
        {
            toolbox.ToggleToolbox();
        }

        Assert.IsFalse(toolbox.TBActive);
    }
}