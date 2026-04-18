// TestHorizontalBoundary.cs
// Saif Badwan
// 9 tests covering: right border clamp, left border clamp, multi-frame persistence,
// custom border values, zero borders, symmetric borders, narrow corridor, and
// hook starting inside valid range staying inside.

using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Saif.GamePlay;
using System.Reflection;

public class TestHorizontalBoundary
{
    // ─── HELPER ──────────────────────────────────────────────────────────────────

    // Sets any private field on FishingHook via reflection
    private void SetPrivateField(FishingHook hook, string fieldName, object value)
    {
        FieldInfo field = typeof(FishingHook).GetField(fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"Field '{fieldName}' not found — did the variable name change?");
        field.SetValue(hook, value);
    }

    // Creates a hook ready for boundary testing — debugMode on, hook in water, no player needed
    private FishingHook CreateHook(float left = -8f, float right = 8f)
    {
        GameObject hookObj = new GameObject("Hook");
        FishingHook hook = hookObj.AddComponent<FishingHook>();

        SetPrivateField(hook, "debugMode", true);
        SetPrivateField(hook, "leftBorder", left);
        SetPrivateField(hook, "rightBorder", right);
        SetPrivateField(hook, "isReadyToCast", false);
        SetPrivateField(hook, "canReel", false);
        SetPrivateField(hook, "animationDelayDone", true);

        return hook;
    }

    // ─── TEST 1: RIGHT BORDER CLAMP ───────────────────────────────────────────────
    // Forces hook to X=15, which is past the right border of 8.
    // After one LateUpdate frame the clamp in HandleFishingPhysics should bring it back.
    [UnityTest]
    public IEnumerator Hook_ClampsToRightBorder()
    {
        FishingHook hook = CreateHook();
        hook.transform.position = new Vector3(15f, 0f, 0f);

        yield return null; // let LateUpdate run once

        Assert.LessOrEqual(hook.transform.position.x, 8f,
            $"Right border failed — hook at X:{hook.transform.position.x}, expected <= 8");

        Debug.Log($"<b>[TEST 1] PASSED — Hook clamped to X:{hook.transform.position.x} (right border: 8)</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 2: LEFT BORDER CLAMP ────────────────────────────────────────────────
    // Forces hook to X=-15, past the left border of -8.
    // Confirms clamping works in the negative direction too.
    [UnityTest]
    public IEnumerator Hook_ClampsToLeftBorder()
    {
        FishingHook hook = CreateHook();
        hook.transform.position = new Vector3(-15f, 0f, 0f);

        yield return null;

        Assert.GreaterOrEqual(hook.transform.position.x, -8f,
            $"Left border failed — hook at X:{hook.transform.position.x}, expected >= -8");

        Debug.Log($"<b>[TEST 2] PASSED — Hook clamped to X:{hook.transform.position.x} (left border: -8)</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 3: STAYS WITHIN BORDERS OVER MULTIPLE FRAMES ───────────────────────
    // Runs 10 frames with hook stuck at X=20. Confirms it never drifts back out
    // after the first clamp — the clamp must hold every single frame.
    [UnityTest]
    public IEnumerator Hook_StaysWithinBordersOverTime()
    {
        FishingHook hook = CreateHook();
        hook.transform.position = new Vector3(20f, 0f, 0f);

        for (int i = 0; i < 10; i++)
        {
            yield return null;
            float x = hook.transform.position.x;
            Assert.LessOrEqual(x, 8f,   $"Frame {i + 1}: Hook escaped right border — X:{x}");
            Assert.GreaterOrEqual(x, -8f, $"Frame {i + 1}: Hook escaped left border — X:{x}");
        }

        Debug.Log("<b>[TEST 3] PASSED — Hook stayed within borders for 10 consecutive frames</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 4: CUSTOM BORDER VALUES ────────────────────────────────────────────
    // Uses the real inspector values (-3 / 3) instead of the test defaults.
    // Confirms the clamp reads the serialized field correctly regardless of value.
    [UnityTest]
    public IEnumerator Hook_RespectsCustomBorderValues()
    {
        FishingHook hook = CreateHook(left: -3f, right: 3f);

        hook.transform.position = new Vector3(10f, 0f, 0f);
        yield return null;
        Assert.LessOrEqual(hook.transform.position.x, 3f,
            $"Custom right border failed — hook at X:{hook.transform.position.x}, expected <= 3");

        hook.transform.position = new Vector3(-10f, 0f, 0f);
        yield return null;
        Assert.GreaterOrEqual(hook.transform.position.x, -3f,
            $"Custom left border failed — hook at X:{hook.transform.position.x}, expected >= -3");

        Debug.Log("<b>[TEST 4] PASSED — Hook respected custom border values (-3 / 3)</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 5: HOOK STARTS INSIDE VALID RANGE AND STAYS THERE ─────────────────
    // Places hook at X=0 (well inside -8/8). Confirms that a hook that starts
    // legally doesn't get nudged or broken by the clamp over 5 frames.
    [UnityTest]
    public IEnumerator Hook_StartingInsideRange_StaysInsideRange()
    {
        FishingHook hook = CreateHook();
        hook.transform.position = new Vector3(0f, 0f, 0f);

        for (int i = 0; i < 5; i++)
        {
            yield return null;
            float x = hook.transform.position.x;
            Assert.LessOrEqual(x, 8f,    $"Frame {i + 1}: Hook drifted past right border — X:{x}");
            Assert.GreaterOrEqual(x, -8f, $"Frame {i + 1}: Hook drifted past left border — X:{x}");
        }

        Debug.Log("<b>[TEST 5] PASSED — Hook starting at X:0 stayed within borders</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 6: ZERO-WIDTH BORDER (LEFT == RIGHT) ───────────────────────────────
    // Sets both borders to 0. The hook must always be exactly at X=0.
    // Edge case: ensures Mathf.Clamp handles equal min/max without throwing.
    [UnityTest]
    public IEnumerator Hook_ZeroWidthBorder_HookLockedAtZero()
    {
        FishingHook hook = CreateHook(left: 0f, right: 0f);
        hook.transform.position = new Vector3(5f, 0f, 0f); // force it away

        yield return null;

        Assert.AreEqual(0f, hook.transform.position.x, 0.01f,
            $"Zero-width border failed — hook at X:{hook.transform.position.x}, expected 0");

        Debug.Log("<b>[TEST 6] PASSED — Hook locked to X:0 with zero-width border</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 7: EXACT BORDER VALUE IS ALLOWED ───────────────────────────────────
    // Places hook exactly on the right border (X=8). It should NOT be moved —
    // Mathf.Clamp is inclusive so being ON the border is valid.
    [UnityTest]
    public IEnumerator Hook_AtExactBorderValue_IsNotMoved()
    {
        FishingHook hook = CreateHook();
        hook.transform.position = new Vector3(8f, 0f, 0f);

        yield return null;

        Assert.LessOrEqual(hook.transform.position.x, 8f,
            $"Hook went past exact border — X:{hook.transform.position.x}");
        Assert.GreaterOrEqual(hook.transform.position.x, 7.9f,
            $"Hook was pushed away from border — X:{hook.transform.position.x}");

        Debug.Log($"<b>[TEST 7] PASSED — Hook at exact border X:{hook.transform.position.x} was not incorrectly moved</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 8: EXTREME POSITION (VERY LARGE X) ─────────────────────────────────
    // Places hook at X=99999 — a very large number. Confirms Mathf.Clamp handles
    // extreme values without floating point weirdness.
    [UnityTest]
    public IEnumerator Hook_ExtremePosition_ClampsCorrectly()
    {
        FishingHook hook = CreateHook();
        hook.transform.position = new Vector3(99999f, 0f, 0f);

        yield return null;

        Assert.LessOrEqual(hook.transform.position.x, 8f,
            $"Extreme position not clamped — hook at X:{hook.transform.position.x}");

        Debug.Log($"<b>[TEST 8] PASSED — Extreme X:99999 clamped to X:{hook.transform.position.x}</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 9: NARROW CORRIDOR (±0.1f) ─────────────────────────────────────────
    // Sets a very tight border of -0.1 to 0.1. Forces hook to X=5.
    // Confirms precision clamping works at small scale.
    [UnityTest]
    public IEnumerator Hook_NarrowBorder_ClampsToTinyRange()
    {
        FishingHook hook = CreateHook(left: -0.1f, right: 0.1f);
        hook.transform.position = new Vector3(5f, 0f, 0f);

        yield return null;

        Assert.LessOrEqual(hook.transform.position.x, 0.1f,
            $"Narrow right border failed — hook at X:{hook.transform.position.x}");
        Assert.GreaterOrEqual(hook.transform.position.x, -0.1f,
            $"Narrow left border failed — hook at X:{hook.transform.position.x}");

        Debug.Log($"<b>[TEST 9] PASSED — Hook clamped into narrow ±0.1 corridor: X:{hook.transform.position.x}</b>");
        Object.DestroyImmediate(hook.gameObject);
    }
}