using UnityEngine;
using NUnit.Framework;

// Tests for the SharkHandler's dialogue management and Animator parameter setting
public class SharkHandlerEditTests : MonoBehaviour
{
    // Test that clicking the Flirt button shows the flirt dialogue and sets the Animator parameter
    [Test]
    public void Flirt_Click_Shows_FlirtDialogue_And_Sets_Animator()
    {
        var obj = new GameObject();
        var shark = obj.AddComponent<SharkHandler>();

        var animator = obj.AddComponent<Animator>();
        var choice = new GameObject();
        var flirt = new GameObject();

        typeof(SharkHandler)
            .GetField("animator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(shark, animator);

        typeof(SharkHandler)
            .GetField("ChoiceDialogue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(shark, choice);

        typeof(SharkHandler)
            .GetField("FlirtDialogue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(shark, flirt);

        shark.OnFlirtClicked();

        Assert.IsFalse(choice.activeSelf);
        Assert.IsTrue(flirt.activeSelf);
    }

    // Test that clicking the Insult button shows the insult dialogue and sets the Animator parameter
    [Test]
    public void Insult_Click_Shows_InsultDialogue()
    {
        var obj = new GameObject();
        var shark = obj.AddComponent<SharkHandler>();

        var animator = obj.AddComponent<Animator>();
        var choice = new GameObject();
        var insult = new GameObject();

        typeof(SharkHandler)
            .GetField("animator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(shark, animator);

        typeof(SharkHandler)
            .GetField("ChoiceDialogue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(shark, choice);

        typeof(SharkHandler)
            .GetField("InsultDialogue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(shark, insult);

        shark.OnInsultClicked();

        Assert.IsFalse(choice.activeSelf);
        Assert.IsTrue(insult.activeSelf);
    }
}
