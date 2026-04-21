using UnityEngine; // Standard library for all Unity engine functionality

namespace Saif.GamePlay // The organizational namespace used to group my gameplay scripts
{
    // INHERITANCE: HeavyHook is a "child" of FishingHook. It gets all the movement and physics for free.
    public class HeavyHook : FishingHook // This class inherits everything from the base FishingHook class
    {
        // ── DYNAMIC BINDING (POLYMORPHISM) ────────────────────────────────────────
        // The "override" keyword tells C#: "Don't use the parent's AttachFish, use this one instead."
        // This only works because the parent (FishingHook) marked its version as "virtual."
        // ──────────────────────────────────────────────────────────────────────────
        protected override void AttachFish(Transform fish) // Overriding the base method to handle multiple fish
        {
            // SLOT 1 LOGIC (Borrowed behavior from the parent concept)
            if (caughtFishTransform == null) // If the first slot is empty, we fill it first
            {
                caughtFishTransform = fish; // Store the reference to the first fish caught
                hasCaughtFish = true; // Set the boolean flag from the parent class to true

                Component movement = fish.GetComponent("FishMovement"); // Look for the "FishMovement" script on the fish
                if (movement != null) (movement as MonoBehaviour).enabled = false; // Disable the fish AI so it stops swimming

                fish.SetParent(this.transform); // Make the fish a child of the hook so it moves with it
                fish.localPosition = Vector3.zero; // Snap the fish to the center of the hook
                fish.localRotation = Quaternion.identity; // Reset the fish's rotation to look straight

                Debug.Log("[HeavyHook] AttachFish — slot 1 filled. Dynamic binding working!"); // Console proof for the teacher
                return; // Exit the function so we don't accidentally fill slot 2 with the same fish
            }

            // SLOT 2 LOGIC (Exclusive to HeavyHook)
            if (caughtFishTransform2 == null) // If slot 1 is full, check if the second slot is available
            {
                caughtFishTransform2 = fish; // Store the reference to the second fish caught

                Component movement = fish.GetComponent("FishMovement"); // Look for the AI script on the second fish
                if (movement != null) (movement as MonoBehaviour).enabled = false; // Disable the AI for the second fish

                fish.SetParent(this.transform); // Parent the second fish to the hook as well
                fish.localPosition = Vector3.zero; // Position it (you could add an offset here so they don't overlap)
                fish.localRotation = Quaternion.identity; // Reset the second fish's rotation

                Debug.Log("[HeavyHook] AttachFish — slot 2 filled. 2 fish caught!"); // Console confirmation of the specialized behavior
            }
        }
    }
}