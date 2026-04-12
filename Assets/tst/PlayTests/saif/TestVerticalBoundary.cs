using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Saif.GamePlay;
using System.Reflection;

public class TestVerticalBoundary
{
    private void SetPrivateField(FishingHook hook, string fieldName, object value)
    {
        FieldInfo field = typeof(FishingHook).GetField(fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"Field '{fieldName}' not found!");
        field.SetValue(hook, value);
    }

    private FishingHook CreateHook(float sinkSpeed = 0f)
    {
        GameObject hookObj = new GameObject("Hook");
        FishingHook hook = hookObj.AddComponent<FishingHook>();
        hook.enabled = false;

        SetPrivateField(hook, "debugMode", true);
        SetPrivateField(hook, "maxDepth", -10f);
        SetPrivateField(hook, "surfaceLevel", 1.77f);
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
    [UnityTest]
    public IEnumerator Hook_ClampsToMaxDepth()
    {
        FishingHook hook = CreateHook();
        yield return null; // let LateUpdate register the component first

        hook.transform.position = new Vector3(0f, -20f, 0f);
        SetPrivateField(hook, "isReadyToCast", false);
        yield return null;

        Assert.GreaterOrEqual(hook.transform.position.y, -10f,
            $"Hook below seafloor — Y: {hook.transform.position.y}");

        Debug.Log("<b>[TEST 1] PASSED</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 2: SURFACE CLAMP ────────────────────────────────────────────────────
    [UnityTest]
    public IEnumerator Hook_ClampsToSurface()
    {
        FishingHook hook = CreateHook();
        yield return null; // let LateUpdate register the component first

        hook.transform.position = new Vector3(0f, 10f, 0f);
        SetPrivateField(hook, "isReadyToCast", false);
        yield return null;

        Assert.LessOrEqual(hook.transform.position.y, 0f,
            $"Hook above surface — Y: {hook.transform.position.y}");

        Debug.Log("<b>[TEST 2] PASSED</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 3: STAYS WITHIN BOUNDS OVER TIME ────────────────────────────────────
    [UnityTest]
    public IEnumerator Hook_StaysWithinVerticalBoundsOverTime()
    {
        FishingHook hook = CreateHook();
        yield return null; // let LateUpdate register the component first

        hook.transform.position = new Vector3(0f, -50f, 0f);
        SetPrivateField(hook, "isReadyToCast", false);

        for (int i = 0; i < 10; i++)
        {
            yield return null;
            Assert.GreaterOrEqual(hook.transform.position.y, -10f,
                $"Frame {i + 1}: Hook below seafloor — Y: {hook.transform.position.y}");
        }

        Debug.Log("<b>[TEST 3] PASSED</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 4: SINKS TO MAX DEPTH AND STOPS ────────────────────────────────────
    [UnityTest]
    public IEnumerator Hook_SinksToMaxDepthAndStops()
    {
        FishingHook hook = CreateHook(sinkSpeed: 20f);
        yield return null; // let LateUpdate register the component first

        hook.transform.position = new Vector3(0f, 0f, 0f);
        SetPrivateField(hook, "isReadyToCast", false);

        float elapsed = 0f;
        while (elapsed < 2f)
        {
            yield return null;
            elapsed += Time.deltaTime;
            Assert.GreaterOrEqual(hook.transform.position.y, -10f,
                $"Hook sank below maxDepth — Y: {hook.transform.position.y}!");
        }

        Assert.AreEqual(-10f, hook.transform.position.y, 0.1f,
            $"Hook didn't reach maxDepth — Y: {hook.transform.position.y}");

        Debug.Log("<b>[TEST 4] PASSED</b>");
        Object.DestroyImmediate(hook.gameObject);
    }
}