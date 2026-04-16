using UnityEngine;

namespace Saif.GamePlay
{
    // HeavyHook extends FishingHook and overrides AttachFish to allow 2 fish.
    // Put this component on your heavy hook prefab instead of FishingHook.
    // Everything else (casting, reeling, physics) is inherited from FishingHook unchanged.
    public class HeavyHook : FishingHook
    {
        // ── DYNAMIC BINDING ───────────────────────────────────────────────────────
        // WITH "virtual" in FishingHook:    C# checks runtime type → this runs → 2 fish ✓
        // WITHOUT "virtual" in FishingHook: base AttachFish always runs → 1 fish only ✓
        //                                   proving dynamic binding is gone
        protected override void AttachFish(Transform fish)
        {
            // Slot 1 — same as SmallHook
            if (caughtFishTransform == null)
            {
                caughtFishTransform = fish;
                hasCaughtFish = true;

                Component movement = fish.GetComponent("FishMovement");
                if (movement != null) (movement as MonoBehaviour).enabled = false;

                fish.SetParent(this.transform);
                fish.localPosition = Vector3.zero;
                fish.localRotation = Quaternion.identity;

                Debug.Log("[HeavyHook] AttachFish — slot 1 filled. Dynamic binding working!");
                return;
            }

            // Slot 2 — HeavyHook exclusive
            if (caughtFishTransform2 == null)
            {
                caughtFishTransform2 = fish;

                Component movement = fish.GetComponent("FishMovement");
                if (movement != null) (movement as MonoBehaviour).enabled = false;

                fish.SetParent(this.transform);
                fish.localPosition = Vector3.zero; // slight offset so fish don't overlap
                fish.localRotation = Quaternion.identity;

                Debug.Log("[HeavyHook] AttachFish — slot 2 filled. 2 fish caught!");
            }
        }
    }
}