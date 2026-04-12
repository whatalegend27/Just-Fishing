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

    // ─── TEST 1: RIGHT BORDER CLAMP ───────────────────────────────────────────────
    // Forces hook past right border — should clamp back to rightBorder value
    [UnityTest]
    public IEnumerator Hook_ClampsToRightBorder()
    {
        GameObject hookObj = new GameObject("Hook");
        FishingHook hook = hookObj.AddComponent<FishingHook>();

        // debugMode skips FindPlayerReference — no player in test scene
        SetPrivateField(hook, "debugMode", true);

        // Set borders via reflection — now private [SerializeField]
        SetPrivateField(hook, "leftBorder", -8f);
        SetPrivateField(hook, "rightBorder", 8f);

        // Simulate hook in water, sinking state
        SetPrivateField(hook, "isReadyToCast", false);
        SetPrivateField(hook, "canReel", false);
        SetPrivateField(hook, "animationDelayDone", true);

        Debug.Log("<b>[TEST 1] Right Border Clamp — forcing hook to X: 15</b>");

        // Force hook way past the right border
        hook.transform.position = new Vector3(15f, 0f, 0f);

        // Wait a frame for LateUpdate to run and clamp the position
        yield return null;

        float x = hook.transform.position.x;
        Assert.LessOrEqual(x, 8f,
            $"Right border failed — hook is at X: {x}, should be clamped to 8!");

        Debug.Log($"<b>[TEST 1] PASSED — Hook clamped to X: {x} (right border: 8)</b>");

        Object.Destroy(hookObj);
    }

    // ─── TEST 2: LEFT BORDER CLAMP ────────────────────────────────────────────────
    // Forces hook past left border — should clamp back to leftBorder value
    [UnityTest]
    public IEnumerator Hook_ClampsToLeftBorder()
    {
        GameObject hookObj = new GameObject("Hook");
        FishingHook hook = hookObj.AddComponent<FishingHook>();

        SetPrivateField(hook, "debugMode", true);
        SetPrivateField(hook, "leftBorder", -8f);
        SetPrivateField(hook, "rightBorder", 8f);
        SetPrivateField(hook, "isReadyToCast", false);
        SetPrivateField(hook, "canReel", false);
        SetPrivateField(hook, "animationDelayDone", true);

        Debug.Log("<b>[TEST 2] Left Border Clamp — forcing hook to X: -15</b>");

        hook.transform.position = new Vector3(-15f, 0f, 0f);

        yield return null;

        float x = hook.transform.position.x;
        Assert.GreaterOrEqual(x, -8f,
            $"Left border failed — hook is at X: {x}, should be clamped to -8!");

        Debug.Log($"<b>[TEST 2] PASSED — Hook clamped to X: {x} (left border: -8)</b>");

        Object.Destroy(hookObj);
    }

    // ─── TEST 3: HOOK STAYS WITHIN BORDERS AFTER MULTIPLE FRAMES ─────────────────
    // Runs several frames with hook outside border — confirms it stays clamped
    [UnityTest]
    public IEnumerator Hook_StaysWithinBordersOverTime()
    {
        GameObject hookObj = new GameObject("Hook");
        FishingHook hook = hookObj.AddComponent<FishingHook>();

        SetPrivateField(hook, "debugMode", true);
        SetPrivateField(hook, "leftBorder", -8f);
        SetPrivateField(hook, "rightBorder", 8f);
        SetPrivateField(hook, "isReadyToCast", false);
        SetPrivateField(hook, "canReel", false);
        SetPrivateField(hook, "animationDelayDone", true);

        Debug.Log("<b>[TEST 3] Multi-frame boundary hold — 10 frames at X: 20</b>");

        hook.transform.position = new Vector3(20f, 0f, 0f);

        // Check every frame for 10 frames that the hook never escapes the border
        for (int i = 0; i < 10; i++)
        {
            yield return null;

            float x = hook.transform.position.x;
            Assert.LessOrEqual(x, 8f,
                $"Frame {i + 1}: Hook escaped right border — X: {x}");
            Assert.GreaterOrEqual(x, -8f,
                $"Frame {i + 1}: Hook escaped left border — X: {x}");
        }

        Debug.Log("<b>[TEST 3] PASSED — Hook stayed within borders for 10 frames.</b>");

        Object.Destroy(hookObj);
    }

    // ─── TEST 4: HOOK RESPECTS CUSTOM BORDER VALUES ───────────────────────────────
    // Confirms clamping works with different border values, not just -8/8
    [UnityTest]
    public IEnumerator Hook_RespectsCustomBorderValues()
    {
        GameObject hookObj = new GameObject("Hook");
        FishingHook hook = hookObj.AddComponent<FishingHook>();

        SetPrivateField(hook, "debugMode", true);
        SetPrivateField(hook, "leftBorder", -3f);   // your actual inspector values
        SetPrivateField(hook, "rightBorder", 3f);
        SetPrivateField(hook, "isReadyToCast", false);
        SetPrivateField(hook, "canReel", false);
        SetPrivateField(hook, "animationDelayDone", true);

        Debug.Log("<b>[TEST 4] Custom borders (-3 / 3) — forcing hook to X: 10</b>");

        hook.transform.position = new Vector3(10f, 0f, 0f);
        yield return null;

        float x = hook.transform.position.x;
        Assert.LessOrEqual(x, 3f,
            $"Custom right border failed — hook at X: {x}, should be <= 3!");

        hook.transform.position = new Vector3(-10f, 0f, 0f);
        yield return null;

        x = hook.transform.position.x;
        Assert.GreaterOrEqual(x, -3f,
            $"Custom left border failed — hook at X: {x}, should be >= -3!");

        Debug.Log("<b>[TEST 4] PASSED — Hook respected custom border values.</b>");

        Object.Destroy(hookObj);
    }
}