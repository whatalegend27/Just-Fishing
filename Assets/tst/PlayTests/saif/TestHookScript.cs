using UnityEngine;
using UnityEngine.TestTools; 
using NUnit.Framework;      
using System.Collections;
using Saif.GamePlay;
using System.Reflection;

public class TestHookScript 
{
    [UnityTest] 
    public IEnumerator TestHookStress()
    {
        // 1. Setup Hook
        GameObject hookObj = new GameObject("Hook");
        var hook = hookObj.AddComponent<FishingHook>();
        
        // Unlock the logic
        FieldInfo field = typeof(FishingHook).GetField("isReadyToCast", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(hook, false);

        // Add 2D Physics to the Hook so it can detect things
        hookObj.AddComponent<BoxCollider2D>().isTrigger = true;
        hookObj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        // 2. Setup Fish Prefab
        GameObject fishPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fishPrefab.tag = "Fish";
        
        Object.DestroyImmediate(fishPrefab.GetComponent<BoxCollider>());

        Debug.Log("<b>[TEST] Starting Hook Stress Validation</b>");

        // 3. Action: Spawn 50 fish
        for (int i = 0; i < 50; i++)
        {
            GameObject f = Object.Instantiate(fishPrefab, hook.transform.position, Quaternion.identity);
            f.tag = "Fish"; 
            
            // Now adding the 2D collider will work perfectly
            f.AddComponent<BoxCollider2D>().isTrigger = true;
            f.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }

        // 4. Wait for physics
        yield return new WaitForSeconds(0.5f); 

        // 5. Validation
        int caughtCount = hook.transform.childCount;
        
        Assert.AreEqual(1, caughtCount, $"Multi-catch Bug: {caughtCount} fish caught instead of 1!");

        Debug.Log("<b>TEST COMPLETED</b>");
        
        // Cleanup
        Object.Destroy(hookObj);
        Object.Destroy(fishPrefab);
    }
}