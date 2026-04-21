// TestVerticalBoundary.cs
// Saif Badwan
// 9 tests covering: max depth clamp, surface clamp, multi-frame persistence,
// sinking to floor and stopping, custom depth values, extreme positions,
// hook starting at valid Y, zero depth range, and slow sink speed accuracy.

using UnityEngine; // Standard Unity engine library
using UnityEngine.TestTools; // Library for PlayMode testing
using NUnit.Framework; // Library for Assertions (Pass/Fail checks)
using System.Collections; // Required for IEnumerators/Coroutines
using Saif.GamePlay; // Access your fishing game logic
using System.Reflection; // Allows "Reflection" to modify private variables for testing

public class TestVerticalBoundary
{
    // ─── HELPER ──────────────────────────────────────────────────────────────────

    // Sets any private field on FishingHook via reflection (Encapsulation proof)
    private void SetPrivateField(FishingHook hook, string fieldName, object value)
    {
        // Finds the hidden field in memory
        FieldInfo field = typeof(FishingHook).GetField(fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        // Error check to ensure the variable name hasn't changed
        Assert.IsNotNull(field, $"Field '{fieldName}' not found — did the variable name change?");
        // Manually injects the value into the script
        field.SetValue(hook, value);
    }

    // Creates a hook ready for vertical boundary testing.
    private FishingHook CreateHook(float maxDepth = -10f, float sinkSpeed = 0f)
    {
        GameObject hookObj = new GameObject("Hook"); // Create a dummy GameObject
        FishingHook hook = hookObj.AddComponent<FishingHook>(); // Add the hook script
        hook.enabled = false; // Disable temporarily to set up variables safely

        SetPrivateField(hook, "debugMode", true); // Skip searching for a real player
        SetPrivateField(hook, "maxDepth", maxDepth); // Set the "sea floor" depth
        SetPrivateField(hook, "sinkSpeed", sinkSpeed); // Set how fast it falls
        SetPrivateField(hook, "isReadyToCast", false); // Force it into "active" mode
        SetPrivateField(hook, "canReel", false); // Disable auto-reeling
        SetPrivateField(hook, "animationDelayDone", true); // Skip the waiting timers

        // MANUALLY call Start() because we are enabling/disabling via code
        typeof(FishingHook)
            .GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(hook, null);

        hook.enabled = true; // Turn the script back on
        return hook; // Return the prepared hook
    }

    // ─── TEST 1: MAX DEPTH CLAMP ──────────────────────────────────────────────────
    /* ORAL EXAM TIP: Mention that this test prevents the player from losing their 
       hook below the floor of your level. It proves the 'Mathf.Clamp' logic works. */
    [UnityTest]
    public IEnumerator Hook_ClampsToMaxDepth()
    {
        FishingHook hook = CreateHook(); // Create default hook (-10 floor)
        yield return null; // Wait for the object to register

        hook.transform.position = new Vector3(0f, -20f, 0f); // Teleport it 10 units below the floor
        SetPrivateField(hook, "isReadyToCast", false); // Ensure it's in a state where physics run
        yield return null; // Wait for the Update/LateUpdate loop

        // Verify the hook was pulled back to -10 or higher
        Assert.GreaterOrEqual(hook.transform.position.y, -10f,
            $"Hook below sea floor — Y:{hook.transform.position.y}, expected >= -10");

        Debug.Log($"<b>[TEST 1] PASSED — Hook clamped to Y:{hook.transform.position.y} (maxDepth: -10)</b>");
        Object.DestroyImmediate(hook.gameObject); // Scene cleanup
    }

    // ─── TEST 2: SURFACE CLAMP ────────────────────────────────────────────────────
    [UnityTest]
    public IEnumerator Hook_ClampsToSurface()
    {
        FishingHook hook = CreateHook(); // Create hook
        yield return null; // Wait for registration

        hook.transform.position = new Vector3(0f, 10f, 0f); // Teleport it into the "sky"
        SetPrivateField(hook, "isReadyToCast", false); // Ensure logic is active
        yield return null; // Wait for physics cycle

        // Verify it was pulled back down to the surface (Y=0)
        Assert.LessOrEqual(hook.transform.position.y, 0f,
            $"Hook above surface — Y:{hook.transform.position.y}, expected <= 0");

        Debug.Log($"<b>[TEST 2] PASSED — Hook clamped to Y:{hook.transform.position.y} (surface: rod tip)</b>");
        Object.DestroyImmediate(hook.gameObject); // Cleanup
    }

    // ─── TEST 3: STAYS WITHIN VERTICAL BOUNDS OVER MULTIPLE FRAMES ───────────────
    [UnityTest]
    public IEnumerator Hook_StaysWithinVerticalBoundsOverTime()
    {
        FishingHook hook = CreateHook(); // Setup hook
        yield return null;

        hook.transform.position = new Vector3(0f, -50f, 0f); // Put it way too deep
        SetPrivateField(hook, "isReadyToCast", false);

        for (int i = 0; i < 10; i++) // Run for 10 consecutive frames
        {
            yield return null; // Wait for next frame
            // Verify it STAYS at the floor and doesn't drift deeper
            Assert.GreaterOrEqual(hook.transform.position.y, -10f,
                $"Frame {i + 1}: Hook below sea floor — Y:{hook.transform.position.y}");
        }

        Debug.Log("<b>[TEST 3] PASSED — Hook stayed above maxDepth for 10 consecutive frames</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 4: SINKS TO MAX DEPTH AND STOPS ────────────────────────────────────
    [UnityTest]
    public IEnumerator Hook_SinksToMaxDepthAndStops()
    {
        FishingHook hook = CreateHook(sinkSpeed: 20f); // Create a fast-sinking hook
        yield return null;

        hook.transform.position = new Vector3(0f, 0f, 0f); // Start at surface
        SetPrivateField(hook, "isReadyToCast", false);

        float elapsed = 0f; // Track time
        while (elapsed < 2f) // Monitor for 2 seconds
        {
            yield return null; // Wait frame
            elapsed += Time.deltaTime; // Increment timer
            // While falling, it should NEVER pass the sea floor
            Assert.GreaterOrEqual(hook.transform.position.y, -10f,
                $"Hook sank past maxDepth during fall — Y:{hook.transform.position.y}!");
        }

        // Final check: It should be exactly at the floor now
        Assert.AreEqual(-10f, hook.transform.position.y, 0.1f,
            $"Hook didn't reach maxDepth — Y:{hook.transform.position.y}");

        Debug.Log($"<b>[TEST 4] PASSED — Hook sank to Y:{hook.transform.position.y} and stopped</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 5: CUSTOM MAX DEPTH VALUE ──────────────────────────────────────────
    [UnityTest]
    public IEnumerator Hook_RespectsCustomMaxDepth()
    {
        FishingHook hook = CreateHook(maxDepth: -5f); // Set a custom "shallow" floor
        yield return null;

        hook.transform.position = new Vector3(0f, -20f, 0f); // Teleport deep
        SetPrivateField(hook, "isReadyToCast", false);
        yield return null; // Process physics

        // Verify it used the custom -5 limit, not the default -10
        Assert.GreaterOrEqual(hook.transform.position.y, -5f,
            $"Custom maxDepth failed — hook at Y:{hook.transform.position.y}, expected >= -5");

        Debug.Log($"<b>[TEST 5] PASSED — Hook respected custom maxDepth of -5: Y:{hook.transform.position.y}</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 6: HOOK STARTING AT VALID Y STAYS VALID ────────────────────────────
    [UnityTest]
    public IEnumerator Hook_StartingAtValidY_StaysInValidRange()
    {
        FishingHook hook = CreateHook(); // Floor is -10
        yield return null;

        hook.transform.position = new Vector3(0f, -5f, 0f); // Start exactly in the middle
        SetPrivateField(hook, "isReadyToCast", false);

        for (int i = 0; i < 5; i++) // Run for 5 frames
        {
            yield return null;
            // It should stay at -5 (not move) if sinkSpeed is 0
            Assert.GreaterOrEqual(hook.transform.position.y, -10f,
                $"Frame {i + 1}: Hook drifted below maxDepth — Y:{hook.transform.position.y}");
        }

        Debug.Log("<b>[TEST 6] PASSED — Hook starting at valid Y:-5 stayed in bounds</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 7: EXTREME POSITION (VERY LARGE NEGATIVE Y) ────────────────────────
    [UnityTest]
    public IEnumerator Hook_ExtremeDepth_ClampsCorrectly()
    {
        FishingHook hook = CreateHook(); // Setup hook
        yield return null;

        hook.transform.position = new Vector3(0f, -99999f, 0f); // Extreme deep teleport
        SetPrivateField(hook, "isReadyToCast", false);
        yield return null;

        // Verify that Mathf.Clamp handles extreme numbers without precision failure
        Assert.GreaterOrEqual(hook.transform.position.y, -10f,
            $"Extreme depth not clamped — hook at Y:{hook.transform.position.y}");

        Debug.Log($"<b>[TEST 7] PASSED — Extreme Y:-99999 clamped to Y:{hook.transform.position.y}</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 8: VERY SHALLOW MAX DEPTH (±0f) ────────────────────────────────────
    [UnityTest]
    public IEnumerator Hook_ZeroDepth_HookLockedAtSurface()
    {
        FishingHook hook = CreateHook(maxDepth: 0f); // Floor is exactly at the surface
        yield return null;

        hook.transform.position = new Vector3(0f, -5f, 0f); // Try to force it down
        SetPrivateField(hook, "isReadyToCast", false);
        yield return null;

        // Hook should be locked at 0 because it has nowhere to sink
        Assert.GreaterOrEqual(hook.transform.position.y, 0f,
            $"Zero-depth boundary failed — hook at Y:{hook.transform.position.y}, expected >= 0");

        Debug.Log($"<b>[TEST 8] PASSED — Hook locked at surface with zero depth: Y:{hook.transform.position.y}</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 9: SLOW SINK SPEED NEVER PASSES MAX DEPTH ──────────────────────────
    [UnityTest]
    public IEnumerator Hook_SlowSink_NeverPassesMaxDepth()
    {
        FishingHook hook = CreateHook(sinkSpeed: 3f); // Realistic slow sinking
        yield return null;

        hook.transform.position = new Vector3(0f, 0f, 0f); // Start at surface
        SetPrivateField(hook, "isReadyToCast", false);

        float elapsed = 0f;
        while (elapsed < 5f) // Run for a long time (5 seconds)
        {
            yield return null; // Wait each frame
            elapsed += Time.deltaTime;
            // Check every single frame that it hasn't glided past the floor
            Assert.GreaterOrEqual(hook.transform.position.y, -10f,
                $"Slow sink passed maxDepth at t={elapsed:F2}s — Y:{hook.transform.position.y}!");
        }

        Debug.Log($"<b>[TEST 9] PASSED — Hook with sinkSpeed:3 never passed maxDepth over 5 seconds. Final Y:{hook.transform.position.y}</b>");
        Object.DestroyImmediate(hook.gameObject); // Cleanup
    }
}