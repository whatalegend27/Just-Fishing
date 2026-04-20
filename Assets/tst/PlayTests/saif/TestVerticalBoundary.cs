// TestVerticalBoundary.cs
// Saif Badwan
// 9 tests covering: max depth clamp, surface clamp, multi-frame persistence,
// sinking to floor and stopping, custom depth values, extreme positions,
// hook starting at valid Y, zero depth range, and slow sink speed accuracy.

using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Saif.GamePlay;
using System.Reflection;

public class TestVerticalBoundary
{
    // ─── HELPER ──────────────────────────────────────────────────────────────────

    private void SetPrivateField(FishingHook hook, string fieldName, object value)
    {
        FieldInfo field = typeof(FishingHook).GetField(fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"Field '{fieldName}' not found — did the variable name change?");
        field.SetValue(hook, value);
    }

    // Creates a hook ready for vertical boundary testing.
    // sinkSpeed=0 by default so the hook doesn't move on its own mid-test unless we want it to.
    private FishingHook CreateHook(float maxDepth = -10f, float sinkSpeed = 0f)
    {
        GameObject hookObj = new GameObject("Hook");
        FishingHook hook = hookObj.AddComponent<FishingHook>();
        hook.enabled = false;

        SetPrivateField(hook, "debugMode", true);
        SetPrivateField(hook, "maxDepth", maxDepth);
        SetPrivateField(hook, "sinkSpeed", sinkSpeed);
        SetPrivateField(hook, "isReadyToCast", false);
        SetPrivateField(hook, "canReel", false);
        SetPrivateField(hook, "animationDelayDone", true);

        typeof(FishingHook)
            .GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(hook, null);

        hook.enabled = true;
        return hook;
    }

    // ─── TEST 1: MAX DEPTH CLAMP ──────────────────────────────────────────────────
    // Forces hook to Y=-20, which is below the sea floor at -10.
    // After one LateUpdate the clamp should pull it back to -10.
    [UnityTest]
    public IEnumerator Hook_ClampsToMaxDepth()
    {
        FishingHook hook = CreateHook();
        yield return null; // let LateUpdate register the component

        hook.transform.position = new Vector3(0f, -20f, 0f);
        SetPrivateField(hook, "isReadyToCast", false);
        yield return null;

        Assert.GreaterOrEqual(hook.transform.position.y, -10f,
            $"Hook below sea floor — Y:{hook.transform.position.y}, expected >= -10");

        Debug.Log($"<b>[TEST 1] PASSED — Hook clamped to Y:{hook.transform.position.y} (maxDepth: -10)</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 2: SURFACE CLAMP ────────────────────────────────────────────────────
    // Forces hook to Y=10, above the rod tip position (which is 0 in debug mode).
    // The clamp uses rodTipPos.y as the ceiling — hook must not go above it.
    [UnityTest]
    public IEnumerator Hook_ClampsToSurface()
    {
        FishingHook hook = CreateHook();
        yield return null;

        hook.transform.position = new Vector3(0f, 10f, 0f);
        SetPrivateField(hook, "isReadyToCast", false);
        yield return null;

        Assert.LessOrEqual(hook.transform.position.y, 0f,
            $"Hook above surface — Y:{hook.transform.position.y}, expected <= 0");

        Debug.Log($"<b>[TEST 2] PASSED — Hook clamped to Y:{hook.transform.position.y} (surface: rod tip)</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 3: STAYS WITHIN VERTICAL BOUNDS OVER MULTIPLE FRAMES ───────────────
    // Places hook at Y=-50 and runs 10 frames. Confirms the clamp holds every frame,
    // not just the first one.
    [UnityTest]
    public IEnumerator Hook_StaysWithinVerticalBoundsOverTime()
    {
        FishingHook hook = CreateHook();
        yield return null;

        hook.transform.position = new Vector3(0f, -50f, 0f);
        SetPrivateField(hook, "isReadyToCast", false);

        for (int i = 0; i < 10; i++)
        {
            yield return null;
            Assert.GreaterOrEqual(hook.transform.position.y, -10f,
                $"Frame {i + 1}: Hook below sea floor — Y:{hook.transform.position.y}");
        }

        Debug.Log("<b>[TEST 3] PASSED — Hook stayed above maxDepth for 10 consecutive frames</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 4: SINKS TO MAX DEPTH AND STOPS ────────────────────────────────────
    // Uses sinkSpeed=20 so the hook falls fast. Checks it never passes -10
    // during the fall and actually arrives at -10 within 2 seconds.
    [UnityTest]
    public IEnumerator Hook_SinksToMaxDepthAndStops()
    {
        FishingHook hook = CreateHook(sinkSpeed: 20f);
        yield return null;

        hook.transform.position = new Vector3(0f, 0f, 0f);
        SetPrivateField(hook, "isReadyToCast", false);

        float elapsed = 0f;
        while (elapsed < 2f)
        {
            yield return null;
            elapsed += Time.deltaTime;
            Assert.GreaterOrEqual(hook.transform.position.y, -10f,
                $"Hook sank past maxDepth during fall — Y:{hook.transform.position.y}!");
        }

        Assert.AreEqual(-10f, hook.transform.position.y, 0.1f,
            $"Hook didn't reach maxDepth — Y:{hook.transform.position.y}");

        Debug.Log($"<b>[TEST 4] PASSED — Hook sank to Y:{hook.transform.position.y} and stopped</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 5: CUSTOM MAX DEPTH VALUE ──────────────────────────────────────────
    // Uses maxDepth=-5 instead of the default -10. Confirms the clamp reads the
    // field correctly — the boundary isn't hardcoded anywhere.
    [UnityTest]
    public IEnumerator Hook_RespectsCustomMaxDepth()
    {
        FishingHook hook = CreateHook(maxDepth: -5f);
        yield return null;

        hook.transform.position = new Vector3(0f, -20f, 0f);
        SetPrivateField(hook, "isReadyToCast", false);
        yield return null;

        Assert.GreaterOrEqual(hook.transform.position.y, -5f,
            $"Custom maxDepth failed — hook at Y:{hook.transform.position.y}, expected >= -5");

        Debug.Log($"<b>[TEST 5] PASSED — Hook respected custom maxDepth of -5: Y:{hook.transform.position.y}</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 6: HOOK STARTING AT VALID Y STAYS VALID ────────────────────────────
    // Places hook at Y=-5, which is inside the valid range (-10 to 0).
    // Confirms the clamp doesn't incorrectly push a legally placed hook.
    [UnityTest]
    public IEnumerator Hook_StartingAtValidY_StaysInValidRange()
    {
        FishingHook hook = CreateHook();
        yield return null;

        hook.transform.position = new Vector3(0f, -5f, 0f);
        SetPrivateField(hook, "isReadyToCast", false);

        for (int i = 0; i < 5; i++)
        {
            yield return null;
            Assert.GreaterOrEqual(hook.transform.position.y, -10f,
                $"Frame {i + 1}: Hook drifted below maxDepth — Y:{hook.transform.position.y}");
        }

        Debug.Log("<b>[TEST 6] PASSED — Hook starting at valid Y:-5 stayed in bounds</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 7: EXTREME POSITION (VERY LARGE NEGATIVE Y) ────────────────────────
    // Forces hook to Y=-99999. Confirms Mathf.Clamp handles extreme floats
    // without floating point errors or passing the boundary.
    [UnityTest]
    public IEnumerator Hook_ExtremeDepth_ClampsCorrectly()
    {
        FishingHook hook = CreateHook();
        yield return null;

        hook.transform.position = new Vector3(0f, -99999f, 0f);
        SetPrivateField(hook, "isReadyToCast", false);
        yield return null;

        Assert.GreaterOrEqual(hook.transform.position.y, -10f,
            $"Extreme depth not clamped — hook at Y:{hook.transform.position.y}");

        Debug.Log($"<b>[TEST 7] PASSED — Extreme Y:-99999 clamped to Y:{hook.transform.position.y}</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 8: VERY SHALLOW MAX DEPTH (±0f) ────────────────────────────────────
    // Sets maxDepth=0 so the hook has no room to sink at all.
    // Forces hook to Y=-5 — it must be pulled back to exactly 0.
    [UnityTest]
    public IEnumerator Hook_ZeroDepth_HookLockedAtSurface()
    {
        FishingHook hook = CreateHook(maxDepth: 0f);
        yield return null;

        hook.transform.position = new Vector3(0f, -5f, 0f);
        SetPrivateField(hook, "isReadyToCast", false);
        yield return null;

        Assert.GreaterOrEqual(hook.transform.position.y, 0f,
            $"Zero-depth boundary failed — hook at Y:{hook.transform.position.y}, expected >= 0");

        Debug.Log($"<b>[TEST 8] PASSED — Hook locked at surface with zero depth: Y:{hook.transform.position.y}</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 9: SLOW SINK SPEED NEVER PASSES MAX DEPTH ──────────────────────────
    // Uses a realistic slow sinkSpeed=3 from Y=0. Runs for 5 seconds and checks
    // every frame that the hook never sneaks past maxDepth even at slow speed.
    [UnityTest]
    public IEnumerator Hook_SlowSink_NeverPassesMaxDepth()
    {
        FishingHook hook = CreateHook(sinkSpeed: 3f);
        yield return null;

        hook.transform.position = new Vector3(0f, 0f, 0f);
        SetPrivateField(hook, "isReadyToCast", false);

        float elapsed = 0f;
        while (elapsed < 5f)
        {
            yield return null;
            elapsed += Time.deltaTime;
            Assert.GreaterOrEqual(hook.transform.position.y, -10f,
                $"Slow sink passed maxDepth at t={elapsed:F2}s — Y:{hook.transform.position.y}!");
        }

        Debug.Log($"<b>[TEST 9] PASSED — Hook with sinkSpeed:3 never passed maxDepth over 5 seconds. Final Y:{hook.transform.position.y}</b>");
        Object.DestroyImmediate(hook.gameObject);
    }
}