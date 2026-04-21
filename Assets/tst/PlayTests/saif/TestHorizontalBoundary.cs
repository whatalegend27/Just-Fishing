// TestHorizontalBoundary.cs
// Saif Badwan
// 9 tests covering: right border clamp, left border clamp, multi-frame persistence,
// custom border values, zero borders, symmetric borders, narrow corridor, and
// hook starting inside valid range staying inside.

using UnityEngine; // Access standard Unity classes
using UnityEngine.TestTools; // Needed for PlayMode [UnityTest] attributes
using NUnit.Framework; // Library used for Assert statements (checks for True/False)
using System.Collections; // Allows the use of Coroutines (IEnumerator)
using Saif.GamePlay; // Accesses your FishingHook namespace
using System.Reflection; // Allows "Reflection" to change private variables during tests

public class TestHorizontalBoundary
{
    // ─── HELPER ──────────────────────────────────────────────────────────────────

    // Sets any private field on FishingHook via reflection (avoids making everything public)
    private void SetPrivateField(FishingHook hook, string fieldName, object value)
    {
        // Finds the "hidden" variable in the class memory
        FieldInfo field = typeof(FishingHook).GetField(fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        // Error check: ensures the variable name hasn't changed in the script
        Assert.IsNotNull(field, $"Field '{fieldName}' not found — did the variable name change?");
        // Injects the test value into the hook instance
        field.SetValue(hook, value);
    }

    // Creates a hook ready for boundary testing — debugMode on, hook in water, no player needed
    private FishingHook CreateHook(float left = -8f, float right = 8f)
    {
        GameObject hookObj = new GameObject("Hook"); // Create a temporary object in the scene
        FishingHook hook = hookObj.AddComponent<FishingHook>(); // Add the script

        SetPrivateField(hook, "debugMode", true); // Bypasses searches for the player object
        SetPrivateField(hook, "leftBorder", left); // Set the virtual left wall
        SetPrivateField(hook, "rightBorder", right); // Set the virtual right wall
        SetPrivateField(hook, "isReadyToCast", false); // Logic: The hook is "active" in the water
        SetPrivateField(hook, "canReel", false); // Logic: Don't let it pull back up automatically
        SetPrivateField(hook, "animationDelayDone", true); // Logic: Skip the start-of-cast delay

        return hook; // Return the fully set-up hook for the test
    }

    // ─── TEST 1: RIGHT BORDER CLAMP ───────────────────────────────────────────────
    // Forces hook to X=15, which is past the right border of 8.
    [UnityTest]
    public IEnumerator Hook_ClampsToRightBorder()
    {
        FishingHook hook = CreateHook(); // Create hook with borders -8 to 8
        hook.transform.position = new Vector3(15f, 0f, 0f); // Teleport it WAY outside the right wall

        yield return null; // Wait 1 frame to let LateUpdate() or HandlePhysics() run

        // Verify that the code pulled the hook back to 8 or less
        Assert.LessOrEqual(hook.transform.position.x, 8f,
            $"Right border failed — hook at X:{hook.transform.position.x}, expected <= 8");

        Debug.Log($"<b>[TEST 1] PASSED — Hook clamped to X:{hook.transform.position.x} (right border: 8)</b>");
        Object.DestroyImmediate(hook.gameObject); // Clean up the scene
    }

    // ─── TEST 2: LEFT BORDER CLAMP ────────────────────────────────────────────────
    // Forces hook to X=-15, past the left border of -8.
    [UnityTest]
    public IEnumerator Hook_ClampsToLeftBorder()
    {
        FishingHook hook = CreateHook(); // Create hook
        hook.transform.position = new Vector3(-15f, 0f, 0f); // Teleport it past the left wall

        yield return null; // Let the physics logic process

        // Verify that the code pushed the hook back to -8 or more
        Assert.GreaterOrEqual(hook.transform.position.x, -8f,
            $"Left border failed — hook at X:{hook.transform.position.x}, expected >= -8");

        Debug.Log($"<b>[TEST 2] PASSED — Hook clamped to X:{hook.transform.position.x} (left border: -8)</b>");
        Object.DestroyImmediate(hook.gameObject); // Clean up
    }

    // ─── TEST 3: STAYS WITHIN BORDERS OVER MULTIPLE FRAMES ───────────────────────
    [UnityTest]
    public IEnumerator Hook_StaysWithinBordersOverTime()
    {
        FishingHook hook = CreateHook(); // Setup hook
        hook.transform.position = new Vector3(20f, 0f, 0f); // Teleport outside

        for (int i = 0; i < 10; i++) // Run for 10 consecutive frames
        {
            yield return null; // Wait for the frame update
            float x = hook.transform.position.x; // Grab current X
            Assert.LessOrEqual(x, 8f,   $"Frame {i + 1}: Hook escaped right border — X:{x}"); // Check right
            Assert.GreaterOrEqual(x, -8f, $"Frame {i + 1}: Hook escaped left border — X:{x}"); // Check left
        }

        Debug.Log("<b>[TEST 3] PASSED — Hook stayed within borders for 10 consecutive frames</b>");
        Object.DestroyImmediate(hook.gameObject); // Clean up
    }

    // ─── TEST 4: CUSTOM BORDER VALUES ────────────────────────────────────────────
    [UnityTest]
    public IEnumerator Hook_RespectsCustomBorderValues()
    {
        FishingHook hook = CreateHook(left: -3f, right: 3f); // Test with narrower values than default

        hook.transform.position = new Vector3(10f, 0f, 0f); // Teleport way past 3
        yield return null; // Wait for update
        Assert.LessOrEqual(hook.transform.position.x, 3f, "Custom right border failed"); // Should clamp to 3

        hook.transform.position = new Vector3(-10f, 0f, 0f); // Teleport way past -3
        yield return null; // Wait for update
        Assert.GreaterOrEqual(hook.transform.position.x, -3f, "Custom left border failed"); // Should clamp to -3

        Debug.Log("<b>[TEST 4] PASSED — Hook respected custom border values (-3 / 3)</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 5: HOOK STARTS INSIDE VALID RANGE AND STAYS THERE ─────────────────
    [UnityTest]
    public IEnumerator Hook_StartingInsideRange_StaysInsideRange()
    {
        FishingHook hook = CreateHook(); // Create hook (-8 to 8)
        hook.transform.position = new Vector3(0f, 0f, 0f); // Put it in the center

        for (int i = 0; i < 5; i++) // Run for 5 frames
        {
            yield return null; // Wait for update
            float x = hook.transform.position.x; // It shouldn't move just because clamping is active
            Assert.LessOrEqual(x, 8f,    "Hook drifted past right border"); 
            Assert.GreaterOrEqual(x, -8f, "Hook drifted past left border");
        }

        Debug.Log("<b>[TEST 5] PASSED — Hook starting at X:0 stayed within borders</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 6: ZERO-WIDTH BORDER (LEFT == RIGHT) ───────────────────────────────
    [UnityTest]
    public IEnumerator Hook_ZeroWidthBorder_HookLockedAtZero()
    {
        FishingHook hook = CreateHook(left: 0f, right: 0f); // Set borders to the same spot
        hook.transform.position = new Vector3(5f, 0f, 0f); // Force it away

        yield return null; // Update

        // Hook should be forced to exactly 0 because there is no room to move
        Assert.AreEqual(0f, hook.transform.position.x, 0.01f,
            $"Zero-width border failed — hook at X:{hook.transform.position.x}, expected 0");

        Debug.Log("<b>[TEST 6] PASSED — Hook locked to X:0 with zero-width border</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 7: EXACT BORDER VALUE IS ALLOWED ───────────────────────────────────
    [UnityTest]
    public IEnumerator Hook_AtExactBorderValue_IsNotMoved()
    {
        FishingHook hook = CreateHook(); // -8 to 8
        hook.transform.position = new Vector3(8f, 0f, 0f); // Put it EXACTLY on the line

        yield return null; // Update

        // The clamp should see X=8 is "valid" and not nudge it inward
        Assert.LessOrEqual(hook.transform.position.x, 8f, "Went past border");
        Assert.GreaterOrEqual(hook.transform.position.x, 7.9f, "Pushed away from border");

        Debug.Log($"<b>[TEST 7] PASSED — Hook at exact border X:{hook.transform.position.x} was not incorrectly moved</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 8: EXTREME POSITION (VERY LARGE X) ─────────────────────────────────
    [UnityTest]
    public IEnumerator Hook_ExtremePosition_ClampsCorrectly()
    {
        FishingHook hook = CreateHook(); // -8 to 8
        hook.transform.position = new Vector3(99999f, 0f, 0f); // Set X to nearly one hundred thousand

        yield return null; // Update

        // Clamping should handle high numbers without breaking (Floating point stability)
        Assert.LessOrEqual(hook.transform.position.x, 8f, "Extreme position not clamped");

        Debug.Log($"<b>[TEST 8] PASSED — Extreme X:99999 clamped to X:{hook.transform.position.x}</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 9: NARROW CORRIDOR (±0.1f) ─────────────────────────────────────────
    [UnityTest]
    public IEnumerator Hook_NarrowBorder_ClampsToTinyRange()
    {
        FishingHook hook = CreateHook(left: -0.1f, right: 0.1f); // Set tiny limits
        hook.transform.position = new Vector3(5f, 0f, 0f); // Force way outside

        yield return null; // Update

        // Confirm it works for precision/small-scale numbers
        Assert.LessOrEqual(hook.transform.position.x, 0.1f, "Narrow right border failed");
        Assert.GreaterOrEqual(hook.transform.position.x, -0.1f, "Narrow left border failed");

        Debug.Log($"<b>[TEST 9] PASSED — Hook clamped into narrow ±0.1 corridor: X:{hook.transform.position.x}</b>");
        Object.DestroyImmediate(hook.gameObject);
    }
}