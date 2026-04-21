using NUnit.Framework;
using UnityEngine;

public class ToolboxTests
{
    private HandleToolbox toolbox;
    private GameObject testToolbox;

    [SetUp]
    public void Setup()
    {
        GameObject obj = new GameObject();
        toolbox = obj.AddComponent<HandleToolbox>();

        toolbox.toolboxes = new GameObject[]
        {
            new GameObject("TB1"),
            new GameObject("TB2"),
            new GameObject("TB3")
        };

        testToolbox = toolbox.toolboxes[0];

        // Start in gameplay
        toolbox.SetGameplayState();
    }

    [Test]
    public void StateTransition_GameplayToToolbox()
    {
        Assert.IsInstanceOf<GameplayState>(toolbox.GetCurrentState());

        toolbox.OpenToolbox(testToolbox);

        Assert.IsInstanceOf<ToolboxState>(toolbox.GetCurrentState());
        Assert.AreEqual(0f, Time.timeScale);
    }

    [Test]
    public void Only_One_Toolbox_Active_In_Toolbox_State()
    {
        toolbox.OpenToolbox(testToolbox);

        int activeCount = 0;

        foreach (GameObject tb in toolbox.toolboxes)
        {
            if (tb.activeSelf)
                activeCount++;
        }

        Assert.AreEqual(1, activeCount);
    }

    [Test]
    public void Stress_Test_State_Toggling()
    {
        for (int i = 0; i < 1000; i++)
        {
            if (toolbox.GetCurrentState() is GameplayState)
                toolbox.OpenToolbox(testToolbox);
            else
                toolbox.SetGameplayState();
        }

        Assert.IsInstanceOf<GameplayState>(toolbox.GetCurrentState());
        Assert.AreEqual(1f, Time.timeScale);
    }
}