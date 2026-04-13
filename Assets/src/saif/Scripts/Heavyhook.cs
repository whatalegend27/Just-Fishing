using UnityEngine;

namespace Saif.GamePlay
{
    // HeavyHook extends FishingHook and overrides AttachFish.
    // Put this component on your heavy hook prefab instead of FishingHook.
    // Everything else (casting, reeling, physics) is inherited from FishingHook unchanged.
    public class HeavyHook : FishingHook
    {
        // ── DYNAMIC BINDING ───────────────────────────────────────────────────────
        // This runs INSTEAD of FishingHook.AttachFish when the prefab is a HeavyHook.
        // Same line of code in OnTriggerEnter2D calls AttachFish — but at runtime
        // C# checks the actual object type and routes to this version.
        // Remove "virtual" from FishingHook.AttachFish — this override gets ignored
        // and the base version runs every time, proving dynamic binding is gone.
        protected override void AttachFish(Transform fish, ref Transform slot)
        {
            slot = fish;

            Component movement = fish.GetComponent("FishMovement");
            if (movement != null) (movement as MonoBehaviour).enabled = false;

            fish.SetParent(this.transform);
            fish.localPosition = Vector3.zero;
            fish.localRotation = Quaternion.identity;

            Debug.Log("[HeavyHook] AttachFish — dynamic binding called the correct subclass! (max 2 fish)");
        }
    }
}