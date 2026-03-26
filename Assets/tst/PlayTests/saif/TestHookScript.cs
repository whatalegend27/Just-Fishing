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
        
        // --- REFLECTION UNLOCKS ---
        // We need to set the hook to: isReadyToCast = false AND isReeling = false
        // This simulates the hook being "In the water" and "Sinking"
        System.Type hookType = typeof(FishingHook);
        
        FieldInfo readyField = hookType.GetField("isReadyToCast", BindingFlags.NonPublic | BindingFlags.Instance);
        readyField.SetValue(hook, false);

        FieldInfo reelingField = hookType.GetField("isReeling", BindingFlags.NonPublic | BindingFlags.Instance);
        reelingField.SetValue(hook, false);

        // Add 2D Physics so it can actually collide
        hookObj.AddComponent<BoxCollider2D>().isTrigger = true;
        Rigidbody2D rb = hookObj.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        // 2. Setup Fish Prefab (Cube base)
        GameObject fishPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fishPrefab.tag = "Fish";
        Object.DestroyImmediate(fishPrefab.GetComponent<BoxCollider>());

        Debug.Log("<b>[TEST] Starting Hook Stress Validation (50 Fish)</b>");

        // 3. Action: Spawn 50 fish exactly on the hook
        for (int i = 0; i < 50; i++)
        {
            GameObject f = Object.Instantiate(fishPrefab, hook.transform.position, Quaternion.identity);
            f.tag = "Fish"; 
            
            f.AddComponent<BoxCollider2D>().isTrigger = true;
            Rigidbody2D fishRb = f.AddComponent<Rigidbody2D>();
            fishRb.bodyType = RigidbodyType2D.Kinematic;
        }

        // 4. Wait for physics processing
        yield return new WaitForSeconds(0.5f); 

        // 5. Validation
        int caughtCount = hook.transform.childCount;
        
        // If your code works, the first fish caught sets hasCaughtFish = true, 
        // preventing the other 49 from attaching.
        Assert.AreEqual(1, caughtCount, $"Multi-catch Bug: {caughtCount} fish caught instead of 1!");

        Debug.Log("<b>TEST COMPLETED: Hook only caught one fish.</b>");
        
        // Cleanup
        Object.Destroy(hookObj);
        Object.Destroy(fishPrefab);
    }
}