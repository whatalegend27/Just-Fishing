using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Saif.GamePlay;
using System.Reflection;

public class TestHorizontalBoundary
{
    [UnityTest]
    public IEnumerator TestHorizontalClamping()
    {
        // 1. Setup: Create the hook
        GameObject hookObj = new GameObject("Hook");
        var hook = hookObj.AddComponent<FishingHook>();
        
        // Set the borders
        hook.leftBorder = -8f;
        hook.rightBorder = 8f;

        // --- NEW REFLECTION LOGIC ---
        System.Type hookType = typeof(FishingHook);
        
        // Set isReadyToCast to false (Hook is in the water)
        FieldInfo readyField = hookType.GetField("isReadyToCast", BindingFlags.NonPublic | BindingFlags.Instance);
        readyField.SetValue(hook, false); 

        // Set isReeling to false (Hook is sinking/moving normally)
        FieldInfo reelingField = hookType.GetField("isReeling", BindingFlags.NonPublic | BindingFlags.Instance);
        reelingField.SetValue(hook, false);

        Debug.Log("<b>[TEST] Starting Horizontal Boundary Validation</b>");

        // 2. Test Right Side
        // Force position to 15. The script should clamp it to 8.
        hook.transform.position = new Vector3(15f, 0, 0);
        yield return new WaitForFixedUpdate(); 
        
        Assert.AreEqual(8f, hook.transform.position.x, "Hook failed to snap to Right Border (8)!");

        // 3. Test Left Side
        // Force position to -15. The script should clamp it to -8.
        hook.transform.position = new Vector3(-15f, 0, 0);
        yield return new WaitForFixedUpdate();
        
        Assert.AreEqual(-8f, hook.transform.position.x, "Hook failed to snap to Left Border (-8)!");

        Debug.Log("<b>TEST COMPLETED: Horizontal boundaries are solid.</b>");

        // 4. Cleanup
        Object.Destroy(hookObj);
    }
}