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

        toolbox.toolboxes = new GameObject[]
        {
            new GameObject(),
            new GameObject(),
            new GameObject()
        };

        toolbox.tbShow = toolbox.toolboxes[0];

        // Initialize to Gameplay state
        toolbox.SetGameplayState();
    }

    [Test]
    public void StateTransition_GameplayToToolbox()
    {
        // Assert initial state
        Assert.IsInstanceOf<GameplayState>(toolbox.GetCurrentState());

        // Act
        toolbox.SetToolboxState();

        // Assert
        Assert.IsInstanceOf<ToolboxState>(toolbox.GetCurrentState());
        Assert.AreEqual(0f, Time.timeScale);
    }

    [Test]
    public void Only_One_Toolbox_Active_In_Toolbox_State()
    {
        toolbox.SetToolboxState();

        int activeCount = 0;
        foreach (GameObject tb in toolbox.toolboxes)
        {
            if (tb.activeSelf) activeCount++;
        }

        Assert.AreEqual(1, activeCount);
    }

    [Test]
    public void Stress_Test_State_Toggling()
    {
        for (int i = 0; i < 1000; i++)
        {
            if (toolbox.GetCurrentState() is GameplayState)
                toolbox.SetToolboxState();
            else
                toolbox.SetGameplayState();
        }

        Assert.IsInstanceOf<GameplayState>(toolbox.GetCurrentState());
        Assert.AreEqual(1f, Time.timeScale);
    }
}