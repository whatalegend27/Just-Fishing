using UnityEngine;

namespace Saif.GamePlay
{
    // SmallHook extends FishingHook and overrides AttachFish.
    // Put this component on your small hook prefab instead of FishingHook.
    // Everything else (casting, reeling, physics) is inherited from FishingHook unchanged.
    public class SmallHook : FishingHook
    {
        // ── DYNAMIC BINDING ───────────────────────────────────────────────────────
        // This runs INSTEAD of FishingHook.AttachFish when the prefab is a SmallHook.
        // C# decides which version to call at RUNTIME based on the actual object type.
        // Remove "virtual" from FishingHook.AttachFish — this override gets ignored
        // and the base version runs every time, proving dynamic binding is gone.
        protected new void AttachFish(Transform fish, ref Transform slot)
        {
            slot = fish;

            Component movement = fish.GetComponent("FishMovement");
            if (movement != null) (movement as MonoBehaviour).enabled = false;

            fish.SetParent(this.transform);
            fish.localPosition = Vector3.zero;
            fish.localRotation = Quaternion.identity;

            Debug.Log("[SmallHook] AttachFish — dynamic binding called the correct subclass! (max 1 fish)");
        }
    }
}