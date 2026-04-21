// TestHookScript.cs
// Saif Badwan
// 12 tests covering: fish catching limits, dynamic binding proof,
// non-fish rejection, hook reset, IsHookCast property, and stress cases.

using UnityEngine; // Standard Unity engine library
using UnityEngine.TestTools; // Library for Unity PlayMode tests
using NUnit.Framework; // The logic behind Assert (Pass/Fail) checks
using System.Collections; // Required for IEnumerators (Tests that take time)
using Saif.GamePlay; // Accessing the custom fishing logic
using System.Reflection; // Required to access private variables (Reflection pattern)

public class TestHookScript
{
    // ─── HELPERS ──────────────────────────────────────────────────────────────────

    // Sets any private field on FishingHook via reflection so we don't need to
    // expose things publicly just for testing (Encapsulation proof)
    private void SetPrivateField(FishingHook hook, string fieldName, object value)
    {
        // Search the class's non-public memory for a variable name
        FieldInfo field = typeof(FishingHook).GetField(fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        // Safety check to ensure the test doesn't crash if names change
        Assert.IsNotNull(field, $"Field '{fieldName}' not found — did the variable name change?");
        // Manually inject the test value into the private variable
        field.SetValue(hook, value);
    }

    // Creates a plain FishingHook (SmallHook behaviour — catches 1 fish)
    private FishingHook CreateSmallHook()
    {
        GameObject hookObj = new GameObject("Hook"); // Spawns a dummy object
        FishingHook hook = hookObj.AddComponent<FishingHook>(); // Attaches script

        SetPrivateField(hook, "debugMode", true); // Bypasses player search logic
        SetPrivateField(hook, "isReadyToCast", false); // Forces hook into "in-water" state
        SetPrivateField(hook, "canReel", false); // Prevents hook from flying back up
        SetPrivateField(hook, "animationDelayDone", true); // Skips the wait timer

        hookObj.AddComponent<BoxCollider2D>().isTrigger = true; // Adds the trigger area
        hookObj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; // Allows trigger events
        return hook; // Return the prepared test subject
    }

    // Creates a HeavyHook (catches 2 fish via dynamic binding override)
    private HeavyHook CreateHeavyHook()
    {
        GameObject hookObj = new GameObject("HeavyHook"); // Spawns object
        HeavyHook hook = hookObj.AddComponent<HeavyHook>(); // Attaches child class

        SetPrivateField(hook, "debugMode", true); // Same setup as SmallHook
        SetPrivateField(hook, "isReadyToCast", false);
        SetPrivateField(hook, "canReel", false);
        SetPrivateField(hook, "animationDelayDone", true);

        hookObj.AddComponent<BoxCollider2D>().isTrigger = true;
        hookObj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        return hook;
    }

    // Spawns a fish GameObject at the given position with a trigger collider
    private GameObject SpawnFish(Vector3 position)
    {
        GameObject fish = new GameObject("Fish"); // Create fish
        fish.tag = "Fish"; // IMPORTANT: This is the tag the script checks for
        fish.transform.position = position; // Put it on top of the hook
        fish.AddComponent<BoxCollider2D>().isTrigger = true; // Make it a trigger
        fish.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; // Enable physics
        return fish;
    }

    // Destroys every GameObject with a given name — keeps test scene clean between runs
    private void DestroyAllNamed(string name)
    {
        // Loop through all objects in the test scene
        foreach (GameObject obj in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            if (obj.name == name) Object.DestroyImmediate(obj); // Remove them instantly
    }

    // ─── TEST 1: SMALL HOOK CATCHES EXACTLY 1 FISH ───────────────────────────────
    /* ORAL EXAM ANSWER: "This test caught the bug! It spawned 50 fish but caught 0. 
       This told me the hook was no longer 'seeing' the fish. I knew the hook logic 
       wasn't broken because of Test 3, so I knew the fish tag was the problem."
    */
    [UnityTest]
    public IEnumerator SmallHook_OnlyCatchesOneFish()
    {
        SimulationMode2D originalMode = Physics2D.simulationMode; // Save the default physics setting
        Physics2D.simulationMode = SimulationMode2D.Script; // Switch to Manual Control mode

        FishingHook hook = CreateSmallHook(); // Make the hook
        for (int i = 0; i < 50; i++) SpawnFish(hook.transform.position); // Spawn 50 tagged fish

        Physics2D.Simulate(0.02f); // Force the physics engine to calculate the collision NOW
        yield return new WaitForSeconds(0.1f); // Wait for the code's internal logic to finish

        Assert.AreEqual(1, hook.transform.childCount, // Verify we have exactly 1 child
            $"SmallHook caught {hook.transform.childCount} fish — should be exactly 1!");

        Debug.Log("<b>[TEST 1] PASSED — SmallHook caught 1 fish from 50 spawned</b>");
        
        Physics2D.simulationMode = originalMode; // Restore physics to automatic
        Object.DestroyImmediate(hook.gameObject); // Cleanup
        DestroyAllNamed("Fish"); // Cleanup
    }

    // ─── TEST 2: HEAVY HOOK CATCHES EXACTLY 2 FISH ───────────────────────────────
    [UnityTest]
    public IEnumerator HeavyHook_OnlyCatchesTwoFish()
    {
        SimulationMode2D originalMode = Physics2D.simulationMode; // Setup manual physics
        Physics2D.simulationMode = SimulationMode2D.Script;

        HeavyHook hook = CreateHeavyHook(); // Create the Heavy variant
        for (int i = 0; i < 50; i++) SpawnFish(hook.transform.position); // Spawn 50 fish

        Physics2D.Simulate(0.02f); // Trigger collisions
        yield return new WaitForSeconds(0.1f); // Wait for processing

        Assert.AreEqual(2, hook.transform.childCount, // Proof of override: should be 2
            $"HeavyHook caught {hook.transform.childCount} fish — should be exactly 2!");

        Debug.Log("<b>[TEST 2] PASSED — HeavyHook caught 2 fish from 50 spawned</b>");
        
        Physics2D.simulationMode = originalMode; // Reset physics
        Object.DestroyImmediate(hook.gameObject); // Cleanup
        DestroyAllNamed("Fish");
    }

    // ─── TEST 3: SMALL HOOK IGNORES NON-FISH OBJECTS ─────────────────────────────
    /* ORAL EXAM ANSWER: "This test was my diagnostic tool. It checks that untagged 
       objects are ignored. Because this test was PASSING while Test 1 was failing, 
       I proved my 'Tag Filter' code was working perfectly. This helped me identify 
       that the error was the fish asset itself, not my script."
    */
    [UnityTest]
    public IEnumerator SmallHook_IgnoresNonFishObjects()
    {
        SimulationMode2D originalMode = Physics2D.simulationMode; // Setup manual physics
        Physics2D.simulationMode = SimulationMode2D.Script;

        FishingHook hook = CreateSmallHook(); // Create hook

        for (int i = 0; i < 20; i++) // Spawn 20 objects WITHOUT the "Fish" tag
        {
            GameObject obj = new GameObject("NotAFish"); 
            obj.transform.position = hook.transform.position; 
            obj.AddComponent<BoxCollider2D>().isTrigger = true; 
            obj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }

        Physics2D.Simulate(0.02f); // Trigger collisions
        yield return new WaitForSeconds(0.1f); // Process logic

        Assert.AreEqual(0, hook.transform.childCount, // Verify zero were caught
            $"Hook caught {hook.transform.childCount} non-fish objects — tag check broken!");

        Debug.Log("<b>[TEST 3] PASSED — Hook correctly ignored all non-fish objects</b>");
        
        Physics2D.simulationMode = originalMode; // Restore physics
        Object.DestroyImmediate(hook.gameObject); // Cleanup
        DestroyAllNamed("NotAFish");
    }

    // ─── TEST 4: HEAVY HOOK IGNORES NON-FISH OBJECTS ─────────────────────────────
    [UnityTest]
    public IEnumerator HeavyHook_IgnoresNonFishObjects()
    {
        SimulationMode2D originalMode = Physics2D.simulationMode; // Physics setup
        Physics2D.simulationMode = SimulationMode2D.Script;

        HeavyHook hook = CreateHeavyHook(); // Create heavy hook

        for (int i = 0; i < 20; i++) // Spawn 20 untagged objects
        {
            GameObject obj = new GameObject("NotAFish");
            obj.transform.position = hook.transform.position;
            obj.AddComponent<BoxCollider2D>().isTrigger = true;
            obj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }

        Physics2D.Simulate(0.02f); // Trigger physics
        yield return new WaitForSeconds(0.1f);

        Assert.AreEqual(0, hook.transform.childCount, // Should be zero
            $"HeavyHook caught {hook.transform.childCount} non-fish — tag check broken!");

        Debug.Log("<b>[TEST 4] PASSED — HeavyHook correctly ignored all non-fish objects</b>");
        
        Physics2D.simulationMode = originalMode; // Reset physics
        Object.DestroyImmediate(hook.gameObject);
        DestroyAllNamed("NotAFish");
    }

    // ─── TEST 5: ISHOOKCAST IS TRUE WHEN CAST ────────────────────────────────────
    [UnityTest]
    public IEnumerator Hook_IsHookCast_TrueWhenInWater()
    {
        FishingHook hook = CreateSmallHook(); // Hook is spawned "in water"
        Assert.IsTrue(hook.IsHookCast, "IsHookCast should be TRUE when hook is in the water!"); // Verify logic

        Debug.Log("<b>[TEST 5] PASSED — IsHookCast correctly reports TRUE when cast</b>");
        yield return null; // Finish frame
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 6: ISHOOKCAST IS FALSE WHEN READY ──────────────────────────────────
    [UnityTest]
    public IEnumerator Hook_IsHookCast_FalseWhenReady()
    {
        FishingHook hook = CreateSmallHook(); // Create hook
        SetPrivateField(hook, "isReadyToCast", true); // Force state to "ready at rod"

        Assert.IsFalse(hook.IsHookCast, "IsHookCast should be FALSE when hook is ready to cast!"); // Verify logic

        Debug.Log("<b>[TEST 6] PASSED — IsHookCast correctly reports FALSE when ready</b>");
        yield return null;
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 7: DYNAMIC BINDING PROOF — SMALL VS HEAVY FISH COUNT ───────────────
    [UnityTest]
    public IEnumerator DynamicBinding_SmallAndHeavyHookBehaveDifferently()
    {
        SimulationMode2D originalMode = Physics2D.simulationMode; // Physics setup
        Physics2D.simulationMode = SimulationMode2D.Script;

        FishingHook smallHook = CreateSmallHook(); // Parent type
        smallHook.transform.position = new Vector3(-5f, 0f, 0f);

        HeavyHook heavyHook = CreateHeavyHook(); // Child type
        heavyHook.transform.position = new Vector3(5f, 0f, 0f);

        for (int i = 0; i < 10; i++) SpawnFish(smallHook.transform.position); // Fish for small
        for (int i = 0; i < 10; i++) SpawnFish(heavyHook.transform.position); // Fish for heavy

        Physics2D.Simulate(0.02f); // Trigger collisions
        yield return new WaitForSeconds(0.2f); // Process logic

        // This line proves that the computer ran two different versions of "AttachFish"
        Assert.AreEqual(1, smallHook.transform.childCount, "SmallHook should have 1 fish");
        Assert.AreEqual(2, heavyHook.transform.childCount, "HeavyHook should have 2 fish");

        Debug.Log("<b>[TEST 7] PASSED — Dynamic binding confirmed: Small=1 fish, Heavy=2 fish</b>");
        
        Physics2D.simulationMode = originalMode; // Reset physics
        Object.DestroyImmediate(smallHook.gameObject);
        Object.DestroyImmediate(heavyHook.gameObject);
        DestroyAllNamed("Fish");
    }

    // ─── TEST 8: SMALL HOOK CATCHES 1 FISH THEN IGNORES MORE ─────────────────────
    [UnityTest]
    public IEnumerator SmallHook_CatchesFirstFishThenIgnoresRest()
    {
        SimulationMode2D originalMode = Physics2D.simulationMode; // Setup manual physics
        Physics2D.simulationMode = SimulationMode2D.Script;

        FishingHook hook = CreateSmallHook(); // Create hook

        SpawnFish(hook.transform.position); // First fish
        Physics2D.Simulate(0.02f); // Collide
        yield return new WaitForSeconds(0.1f);

        Assert.AreEqual(1, hook.transform.childCount, "Should have caught the first fish!");

        for (int i = 0; i < 5; i++) SpawnFish(hook.transform.position); // Try to catch 5 more
        Physics2D.Simulate(0.02f); // Collide
        yield return new WaitForSeconds(0.1f);

        Assert.AreEqual(1, hook.transform.childCount, "Should NOT have caught more fish!"); // Verify limit

        Debug.Log("<b>[TEST 8] PASSED — SmallHook correctly ignored fish after slot was full</b>");
        
        Physics2D.simulationMode = originalMode; // Reset physics
        Object.DestroyImmediate(hook.gameObject);
        DestroyAllNamed("Fish");
    }

    // ─── TEST 9: HEAVY HOOK FILLS SLOT 1 THEN SLOT 2 ────────────────────────────
    [UnityTest]
    public IEnumerator HeavyHook_FillsBothSlotsSequentially()
    {
        SimulationMode2D originalMode = Physics2D.simulationMode; // Setup physics
        Physics2D.simulationMode = SimulationMode2D.Script;

        HeavyHook hook = CreateHeavyHook(); // Create heavy hook

        SpawnFish(hook.transform.position); // Catch one
        Physics2D.Simulate(0.02f);
        yield return new WaitForSeconds(0.1f);
        Assert.AreEqual(1, hook.transform.childCount, "Slot 1 filled"); // Check first

        SpawnFish(hook.transform.position); // Catch second
        Physics2D.Simulate(0.02f);
        yield return new WaitForSeconds(0.1f);
        Assert.AreEqual(2, hook.transform.childCount, "Slot 2 filled"); // Check second

        Debug.Log("<b>[TEST 9] PASSED — HeavyHook filled both slots then correctly stopped</b>");
        
        Physics2D.simulationMode = originalMode; // Reset physics
        Object.DestroyImmediate(hook.gameObject);
        DestroyAllNamed("Fish");
    }

    // ─── TEST 10: MIXED OBJECTS — FISH AND NON-FISH TOGETHER ─────────────────────
    [UnityTest]
    public IEnumerator SmallHook_MixedObjects_OnlyCatchesFish()
    {
        SimulationMode2D originalMode = Physics2D.simulationMode; // Physics setup
        Physics2D.simulationMode = SimulationMode2D.Script;

        FishingHook hook = CreateSmallHook(); // Create hook

        for (int i = 0; i < 10; i++) // Spawn 10 junk objects
        {
            GameObject obj = new GameObject("NotAFish");
            obj.transform.position = hook.transform.position;
            obj.AddComponent<BoxCollider2D>().isTrigger = true;
            obj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }

        for (int i = 0; i < 10; i++) SpawnFish(hook.transform.position); // Spawn 10 real fish

        Physics2D.Simulate(0.02f); // Trigger collisions
        yield return new WaitForSeconds(0.2f);

        Assert.AreEqual(1, hook.transform.childCount, "Expected 1 caught"); // Verify limit
        Assert.AreEqual("Fish", hook.transform.GetChild(0).tag, "Must be tagged Fish"); // Verify tag filtering

        Debug.Log("<b>[TEST 10] PASSED — Hook correctly caught only 1 tagged fish among mixed objects</b>");
        
        Physics2D.simulationMode = originalMode; // Reset physics
        Object.DestroyImmediate(hook.gameObject);
        DestroyAllNamed("Fish");
        DestroyAllNamed("NotAFish");
    }

    // ─── TEST 11: SINGLE FISH CAUGHT BECOMES CHILD OF HOOK ───────────────────────
    [UnityTest]
    public IEnumerator SmallHook_CaughtFishBecomesChildOfHook()
    {
        SimulationMode2D originalMode = Physics2D.simulationMode; // Setup physics
        Physics2D.simulationMode = SimulationMode2D.Script;

        FishingHook hook = CreateSmallHook(); // Create hook
        GameObject fish = SpawnFish(hook.transform.position); // Spawn fish

        Physics2D.Simulate(0.02f); // Trigger collision
        yield return new WaitForSeconds(0.1f);

        Assert.AreEqual(hook.transform, fish.transform.parent, "Fish should be parented"); // Verify parenting

        Debug.Log("<b>[TEST 11] PASSED — Caught fish is correctly parented to hook</b>");
        
        Physics2D.simulationMode = originalMode; // Reset physics
        Object.DestroyImmediate(hook.gameObject);
        DestroyAllNamed("Fish");
    }

    // ─── TEST 12: HEAVY HOOK STRESS — 100 FISH, NEVER EXCEEDS 2 ─────────────────
    [UnityTest]
    public IEnumerator HeavyHook_StressTest_NeverExceedsTwoFish()
    {
        SimulationMode2D originalMode = Physics2D.simulationMode; // Physics setup
        Physics2D.simulationMode = SimulationMode2D.Script;

        HeavyHook hook = CreateHeavyHook(); // Create heavy hook
        for (int i = 0; i < 100; i++) SpawnFish(hook.transform.position); // Spawn a massive amount of fish

        Physics2D.Simulate(0.02f); // Trigger mass collisions
        yield return new WaitForSeconds(0.3f);

        Assert.LessOrEqual(hook.transform.childCount, 2, "HeavyHook exceeded limit!"); // Stress limit check

        Debug.Log($"<b>[TEST 12] PASSED — HeavyHook held at {hook.transform.childCount} fish under 100-fish stress</b>");
        
        Physics2D.simulationMode = originalMode; // Reset physics
        Object.DestroyImmediate(hook.gameObject);
        DestroyAllNamed("Fish");
    }
}