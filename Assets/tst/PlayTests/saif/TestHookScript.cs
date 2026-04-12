using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Saif.GamePlay;
using System.Reflection;

public class TestHookScript
{
    private void SetPrivateField(FishingHook hook, string fieldName, object value)
    {
        FieldInfo field = typeof(FishingHook).GetField(fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"Field '{fieldName}' not found!");
        field.SetValue(hook, value);
    }

    private FishingHook CreateHook(bool heavy = false)
    {
        GameObject hookObj = new GameObject("Hook");
        FishingHook hook = hookObj.AddComponent<FishingHook>();
        hook.enabled = false;

        SetPrivateField(hook, "debugMode", true);
        SetPrivateField(hook, "isReadyToCast", false);
        SetPrivateField(hook, "canReel", false);
        SetPrivateField(hook, "animationDelayDone", true);
        hook.SetHookType(heavy);

        typeof(FishingHook)
            .GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(hook, null);

        hook.enabled = true;
        hookObj.AddComponent<BoxCollider2D>().isTrigger = true;
        hookObj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        return hook;
    }

    // Destroys all GameObjects with a given name — cleans up stragglers between tests
    private void DestroyAllNamed(string name)
    {
        foreach (GameObject obj in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            if (obj.name == name)
                Object.DestroyImmediate(obj);
    }

    private GameObject SpawnFish(Vector3 position)
    {
        GameObject fish = new GameObject("Fish");
        fish.tag = "Fish";
        fish.transform.position = position;
        fish.AddComponent<BoxCollider2D>().isTrigger = true;
        fish.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        return fish;
    }

    // ─── TEST 1: SMALL HOOK STRESS ────────────────────────────────────────────────
    [UnityTest]
    public IEnumerator SmallHook_StressTest_OnlyCatchesOneFish()
    {
        FishingHook hook = CreateHook(heavy: false);

        for (int i = 0; i < 50; i++)
            SpawnFish(hook.transform.position);

        yield return new WaitForSeconds(0.5f);

        int caughtCount = hook.transform.childCount;
        Assert.AreEqual(1, caughtCount,
            $"Small hook caught {caughtCount} fish instead of 1!");

        Debug.Log("<b>[TEST 1] PASSED</b>");

        // Use DestroyImmediate so nothing lingers into the next test
        Object.DestroyImmediate(hook.gameObject);
        DestroyAllNamed("Fish");
    }

    // ─── TEST 2: HEAVY HOOK STRESS ────────────────────────────────────────────────
    [UnityTest]
    public IEnumerator HeavyHook_StressTest_OnlyCatchesTwoFish()
    {
        FishingHook hook = CreateHook(heavy: true);

        for (int i = 0; i < 50; i++)
            SpawnFish(hook.transform.position);

        yield return new WaitForSeconds(0.5f);

        int caughtCount = hook.transform.childCount;
        Assert.AreEqual(2, caughtCount,
            $"Heavy hook caught {caughtCount} fish instead of 2!");

        Debug.Log("<b>[TEST 2] PASSED</b>");

        Object.DestroyImmediate(hook.gameObject);
        DestroyAllNamed("Fish");
    }

    // ─── TEST 3: SMALL HOOK IGNORES NON-FISH OBJECTS ─────────────────────────────
    [UnityTest]
    public IEnumerator SmallHook_IgnoresNonFishObjects()
    {
        // Nuke any leftover Fish objects from previous tests before starting
        DestroyAllNamed("Fish");

        FishingHook hook = CreateHook(heavy: false);

        for (int i = 0; i < 10; i++)
        {
            GameObject obj = new GameObject("NotAFish");
            obj.transform.position = hook.transform.position;
            obj.AddComponent<BoxCollider2D>().isTrigger = true;
            obj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }

        yield return new WaitForSeconds(0.5f);

        int caughtCount = hook.transform.childCount;
        foreach (Transform child in hook.transform)
            Debug.LogWarning($"Unexpected catch: '{child.name}' tag: '{child.tag}'");

        Assert.AreEqual(0, caughtCount,
            $"Hook caught {caughtCount} non-fish objects!");

        Debug.Log("<b>[TEST 3] PASSED</b>");
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 4: ISHOOKCAST PROPERTY ─────────────────────────────────────────────
    [UnityTest]
    public IEnumerator Hook_IsHookCast_ReflectsState()
    {
        FishingHook hook = CreateHook(heavy: false);

        Assert.IsTrue(hook.IsHookCast, "IsHookCast should be true when hook is cast!");

        SetPrivateField(hook, "isReadyToCast", true);
        Assert.IsFalse(hook.IsHookCast, "IsHookCast should be false when ready to cast!");

        Debug.Log("<b>[TEST 4] PASSED</b>");
        yield return null;
        Object.DestroyImmediate(hook.gameObject);
    }
}