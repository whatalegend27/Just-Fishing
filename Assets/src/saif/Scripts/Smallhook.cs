using UnityEngine; // Standard library for Unity features

namespace Saif.GamePlay // Organized in the gameplay folder
{
    // INHERITANCE: SmallHook "is a" FishingHook. It inherits all movement and physics.
    public class SmallHook : FishingHook // Inheriting from the parent FishingHook class
    {
        // ── DYNAMIC BINDING / METHOD HIDING ───────────────────────────────────────
        // The "new" keyword hides the parent's method. 
        // In an exam, if you use "override", C# decides which version to call at RUNTIME.
        // This is the specific behavior for the basic/starter hook.
        // ──────────────────────────────────────────────────────────────────────────
        protected new void AttachFish(Transform fish, ref Transform slot) // Specialized logic for the small hook
        {
            slot = fish; // Assign the fish to the provided slot (usually slot 1)

            Component movement = fish.GetComponent("FishMovement"); // Look for the "FishMovement" script on the fish
            if (movement != null) (movement as MonoBehaviour).enabled = false; // Turn off the fish's AI so it stops moving

            fish.SetParent(this.transform); // Make the fish a child of the hook so it follows the reel up
            fish.localPosition = Vector3.zero; // Snap the fish to the center of the hook
            fish.localRotation = Quaternion.identity; // Reset the fish's rotation to zero

            // Console output to prove to the teacher that the specific SmallHook logic is running
            Debug.Log("[SmallHook] AttachFish — dynamic binding called the correct subclass! (max 1 fish)");
        }
    }
}