// TestHookScript.cs
// Saif Badwan
// 12 tests covering: fish catching limits, dynamic binding proof,
// non-fish rejection, hook reset, IsHookCast property, and stress cases.

using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Saif.GamePlay;
using System.Reflection;

public class TestHookScript
{
    // ─── HELPERS ──────────────────────────────────────────────────────────────────

    // Sets any private field on FishingHook via reflection so we don't need to
    // expose things publicly just for testing
    private void SetPrivateField(FishingHook hook, string fieldName, object value)
    {
        FieldInfo field = typeof(FishingHook).GetField(fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"Field '{fieldName}' not found — did the variable name change?");
        field.SetValue(hook, value);
    }

    // Creates a plain FishingHook (SmallHook behaviour — catches 1 fish)
    // debugMode=true skips FindPlayerReference so no player is needed in test scene
    private FishingHook CreateSmallHook()
    {
        GameObject hookObj = new GameObject("Hook");
        FishingHook hook = hookObj.AddComponent<FishingHook>();

        SetPrivateField(hook, "debugMode", true);
        SetPrivateField(hook, "isReadyToCast", false);
        SetPrivateField(hook, "canReel", false);
        SetPrivateField(hook, "animationDelayDone", true);

        hookObj.AddComponent<BoxCollider2D>().isTrigger = true;
        hookObj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        return hook;
    }

    // Creates a HeavyHook (catches 2 fish via dynamic binding override)
    private HeavyHook CreateHeavyHook()
    {
        GameObject hookObj = new GameObject("HeavyHook");
        HeavyHook hook = hookObj.AddComponent<HeavyHook>();

        SetPrivateField(hook, "debugMode", true);
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
        GameObject fish = new GameObject("Fish");
        fish.tag = "Fish";
        fish.transform.position = position;
        fish.AddComponent<BoxCollider2D>().isTrigger = true;
        fish.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        return fish;
    }

    // Destroys every GameObject with a given name — keeps test scene clean between runs
    private void DestroyAllNamed(string name)
    {
        foreach (GameObject obj in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            if (obj.name == name) Object.DestroyImmediate(obj);
    }

    // ─── TEST 1: SMALL HOOK CATCHES EXACTLY 1 FISH ───────────────────────────────
    // Spawns 50 fish on top of the hook and confirms only 1 is ever caught.
    // The base AttachFish returns early if caughtFishTransform is already filled.
    [UnityTest]
    public IEnumerator SmallHook_OnlyCatchesOneFish()
    {
        FishingHook hook = CreateSmallHook();
        for (int i = 0; i < 50; i++) SpawnFish(hook.transform.position);

        yield return new WaitForSeconds(0.3f);

        Assert.AreEqual(1, hook.transform.childCount,
            $"SmallHook caught {hook.transform.childCount} fish — should be exactly 1!");

        Debug.Log("<b>[TEST 1] PASSED — SmallHook caught 1 fish from 50 spawned</b>");
        Object.DestroyImmediate(hook.gameObject);
        DestroyAllNamed("Fish");
    }

    // ─── TEST 2: HEAVY HOOK CATCHES EXACTLY 2 FISH ───────────────────────────────
    // Spawns 50 fish and confirms HeavyHook stops at 2 via its override.
    // This is the dynamic binding proof — the override fills slot 2, base never would.
    [UnityTest]
    public IEnumerator HeavyHook_OnlyCatchesTwoFish()
    {
        HeavyHook hook = CreateHeavyHook();
        for (int i = 0; i < 50; i++) SpawnFish(hook.transform.position);

        yield return new WaitForSeconds(0.3f);

        Assert.AreEqual(2, hook.transform.childCount,
            $"HeavyHook caught {hook.transform.childCount} fish — should be exactly 2!");

        Debug.Log("<b>[TEST 2] PASSED — HeavyHook caught 2 fish from 50 spawned</b>");
        Object.DestroyImmediate(hook.gameObject);
        DestroyAllNamed("Fish");
    }

    // ─── TEST 3: SMALL HOOK IGNORES NON-FISH OBJECTS ─────────────────────────────
    // Spawns 20 untagged objects on the hook. OnTriggerEnter2D filters by "Fish" tag
    // so none should be caught. Confirms tag check is working.
    [UnityTest]
    public IEnumerator SmallHook_IgnoresNonFishObjects()
    {
        FishingHook hook = CreateSmallHook();

        for (int i = 0; i < 20; i++)
        {
            GameObject obj = new GameObject("NotAFish");  // no "Fish" tag
            obj.transform.position = hook.transform.position;
            obj.AddComponent<BoxCollider2D>().isTrigger = true;
            obj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }

        yield return new WaitForSeconds(0.3f);

        Assert.AreEqual(0, hook.transform.childCount,
            $"Hook caught {hook.transform.childCount} non-fish objects — tag check broken!");

        Debug.Log("<b>[TEST 3] PASSED — Hook correctly ignored all non-fish objects</b>");
        Object.DestroyImmediate(hook.gameObject);
        DestroyAllNamed("NotAFish");
    }

    // ─── TEST 4: HEAVY HOOK IGNORES NON-FISH OBJECTS ─────────────────────────────
    // Same as TEST 3 but with HeavyHook — confirms its override also respects the tag filter.
    [UnityTest]
    public IEnumerator HeavyHook_IgnoresNonFishObjects()
    {
        HeavyHook hook = CreateHeavyHook();

        for (int i = 0; i < 20; i++)
        {
            GameObject obj = new GameObject("NotAFish");
            obj.transform.position = hook.transform.position;
            obj.AddComponent<BoxCollider2D>().isTrigger = true;
            obj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }

        yield return new WaitForSeconds(0.3f);

        Assert.AreEqual(0, hook.transform.childCount,
            $"HeavyHook caught {hook.transform.childCount} non-fish — tag check broken!");

        Debug.Log("<b>[TEST 4] PASSED — HeavyHook correctly ignored all non-fish objects</b>");
        Object.DestroyImmediate(hook.gameObject);
        DestroyAllNamed("NotAFish");
    }

    // ─── TEST 5: ISHOOKCAST IS TRUE WHEN CAST ────────────────────────────────────
    // IsHookCast returns !isReadyToCast. We set isReadyToCast=false (hook is in water)
    // and confirm IsHookCast reads true. Used by HookSelector to block swapping mid-cast.
    [UnityTest]
    public IEnumerator Hook_IsHookCast_TrueWhenInWater()
    {
        FishingHook hook = CreateSmallHook();
        // isReadyToCast is already false from CreateSmallHook
        Assert.IsTrue(hook.IsHookCast, "IsHookCast should be TRUE when hook is in the water!");

        Debug.Log("<b>[TEST 5] PASSED — IsHookCast correctly reports TRUE when cast</b>");
        yield return null;
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 6: ISHOOKCAST IS FALSE WHEN READY ──────────────────────────────────
    // Sets isReadyToCast=true (hook is back at rod tip) and confirms IsHookCast is false.
    // Proves the property flips correctly for both states.
    [UnityTest]
    public IEnumerator Hook_IsHookCast_FalseWhenReady()
    {
        FishingHook hook = CreateSmallHook();
        SetPrivateField(hook, "isReadyToCast", true);

        Assert.IsFalse(hook.IsHookCast, "IsHookCast should be FALSE when hook is ready to cast!");

        Debug.Log("<b>[TEST 6] PASSED — IsHookCast correctly reports FALSE when ready</b>");
        yield return null;
        Object.DestroyImmediate(hook.gameObject);
    }

    // ─── TEST 7: DYNAMIC BINDING PROOF — SMALL VS HEAVY FISH COUNT ───────────────
    // Creates both hook types side by side and spawns 10 fish on each.
    // SmallHook (base) → 1 fish. HeavyHook (override) → 2 fish.
    // This directly proves virtual dispatch is routing to the correct version.
    [UnityTest]
    public IEnumerator DynamicBinding_SmallAndHeavyHookBehaveDifferently()
    {
        FishingHook smallHook = CreateSmallHook();
        smallHook.transform.position = new Vector3(-5f, 0f, 0f);

        HeavyHook heavyHook = CreateHeavyHook();
        heavyHook.transform.position = new Vector3(5f, 0f, 0f);

        // Spawn fish on small hook
        for (int i = 0; i < 10; i++) SpawnFish(smallHook.transform.position);
        // Spawn fish on heavy hook
        for (int i = 0; i < 10; i++)
        {
            GameObject fish = new GameObject("Fish");
            fish.tag = "Fish";
            fish.transform.position = heavyHook.transform.position;
            fish.AddComponent<BoxCollider2D>().isTrigger = true;
            fish.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }

        yield return new WaitForSeconds(0.3f);

        Assert.AreEqual(1, smallHook.transform.childCount,
            $"SmallHook should have 1 fish, has {smallHook.transform.childCount}");
        Assert.AreEqual(2, heavyHook.transform.childCount,
            $"HeavyHook should have 2 fish, has {heavyHook.transform.childCount}");

        Debug.Log("<b>[TEST 7] PASSED — Dynamic binding confirmed: Small=1 fish, Heavy=2 fish</b>");
        Object.DestroyImmediate(smallHook.gameObject);
        Object.DestroyImmediate(heavyHook.gameObject);
        DestroyAllNamed("Fish");
    }

    // ─── TEST 8: SMALL HOOK CATCHES 1 FISH THEN IGNORES MORE ─────────────────────
    // Spawns 1 fish, waits for it to be caught, then spawns 5 more.
    // Confirms the hook stays at 1 child and doesn't grab the later fish.
    [UnityTest]
    public IEnumerator SmallHook_CatchesFirstFishThenIgnoresRest()
    {
        FishingHook hook = CreateSmallHook();

        SpawnFish(hook.transform.position);
        yield return new WaitForSeconds(0.2f);

        Assert.AreEqual(1, hook.transform.childCount, "Should have caught the first fish!");

        // Spawn 5 more after the slot is full
        for (int i = 0; i < 5; i++) SpawnFish(hook.transform.position);
        yield return new WaitForSeconds(0.2f);

        Assert.AreEqual(1, hook.transform.childCount,
            $"SmallHook grabbed more fish after slot was full — has {hook.transform.childCount}!");

        Debug.Log("<b>[TEST 8] PASSED — SmallHook correctly ignored fish after slot was full</b>");
        Object.DestroyImmediate(hook.gameObject);
        DestroyAllNamed("Fish");
    }

    // ─── TEST 9: HEAVY HOOK FILLS SLOT 1 THEN SLOT 2 ────────────────────────────
    // Spawns fish one at a time and checks child count grows to 2 then stops.
    // Proves the two-slot logic in HeavyHook.AttachFish works correctly in sequence.
    [UnityTest]
    public IEnumerator HeavyHook_FillsBothSlotsSequentially()
    {
        HeavyHook hook = CreateHeavyHook();

        SpawnFish(hook.transform.position);
        yield return new WaitForSeconds(0.2f);
        Assert.AreEqual(1, hook.transform.childCount, "Slot 1 should be filled after first fish!");

        SpawnFish(hook.transform.position);
        yield return new WaitForSeconds(0.2f);
        Assert.AreEqual(2, hook.transform.childCount, "Slot 2 should be filled after second fish!");

        SpawnFish(hook.transform.position);
        yield return new WaitForSeconds(0.2f);
        Assert.AreEqual(2, hook.transform.childCount,
            $"HeavyHook should stop at 2 — has {hook.transform.childCount}!");

        Debug.Log("<b>[TEST 9] PASSED — HeavyHook filled both slots then correctly stopped</b>");
        Object.DestroyImmediate(hook.gameObject);
        DestroyAllNamed("Fish");
    }

    // ─── TEST 10: MIXED OBJECTS — FISH AND NON-FISH TOGETHER ─────────────────────
    // Spawns a mix of tagged fish and untagged objects on the hook.
    // Confirms only the tagged fish are caught — non-fish objects are ignored even
    // when they arrive at the same time as real fish.
    [UnityTest]
    public IEnumerator SmallHook_MixedObjects_OnlyCatchesFish()
    {
        FishingHook hook = CreateSmallHook();

        // Spawn 10 non-fish
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = new GameObject("NotAFish");
            obj.transform.position = hook.transform.position;
            obj.AddComponent<BoxCollider2D>().isTrigger = true;
            obj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }

        // Spawn 10 real fish in the same batch
        for (int i = 0; i < 10; i++) SpawnFish(hook.transform.position);

        yield return new WaitForSeconds(0.3f);

        // Should have caught exactly 1 (the limit), and it must be a fish
        Assert.AreEqual(1, hook.transform.childCount,
            $"Expected 1 caught, got {hook.transform.childCount}");
        Assert.AreEqual("Fish", hook.transform.GetChild(0).tag,
            "The caught object is not tagged Fish — non-fish slipped through!");

        Debug.Log("<b>[TEST 10] PASSED — Hook correctly caught only 1 tagged fish among mixed objects</b>");
        Object.DestroyImmediate(hook.gameObject);
        DestroyAllNamed("Fish");
        DestroyAllNamed("NotAFish");
    }

    // ─── TEST 11: SINGLE FISH CAUGHT BECOMES CHILD OF HOOK ───────────────────────
    // Confirms that after being caught, the fish transform's parent is the hook.
    // This checks the SetParent call inside AttachFish actually ran.
    [UnityTest]
    public IEnumerator SmallHook_CaughtFishBecomesChildOfHook()
    {
        FishingHook hook = CreateSmallHook();
        GameObject fish = SpawnFish(hook.transform.position);

        yield return new WaitForSeconds(0.2f);

        // The fish should now be parented to the hook
        Assert.AreEqual(hook.transform, fish.transform.parent,
            "Caught fish is not parented to the hook — SetParent failed!");

        Debug.Log("<b>[TEST 11] PASSED — Caught fish is correctly parented to hook</b>");
        Object.DestroyImmediate(hook.gameObject);
        DestroyAllNamed("Fish");
    }

    // ─── TEST 12: HEAVY HOOK STRESS — 100 FISH, NEVER EXCEEDS 2 ─────────────────
    // Extreme stress test: 100 fish all at once on HeavyHook.
    // Confirms the two-slot guard never breaks under heavy load.
    [UnityTest]
    public IEnumerator HeavyHook_StressTest_NeverExceedsTwoFish()
    {
        HeavyHook hook = CreateHeavyHook();
        for (int i = 0; i < 100; i++) SpawnFish(hook.transform.position);

        yield return new WaitForSeconds(0.5f);

        Assert.LessOrEqual(hook.transform.childCount, 2,
            $"HeavyHook exceeded 2 fish under stress — caught {hook.transform.childCount}!");

        Debug.Log($"<b>[TEST 12] PASSED — HeavyHook held at {hook.transform.childCount} fish under 100-fish stress</b>");
        Object.DestroyImmediate(hook.gameObject);
        DestroyAllNamed("Fish");
    }
}