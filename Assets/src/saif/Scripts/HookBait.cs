using UnityEngine;

namespace Saif.GamePlay
{
    /********************************
     * HookBait.cs
     * Saif Badwan
     *
     * Attach this to the Bait child object inside the hook prefab.
     * - Bait only shows if the player has at least 1 Lure in their inventory
     * - When the hook is cast, 1 Lure is consumed from the inventory
     * - Bait disappears when a fish is caught (fish ate it)
     * - Bait reappears on next cast only if inventory still has Lures
     ********************************/
    public class HookBait : MonoBehaviour
    {
        [Header("Bait Settings")]
        [Tooltip("Offset from hook center where the bait sits")]
        [SerializeField] private Vector3 baitOffset = new Vector3(0f, -0.2f, 0f);

        [Header("References")]
        [SerializeField] private SpriteRenderer baitSprite;

        private FishingHook parentHook;

        // Tracks if we already consumed a bait for this cast
        // so we don't keep removing from inventory every frame
        private bool baitConsumedThisCast = false;

        // Tracks the last cast state to detect when a new cast starts
        private bool wasHookCastLastFrame = false;

        void Start()
        {
            parentHook = GetComponentInParent<FishingHook>();

            if (parentHook == null)
                Debug.LogWarning("[HookBait] No FishingHook found on parent!");

            transform.localPosition = baitOffset;
            ShowBait(false); // Hidden at start — hook hasn't been cast yet
        }

        void Update()
        {
            if (parentHook == null) return;

            bool hookIscastNow = parentHook.IsHookCast;

            // ── NEW CAST STARTED ──────────────────────────────────────────────
            // Detect the moment the hook goes from not cast → cast
            if (hookIscastNow && !wasHookCastLastFrame)
            {
                baitConsumedThisCast = false; // Reset for new cast
            }

            // ── HOOK IS IN THE WATER ──────────────────────────────────────────
            if (hookIscastNow)
            {
                if (parentHook.HasFishAttached)
                {
                    // Fish caught — hide bait (fish ate it)
                    ShowBait(false);
                }
                else if (!baitConsumedThisCast)
                {
                    // First frame of cast — check inventory and consume 1 bait
                    if (HasBaitInInventory())
                    {
                        ConsumeBait();              // Remove 1 from inventory
                        baitConsumedThisCast = true;
                        ShowBait(true);             // Show the bait on the hook
                    }
                    else
                    {
                        // No bait in inventory — fish without bait
                        ShowBait(false);
                        Debug.Log("[HookBait] No Lure in inventory — fishing without bait.");
                    }
                }
            }
            else
            {
                // ── HOOK RESET (back at rod tip) ──────────────────────────────
                ShowBait(false); // Hide bait when not fishing
            }

            wasHookCastLastFrame = hookIscastNow;
        }

        // Checks if the player has at least 1 Lure in their inventory
        private bool HasBaitInInventory()
        {
            if (InventoryManager.Instance == null) return false;

            foreach (InventorySlotData slot in InventoryManager.Instance.slots)
            {
                if (slot.item != null &&
                    slot.item.Type == ItemScript.ItemType.Lure &&
                    slot.quantity > 0)
                {
                    return true;
                }
            }
            return false;
        }

        // Finds the bait item in inventory and removes 1
        private void ConsumeBait()
        {
            if (InventoryManager.Instance == null) return;

            foreach (InventorySlotData slot in InventoryManager.Instance.slots)
            {
                if (slot.item != null &&
                    slot.item.Type == ItemScript.ItemType.Lure &&
                    slot.quantity > 0)
                {
                    InventoryManager.Instance.RemoveItem(slot.item);
                    Debug.Log("[HookBait] 1 Lure consumed from inventory.");
                    return;
                }
            }
        }

        // Shows or hides the bait sprite
        private void ShowBait(bool visible)
        {
            if (baitSprite != null) baitSprite.enabled = visible;
        }

        // Gabe call this to get the bait world position
        public Vector3 GetBaitWorldPosition()
        {
            return transform.position;
        }
    }
}