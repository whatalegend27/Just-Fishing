using UnityEngine; // Using the Unity engine library

namespace Saif.GamePlay // Organizing my code into a specific "Saif" folder/namespace
{
    // DYNAMIC BINDING: The "virtual" keyword here allows subclasses to change this method at runtime
    public class FishingHook : MonoBehaviour // Main class inheriting from Unity's base script type
    {
        [Header("TESTING - Check this to test without a player")]
        [SerializeField] private bool debugMode = false; // Encapsulation: Private but editable in Unity UI

        [Header("Speeds")]
        [SerializeField] private float sinkSpeed = 3f;   // Speed the hook falls down
        [SerializeField] private float moveSpeed = 5f;   // Speed the hook moves left/right
        [SerializeField] private float reelSpeed = 7f;   // Speed the hook pulls back up

        [Header("Borders & Limits")]
        [SerializeField] private float maxDepth = -10f;  // The Y-coordinate limit for sinking
        [SerializeField] private float surfaceLevel = 1.77f; // Where the water meets the air
        [SerializeField] private float leftBorder = -3f; // Left boundary of the screen
        [SerializeField] private float rightBorder = 3f; // Right boundary of the screen

        [Header("Rod Tip Offset - Tweak these to align with rod tip")]
        [SerializeField] private Vector3 rodTipOffset = new Vector3(0.44f, 0f, 0f); // Spawning position relative to player

        [Header("Animation Delay")]
        [SerializeField] private float castAnimationDelay = 0.5f; // Wait time to match the player's arm movement

        [Header("References")]
        [SerializeField] private LineRenderer line;       // Reference to the visual fishing line
        [SerializeField] private SpriteRenderer hookSprite; // Reference to the hook's image

        [HideInInspector] public Vector3 debugSpawnOverride; // A public variable hidden from the Unity UI

        public bool IsHookCast => !isReadyToCast; // A "Getter" property to tell other scripts if we are currently fishing
        public bool HasFishAttached => hasCaughtFish;

        private Animator playerAnimator;     // Private reference to the player's animation controller
        private SpriteRenderer playerSprite; // Private reference to the player's image (for flipping)
        private Transform playerTransform;   // Private reference to the player's position

        protected bool hasCaughtFish = false; // Protected: Child classes (HeavyHook) can see and change this
        private bool isReadyToCast = true;    // Track if the hook is sitting at the rod tip
        private bool canReel = false;         // Track if the player is allowed to pull the hook back yet
        private bool animationDelayDone = false; // Track if the initial "wind-up" delay is finished
        private float castingTimer = 0f;      // Timer to count the delay seconds

        protected Transform caughtFishTransform;  // Slot 1 for a caught fish (Visible to children)
        protected Transform caughtFishTransform2; // Slot 2 for a caught fish (Used by HeavyHook)

        private Vector3 debugStartPosition; // Stores the original position for testing

        [HideInInspector] public float mobileHorizontal = 0f; // Stores left/right input from mobile buttons
        [HideInInspector] public bool mobileSpaceDown = false; // Stores "Tap" event from mobile
        [HideInInspector] public bool mobileSpaceHeld = false; // Stores "Holding" event from mobile
        [HideInInspector] public bool mobileSpaceUp = false;   // Stores "Letting go" event from mobile

        public void MobileCastReelPress()   { mobileSpaceDown = true; mobileSpaceHeld = true; } // Bridge: Triggered by UI Button Down
        public void MobileCastReelRelease() { mobileSpaceHeld = false; mobileSpaceUp = true; } // Bridge: Triggered by UI Button Up
        public void MobileSetHorizontal(float value) { mobileHorizontal = value; } // Bridge: Updates X movement from UI

        private bool GetSpaceDown() => Input.GetKeyDown(KeyCode.Space) || mobileSpaceDown; // Checks if Keyboard OR Mobile "pressed"
        private bool GetSpaceHeld() => Input.GetKey(KeyCode.Space)     || mobileSpaceHeld; // Checks if Keyboard OR Mobile "is holding"
        private bool GetSpaceUp()   => Input.GetKeyUp(KeyCode.Space)   || mobileSpaceUp;   // Checks if Keyboard OR Mobile "released"
        private float GetHorizontal() // Merges Keyboard and Mobile movement into one number
        {
            float kb = Input.GetAxis("Horizontal"); // Get keyboard A/D or Left/Right
            return Mathf.Abs(kb) > 0.01f ? kb : mobileHorizontal; // If keyboard is idle, use mobile input
        }

        public void SetDebugSpawnOverride(Vector3 position) { debugSpawnOverride = position; } // Test helper to force spawn point

        void Start() // Runs once when the game begins
        {
            debugStartPosition = (debugSpawnOverride != Vector3.zero) ? debugSpawnOverride : transform.position; // Set start point
            SetVisuals(false); // Make hook invisible at start
            if (!debugMode) FindPlayerReference(); // If not in test mode, find the player automatically
        }

        private void FindPlayerReference() // Dynamic Search: Finds the player using code instead of drag-and-drop
        {
            GameObject playerObj = GameObject.FindWithTag("Player"); // Look for an object tagged "Player"
            if (playerObj != null) // If we found them
            {
                playerTransform = playerObj.transform; // Grab their position
                playerAnimator  = playerObj.GetComponent<Animator>(); // Grab their animations
                playerSprite    = playerObj.GetComponent<SpriteRenderer>(); // Grab their sprite
                return; // Stop searching
            }

            Animator[] anims = Object.FindObjectsByType<Animator>(FindObjectsSortMode.None); // Fallback: Search all animators
            foreach (Animator a in anims) // Loop through every animator found
                foreach (var param in a.parameters) // Loop through their internal settings
                    if (param.name == "IsCasting") // If they have an "IsCasting" setting, they are the player
                    {
                        playerAnimator  = a; // Set animator
                        playerTransform = a.transform; // Set transform
                        playerSprite    = a.GetComponent<SpriteRenderer>(); // Set sprite
                        return; // Stop searching
                    }
        }

        private Vector3 GetRodTipWorldPos() // Calculates where the hook should be relative to the rod
        {
            if (debugMode) return debugStartPosition; // In debug, stay at the fixed start point
            if (playerTransform == null) return transform.position; // Safety check
            float direction = (playerSprite != null && playerSprite.flipX) ? -1f : 1f; // Flip the hook spawn if player faces left
            return playerTransform.position + new Vector3(rodTipOffset.x * direction, rodTipOffset.y, rodTipOffset.z); // Apply offset
        }

        void LateUpdate() // Runs every frame AFTER movement is calculated (good for line renderers)
        {
            if (debugMode) HandleDebugMode(); // Run simple logic for testing
            else           HandleNormalMode(); // Run full gameplay logic

            mobileSpaceDown = false; // Reset the "Tap" flag so it doesn't repeat next frame
            mobileSpaceUp   = false; // Reset the "Release" flag
        }

        private void HandleDebugMode() // Simplified fishing logic for developers
        {
            Vector3 rodTipPos = debugStartPosition; // Use the fixed test position
            if (isReadyToCast) // If hook is waiting to be thrown
            {
                transform.position = rodTipPos; // Keep it at the tip
                SetVisuals(false); // Keep it invisible
                if (GetSpaceDown()) { isReadyToCast = false; canReel = false; SetVisuals(true); } // Press Space to start
                return; 
            }
            UpdateLine(rodTipPos); // Draw the line
            HandleFishingPhysics(rodTipPos); // Handle movement
        }

        private void HandleNormalMode() // Real gameplay logic
        {
            bool isCasting = playerAnimator != null && playerAnimator.GetBool("IsCasting"); // Check player's animation state

            if (!isCasting) // If player is just walking or idle
            {
                ResetHook(); // Clean up everything
                SetVisuals(false); // Hide hook
                transform.position = GetRodTipWorldPos(); // Stick to rod tip
                return;
            }

            if (!animationDelayDone) // If we just started casting, wait for the arm to swing
            {
                castingTimer += Time.deltaTime; // Count up
                transform.position = GetRodTipWorldPos(); // Stick to rod
                SetVisuals(false); // Stay invisible
                if (castingTimer >= castAnimationDelay) animationDelayDone = true; // Delay over
                return;
            }

            Vector3 rodTipPos = GetRodTipWorldPos(); // Get current rod position

            if (isReadyToCast) // Hook is ready but hasn't dropped yet
            {
                transform.position = rodTipPos; // Stay at tip
                SetVisuals(true); // Now show the hook
                SetLineVisible(false); // Hide line until it actually drops
                if (GetSpaceDown()) { isReadyToCast = false; canReel = false; SetVisuals(true); SetLineVisible(true); } // Drop it!
                return;
            }

            UpdateLine(rodTipPos); // Draw line while falling
            HandleFishingPhysics(rodTipPos); // Move hook
        }

        private void HandleFishingPhysics(Vector3 rodTipPos) // Controls the actual movement in the water
        {
            if (!canReel && GetSpaceUp()) canReel = true; // Once you let go of Space, you are allowed to reel back in

            float h    = GetHorizontal(); // Get A/D or Mobile Left/Right
            float newX = transform.position.x; // Store current X
            float newY = transform.position.y; // Store current Y

            if (canReel && GetSpaceHeld()) // If you are holding Space to pull the fish up
            {
                newY += reelSpeed * Time.deltaTime; // Move UP
                float distanceRatio   = 1f - Mathf.Clamp01((rodTipPos.y - newY) / Mathf.Abs(maxDepth)); // Calculate how close to surface we are
                float homingStrength  = Mathf.Lerp(0.5f, moveSpeed * 2f, distanceRatio); // Pull harder toward the rod as we get closer
                newX = Mathf.MoveTowards(newX, rodTipPos.x, homingStrength * Time.deltaTime); // Auto-align X with the rod
                newX += h * (moveSpeed * 0.5f) * (1f - distanceRatio * 0.8f) * Time.deltaTime; // Allow small manual X adjustments while reeling

                if (newY >= rodTipPos.y) // If we hit the rod tip
                {
                    newY = rodTipPos.y; // Snap to position
                    if (hasCaughtFish) CollectFish(); // If there's a fish, delete it (score point)
                    ResetHook(); // Start over
                    return;
                }
            }
            else // If we are sinking down
            {
                newX += h * moveSpeed * Time.deltaTime; // Full manual control of X
                if (newY > maxDepth) newY -= sinkSpeed * Time.deltaTime; // Gravity pulls it down to max depth
            }

            newX = Mathf.Clamp(newX, leftBorder, rightBorder); // Keep hook inside screen walls
            newY = Mathf.Clamp(newY, maxDepth, rodTipPos.y);   // Keep hook between floor and rod
            transform.position = new Vector3(newX, newY, transform.position.z); // Apply the new position
        }

        private void UpdateLine(Vector3 rodTipPos) // Draws the line between two points
        {
            if (line != null && line.enabled) // If line exists and is active
            {
                line.SetPosition(0, rodTipPos); // Point A: Rod Tip
                line.SetPosition(1, transform.position); // Point B: Hook
            }
        }

        private void SetVisuals(bool visible) // Helper to toggle the hook and line together
        {
            if (hookSprite != null) hookSprite.enabled = visible; // Toggle hook image
            if (line != null) line.enabled = visible; // Toggle line visual
        }

        private void SetLineVisible(bool visible) // Helper specifically for the line
        {
            if (line != null) line.enabled = visible; // Toggle line only
        }

        private void OnTriggerEnter2D(Collider2D collision) // Runs when something hits the hook
        {
            if (!collision.CompareTag("Fish")) return; // If it's not a fish, ignore it
            AttachFish(collision.transform); // DYNAMIC BINDING CALL: Run the attachment logic
        }

        // DYNAMIC BINDING (POLYMOPHISM): The "virtual" keyword allows the HeavyHook to override this
        protected virtual void AttachFish(Transform fish) 
        {
            if (caughtFishTransform != null) return; // Base hook (SmallHook) only allows 1 fish

            caughtFishTransform = fish; // Store the fish
            hasCaughtFish = true; // Set the flag

            Component movement = fish.GetComponent("FishMovement"); // Search for the fish's AI script
            if (movement != null) (movement as MonoBehaviour).enabled = false; // Disable AI so the fish stops swimming

            fish.SetParent(this.transform); // Make the fish a child of the hook so it follows us up
            fish.localPosition = Vector3.zero; // Center the fish on the hook
            fish.localRotation = Quaternion.identity; // Straighten the fish
            Debug.Log("[FishingHook] base AttachFish — 1 fish max."); // Log for the teacher to see which version ran
        }

        protected void ResetHook() // Cleans the hook for the next cast
        {
            isReadyToCast = true; canReel = false; hasCaughtFish = false; // Reset flags
            animationDelayDone = false; castingTimer = 0f; // Reset timers
            SetVisuals(false); // Hide visuals

            if (caughtFishTransform != null) // If a fish was attached but not collected
            {
                Component m = caughtFishTransform.GetComponent("FishMovement"); // Find AI
                if (m != null) (m as MonoBehaviour).enabled = true; // Turn AI back on
                caughtFishTransform.SetParent(null); // Detach from hook
                caughtFishTransform = null; // Clear slot
            }

            if (caughtFishTransform2 != null) // Same for second slot (HeavyHook)
            {
                Component m = caughtFishTransform2.GetComponent("FishMovement"); 
                if (m != null) (m as MonoBehaviour).enabled = true;
                caughtFishTransform2.SetParent(null);
                caughtFishTransform2 = null;
            }
        }

        protected void CollectFish() // Runs when the fish reaches the surface
        {
            if (caughtFishTransform  != null) { Destroy(caughtFishTransform.gameObject);  caughtFishTransform  = null; } // Delete fish 1
            if (caughtFishTransform2 != null) { Destroy(caughtFishTransform2.gameObject); caughtFishTransform2 = null; } // Delete fish 2
            hasCaughtFish = false; // Reset caught flag
        }
    }
}