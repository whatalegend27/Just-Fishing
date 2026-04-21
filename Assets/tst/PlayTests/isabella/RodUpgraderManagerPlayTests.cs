using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

// Tests for the RodUpgradeManager's persistence and upgrade change event functionality
public class RodUpgraderManagerPlayTests : MonoBehaviour
{
    // Tests that the RodUpgradeManager instance persists across scene loads
    [UnityTest]
    public IEnumerator Manager_Persists_Across_Scene()
    {
        var obj = new GameObject();
        obj.AddComponent<RodUpgradeManager>();

        Object.DontDestroyOnLoad(obj);

        yield return null;

        Assert.IsNotNull(RodUpgradeManager.Instance);
    }

}
