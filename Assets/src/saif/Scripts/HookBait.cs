using UnityEngine;

namespace Saif.GamePlay
{
    /********************************
     * HookBait.cs
     * Saif Badwan
     * 
     * Gabe FishMovement script should look for a GameObject tagged "Bait"
     * to find the bait position and move the fish toward it.
     ********************************/
    public class HookBait : MonoBehaviour
    {
        [Header("Bait Settings")]
        [Tooltip("Offset from the hook center where the bait sits (e.g. slightly below the hook)")]
        [SerializeField] private Vector3 baitOffset = new Vector3(0f, -0.2f, 0f);

        [Header("References")]
        [SerializeField] private SpriteRenderer baitSprite; // Drag your bait sprite here in the Inspector

        // The hook this bait belongs to — found automatically at Start
        private FishingHook parentHook;

        // Track whether the bait is currently visible
        private bool baitActive = true;

        void Start()
        {
            // Find the FishingHook on the parent object
            // The bait prefab should be a child of the hook prefab in the hierarchy
            parentHook = GetComponentInParent<FishingHook>();

            if (parentHook == null)
                Debug.LogWarning("[HookBait] No FishingHook found on parent! Make sure Bait is a child of the hook prefab.");

            // Apply the offset so bait sits just below the hook tip
            transform.localPosition = baitOffset;

            ShowBait(true); // Start visible
        }

        void Update()
        {
            if (parentHook == null) return;

            // If the hook is cast and has caught a fish → hide the bait (fish ate it)
            // If the hook reset (back at rod tip, not cast) → show the bait again for next cast
            if (parentHook.IsHookCast && parentHook.HasFishAttached)
                ShowBait(false); // Fish ate the bait
            else if (!parentHook.IsHookCast)
                ShowBait(true);  // Hook reset, bait is fresh again for next cast
        }

        // Shows or hides the bait sprite
        private void ShowBait(bool visible)
        {
            if (baitActive == visible) return; // No change needed
            baitActive = visible;
            if (baitSprite != null) baitSprite.enabled = visible;
        }

        // Gabe you can call this to get the bait's world position
        // and move the fish toward it
        public Vector3 GetBaitWorldPosition()
        {
            return transform.position;
        }
    }
}