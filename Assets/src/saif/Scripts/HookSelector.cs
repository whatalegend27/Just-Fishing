using UnityEngine;

namespace Saif.GamePlay 
{
    // ─── DYNAMIC BINDING NOTE ────────────────────────────────────────────────────
    // Dynamic binding is demonstrated here through:
    // 1. GetComponent<FishingHook>() at runtime — resolves the exact hook script
    //    on whatever prefab was instantiated, not known until runtime
    // 2. Instantiate(hookPrefab) — which prefab gets spawned is decided at runtime
    //    based on isHeavyHook state — the spawned object's behavior is bound dynamically
    // 3. Camera.main.GetComponent<CameraFollow>() — finds the camera component
    //    at runtime, not hardcoded at compile time
    // ─────────────────────────────────────────────────────────────────────────────
    public class HookSelector : MonoBehaviour
    {
        // ── HOOK PREFABS ──────────────────────────────────────────────────────────
        // Serialized so they can be assigned in the Inspector without being public
        // The actual hook type spawned is decided at runtime — DYNAMIC BINDING
        [Header("Hook Prefabs")]
        [SerializeField] private GameObject smallHookPrefab;
        [SerializeField] private GameObject heavyHookPrefab;

        // ── SPAWN POINTS ──────────────────────────────────────────────────────────
        // spawnPoint — used in the combined scene, positioned at the rod tip
        // debugSpawnPoint — optional, used when testing without the player scene
        [Header("Spawn Points")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform debugSpawnPoint;

        // ── PRIVATE STATE ─────────────────────────────────────────────────────────
        // All state is private — only HookSelector manages hook spawning
        private GameObject currentHook;   // reference to the currently active hook
        private bool isHeavyHook = false; // tracks which hook type is active
        private CameraFollow cameraFollow; // cached camera reference for target updates

        void Start()
        {
            // DYNAMIC BINDING: GetComponent resolves CameraFollow at runtime
            cameraFollow = Camera.main.GetComponent<CameraFollow>();

            // Spawn the small hook by default at game start
            SpawnHook(smallHookPrefab);
        }

        void Update()
        {
            // Z key toggles between hook types on keyboard
            // Blocked if the hook is currently cast — must reel in first
            if (Input.GetKeyDown(KeyCode.Z))
                ToggleHook();
        }

        // ── PUBLIC TOGGLE ─────────────────────────────────────────────────────────
        // Called by both the Z key and the mobile UI switch button
        // Blocks the swap if the hook is currently in the water
        public void ToggleHook()
        {
            // DYNAMIC BINDING: GetComponent resolves FishingHook at runtime
            FishingHook hookScript = currentHook != null
                ? currentHook.GetComponent<FishingHook>()
                : null;

            // Prevent swapping while hook is cast — player must reel in first
            if (hookScript != null && hookScript.IsHookCast) return;

            if (isHeavyHook)
            {
                isHeavyHook = false;
                SpawnHook(smallHookPrefab);
            }
            else
            {
                isHeavyHook = true;
                SpawnHook(heavyHookPrefab);
            }
        }

        // ── SPAWN HOOK ────────────────────────────────────────────────────────────
        // Destroys the current hook and instantiates a new one at the spawn point
        // DYNAMIC BINDING: Instantiate creates a runtime instance of the prefab
        // The exact behavior of the spawned hook depends on which prefab is passed in
        private void SpawnHook(GameObject hookPrefab)
        {
            // Destroy previous hook before spawning a new one
            if (currentHook != null)
                Destroy(currentHook);

            // Use debugSpawnPoint if assigned, otherwise use the main spawnPoint
            Transform spawnPos = (debugSpawnPoint != null) ? debugSpawnPoint : spawnPoint;
            currentHook = Instantiate(hookPrefab, spawnPos.position, Quaternion.identity);

            // Configure the spawned hook via public setters instead of direct field access
            // This maintains encapsulation — HookSelector doesn't touch internal state directly
            FishingHook hookScript = currentHook.GetComponent<FishingHook>();
            if (hookScript != null)
            {
                hookScript.SetHookType(isHeavyHook);
                hookScript.SetDebugSpawnOverride(spawnPos.position);
            }

            // Tell the camera to follow the new hook immediately
            // Prevents the camera from losing its target during the swap
            if (cameraFollow != null)
                cameraFollow.target = currentHook.transform;
        }

        // ── UI BUTTON METHODS ─────────────────────────────────────────────────────
        // These are kept public for UI button assignments in the Inspector
        // Both check if the hook is cast before allowing a swap

        /// <summary>
        /// Switches to the small hook. Blocked if hook is currently cast.
        /// Assign to UI button OnClick in the Inspector.
        /// </summary>
        public void SelectSmallHook()
        {
            FishingHook hookScript = currentHook != null
                ? currentHook.GetComponent<FishingHook>()
                : null;
            if (hookScript != null && hookScript.IsHookCast) return;

            isHeavyHook = false;
            SpawnHook(smallHookPrefab);
        }

        /// <summary>
        /// Switches to the heavy hook. Blocked if hook is currently cast.
        /// Assign to UI button OnClick in the Inspector.
        /// </summary>
        public void SelectHeavyHook()
        {
            FishingHook hookScript = currentHook != null
                ? currentHook.GetComponent<FishingHook>()
                : null;
            if (hookScript != null && hookScript.IsHookCast) return;

            isHeavyHook = true;
            SpawnHook(heavyHookPrefab);
        }
    }
}