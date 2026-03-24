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
        hook.sinkSpeed = 0f; // Set sink speed to 0 so it doesn't move during the test!

        // Unlock logic
        FieldInfo field = typeof(FishingHook).GetField("isReadyToCast", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(hook, false); 

        Debug.Log("<b>[TEST] Starting Vertical Boundary Validation</b>");

        // 2. Max Depth Case (Floor)
        // Force it to -20. Logic should snap it to -10.
        hook.transform.position = new Vector3(0, -20f, 0);
        yield return new WaitForFixedUpdate();
        
        // We check if it is at or above -10
        Assert.GreaterOrEqual(hook.transform.position.y, hook.maxDepth, "Hook is below the seafloor!");

        // 3. Surface Case (Ceiling)
        // Force it to +10. Logic should snap it to 0.
        hook.transform.position = new Vector3(0, 10f, 0);
        yield return new WaitForFixedUpdate();
        
        // We check if it is at or below 0
        Assert.LessOrEqual(hook.transform.position.y, hook.surfaceLevel, "Hook is flying above the water!");

        Debug.Log("<b>TEST COMPLETED</b>");

        Object.Destroy(hookObj);
    }
}