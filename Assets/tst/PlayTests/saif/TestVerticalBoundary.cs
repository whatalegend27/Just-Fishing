using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Saif.GamePlay;
using System.Reflection;

public class TestVerticalBoundary
{
    [UnityTest]
    public IEnumerator TestVerticalClamping()
    {
        // 1. Setup
        GameObject hookObj = new GameObject("Hook");
        var hook = hookObj.AddComponent<FishingHook>();
        hook.maxDepth = -10f;
        hook.surfaceLevel = 0f;
        hook.sinkSpeed = 0f; // Disable automatic sinking for the test

        // --- UPDATED REFLECTION LOGIC ---
        System.Type hookType = typeof(FishingHook);
        
        // Unlock logic: Hook is in the water (not on the boat)
        FieldInfo readyField = hookType.GetField("isReadyToCast", BindingFlags.NonPublic | BindingFlags.Instance);
        readyField.SetValue(hook, false); 

        // Set canReel to false (Hook is in the "sinking" logic path)
        FieldInfo reelField = hookType.GetField("canReel", BindingFlags.NonPublic | BindingFlags.Instance);
        reelField.SetValue(hook, false);

        Debug.Log("<b>[TEST] Starting Vertical Boundary Validation</b>");

        // 2. Max Depth Case (Floor)
        // Force it to -20. The script should clamp it back to -10.
        hook.transform.position = new Vector3(0, -20f, 0);
        yield return new WaitForFixedUpdate();
        
        Assert.GreaterOrEqual(hook.transform.position.y, hook.maxDepth, "Hook is below the seafloor limit!");

        // 3. Surface Case (Ceiling)
        // Force it to +10. The script should clamp it back to 0.
        hook.transform.position = new Vector3(0, 10f, 0);
        yield return new WaitForFixedUpdate();
        
        Assert.LessOrEqual(hook.transform.position.y, hook.surfaceLevel, "Hook is above the water surface!");

        Debug.Log("<b>TEST COMPLETED: Vertical limits are locked.</b>");

        // 4. Cleanup
        Object.Destroy(hookObj);
    }
}