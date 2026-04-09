using UnityEngine;

namespace Saif.GamePlay
{
    // ─── DYNAMIC BINDING NOTE ────────────────────────────────────────────────────
    // Dynamic binding is demonstrated in this script through virtual/override methods
    // and runtime component lookups. Specifically:
    // 1. GetComponent<FishMovement>() at runtime — the exact component resolved depends
    //    on what is attached to the fish GameObject, not known until the game runs.
    // 2. The fishing physics behavior (HandleFishingPhysics) is called through a
    //    runtime check (debugMode) — which version of the logic runs is decided
    //    dynamically at runtime, not compile time.
    // 3. FindPlayerReference() scans all Animators at runtime to find the correct one —
    //    the binding to the player happens dynamically when the scene loads.
    // ─────────────────────────────────────────────────────────────────────────────
    public class FishingHook : MonoBehaviour
    {
        // ── TESTING ──────────────────────────────────────────────────────────────
        // SerializeField lets us see and toggle this in the Inspector without making
        // it public — keeps encapsulation intact while remaining editable in the editor
        [Header("TESTING - Check this to test without a player")]
        [SerializeField] private bool debugMode = false;

        // ── HOOK TYPE ─────────────────────────────────────────────────────────────
        // Set by HookSelector at runtime via SetHookType() — not public directly
        [Header("Hook Type")]
        [SerializeField] private bool isHeavyHook = false;

        // ── SPEEDS ───────────────────────────────────────────────────────────────
        // All speed values are serialized so designers can tweak them in the Inspector
        // without needing to modify code
        [Header("Speeds")]
        [SerializeField] private float sinkSpeed = 3f;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float reelSpeed = 7f;

        // ── BORDERS & LIMITS ─────────────────────────────────────────────────────
        // Define the playable area for the hook — clamped in HandleFishingPhysics
        [Header("Borders & Limits")]
        [SerializeField] private float maxDepth = -10f;
        [SerializeField] private float surfaceLevel = 1.77f;
        [SerializeField] private float leftBorder = -3f;
        [SerializeField] private float rightBorder = 3f;

        // ── ROD TIP OFFSET ───────────────────────────────────────────────────────
        // Offset from the player's position to the tip of the fishing rod sprite
        // X flips automatically when the player faces left (see GetRodTipWorldPos)
        [Header("Rod Tip Offset - Tweak these to align with rod tip")]
        [SerializeField] private Vector3 rodTipOffset = new Vector3(0.44f, 0f, 0f);

        // ── ANIMATION DELAY ──────────────────────────────────────────────────────
        // How long to wait after IsCasting becomes true before showing the hook
        // Gives the cast animation time to play before the hook appears
        [Header("Animation Delay")]
        [Tooltip("How long to wait after X is pressed before showing the hook. Adjust to match cast animation length.")]
        [SerializeField] private float castAnimationDelay = 0.5f;

        // ── REFERENCES ───────────────────────────────────────────────────────────
        // Visual components — serialized so they can be assigned in the Inspector
        [Header("References")]
        [SerializeField] private LineRenderer line;
        [SerializeField] private SpriteRenderer hookSprite;

        // ── HIDDEN INSPECTOR FIELDS ──────────────────────────────────────────────
        // Set by HookSelector at spawn time — hidden from Inspector to avoid confusion
        // HideInInspector keeps it private-feeling while still accessible from other scripts
        [HideInInspector] public Vector3 debugSpawnOverride;

        // ── PUBLIC PROPERTY ───────────────────────────────────────────────────────
        // Read-only property — external scripts can check if the hook is cast
        // but cannot change the state directly (encapsulation)
        public bool IsHookCast => !isReadyToCast;

        // ── PLAYER REFERENCES ─────────────────────────────────────────────────────
        // Found at runtime via FindPlayerReference() — DYNAMIC BINDING
        // These are private because nothing outside this script needs to access them
        private Animator playerAnimator;
        private SpriteRenderer playerSprite;
        private Transform playerTransform;

        // ── STATE VARIABLES ───────────────────────────────────────────────────────
        // All state is private — only this script controls the fishing logic
        private bool hasCaughtFish = false;
        private bool isReadyToCast = true;
        private bool canReel = false;
        private bool animationDelayDone = false;
        private float castingTimer = 0f;

        // ── FISH SLOTS ────────────────────────────────────────────────────────────
        // Small hook uses only slot 1, heavy hook can use both slots simultaneously
        private Transform caughtFishTransform;
        private Transform caughtFishTransform2;

        // ── DEBUG START POSITION ──────────────────────────────────────────────────
        // Used in debug mode as the "rod tip" since there is no player in the scene
        private Vector3 debugStartPosition;

        // ── PUBLIC SETTERS ────────────────────────────────────────────────────────
        // These replace direct public variable access from HookSelector
        // Proper encapsulation — external scripts call methods, not set variables directly

        /// <summary>
        /// Called by HookSelector to define what type of hook this is.
        /// Heavy hook can catch 2 fish, small hook catches 1.
        /// </summary>
        public void SetHookType(bool heavy)
        {
            isHeavyHook = heavy;
        }

        /// <summary>
        /// Called by HookSelector to set the debug spawn position override.
        /// Only used when debugMode is true and there is no player in the scene.
        /// </summary>
        public void SetDebugSpawnOverride(Vector3 position)
        {
            debugSpawnOverride = position;
        }

        // ─────────────────────────────────────────────────────────────────────────
        void Start()
        {
            // Use the override position if HookSelector provided one,
            // otherwise fall back to this object's own world position
            debugStartPosition = (debugSpawnOverride != Vector3.zero)
                ? debugSpawnOverride
                : transform.position;

            // Hide hook and line until the player enters casting animation
            SetVisuals(false);

            // Only search for the player in normal mode —
            // debug mode has no player in the scene
            if (!debugMode)
                FindPlayerReference();
        }

        // ── DYNAMIC BINDING: FindPlayerReference ──────────────────────────────────
        // This method demonstrates dynamic binding — it searches all Animators
        // in the scene at runtime and binds to whichever one has the "IsCasting"
        // parameter. The exact object is not known until the game runs.
        private void FindPlayerReference()
        {
            // First try finding by tag — fastest lookup
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
                playerAnimator = playerObj.GetComponent<Animator>();
                playerSprite = playerObj.GetComponent<SpriteRenderer>();
                return;
            }

            // Fallback — scan every Animator in the scene for "IsCasting" parameter
            // This handles cases where the player is not tagged correctly
            // DYNAMIC BINDING: the correct Animator is resolved at runtime
            Animator[] anims = Object.FindObjectsByType<Animator>(FindObjectsSortMode.None);
            foreach (Animator a in anims)
            {
                foreach (var param in a.parameters)
                {
                    if (param.name == "IsCasting")
                    {
                        playerAnimator = a;
                        playerTransform = a.transform;
                        playerSprite = a.GetComponent<SpriteRenderer>();
                        return;
                    }
                }
            }

            Debug.LogWarning("FishingHook: Could not find player with IsCasting parameter!");
        }

        // ── ROD TIP POSITION ──────────────────────────────────────────────────────
        // Calculates the world position of the rod tip each frame
        // Handles player facing direction by flipping the X offset
        private Vector3 GetRodTipWorldPos()
        {
            if (debugMode) return debugStartPosition;
            if (playerTransform == null) return transform.position;

            // Flip X offset when player is facing left (spriteRenderer.flipX is true)
            float direction = (playerSprite != null && playerSprite.flipX) ? -1f : 1f;
            Vector3 offset = new Vector3(rodTipOffset.x * direction, rodTipOffset.y, rodTipOffset.z);

            return playerTransform.position + offset;
        }

        void LateUpdate()
        {
            // LateUpdate ensures hook position is set AFTER player animation updates
            // preventing one-frame lag between player movement and hook position

            // DYNAMIC BINDING: which mode runs is decided at runtime based on debugMode
            if (debugMode)
                HandleDebugMode();
            else
                HandleNormalMode();
        }

        // ── DEBUG MODE ────────────────────────────────────────────────────────────
        // Runs when no player is in the scene — used for isolated hook testing
        // Hook starts at its spawn position and behaves as normal
        private void HandleDebugMode()
        {
            Vector3 rodTipPos = debugStartPosition;

            if (isReadyToCast)
            {
                // Keep hook at spawn position until Space is pressed
                transform.position = rodTipPos;
                SetVisuals(false);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isReadyToCast = false;
                    canReel = false;
                    SetVisuals(true);
                }
                return;
            }

            UpdateLine(rodTipPos);
            HandleFishingPhysics(rodTipPos);
        }

        // ── NORMAL MODE ───────────────────────────────────────────────────────────
        // Runs in the full game with the player present
        // Gates all hook behavior behind the IsCasting animation state
        private void HandleNormalMode()
        {
            // Read the animator bool — DYNAMIC BINDING: resolved at runtime
            bool isCasting = playerAnimator != null && playerAnimator.GetBool("IsCasting");

            // If player exited casting animation, reset and hide hook
            if (!isCasting)
            {
                ResetHook();
                SetVisuals(false);
                transform.position = GetRodTipWorldPos();
                return;
            }

            // Wait for cast animation to finish playing before showing hook
            if (!animationDelayDone)
            {
                castingTimer += Time.deltaTime;
                transform.position = GetRodTipWorldPos();
                SetVisuals(false);

                if (castingTimer >= castAnimationDelay)
                    animationDelayDone = true;

                return;
            }

            Vector3 rodTipPos = GetRodTipWorldPos();

            // Hook is visible at rod tip — waiting for player to press Space to cast
            if (isReadyToCast)
            {
                transform.position = rodTipPos;
                SetVisuals(true);
                SetLineVisible(false); // line hidden until hook is actually cast

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isReadyToCast = false;
                    canReel = false;
                    SetVisuals(true);
                    SetLineVisible(true);
                }
                return;
            }

            // Hook is in the water — update line and run physics
            UpdateLine(rodTipPos);
            HandleFishingPhysics(rodTipPos);
        }

        // ── FISHING PHYSICS ───────────────────────────────────────────────────────
        // Core movement logic — shared between debug and normal mode
        // Handles sinking, horizontal movement, and reeling back to rod tip
        private void HandleFishingPhysics(Vector3 rodTipPos)
        {
            // Unlock reeling only after Space has been fully released post-cast
            // Prevents the hook from instantly reeling the moment it's cast
            if (!canReel && Input.GetKeyUp(KeyCode.Space)) canReel = true;

            float h = Input.GetAxis("Horizontal");
            float newX = transform.position.x;
            float newY = transform.position.y;

            if (canReel && Input.GetKey(KeyCode.Space))
            {
                // Reel the hook upward
                newY += reelSpeed * Time.deltaTime;

                // distanceRatio approaches 1 as hook nears the rod tip
                // Used to gradually strengthen homing and reduce player control near surface
                float distanceRatio = 1f - Mathf.Clamp01((rodTipPos.y - newY) / Mathf.Abs(maxDepth));

                // Homing strength increases as hook gets closer to rod tip
                // Ensures hook always snaps cleanly home at the end of the reel
                float homingStrength = Mathf.Lerp(0.5f, moveSpeed * 2f, distanceRatio);
                newX = Mathf.MoveTowards(newX, rodTipPos.x, homingStrength * Time.deltaTime);

                // Player can nudge hook horizontally while reeling
                // But influence decreases near surface so homing takes over
                newX += h * (moveSpeed * 0.5f) * (1f - distanceRatio * 0.8f) * Time.deltaTime;

                // Hook reached the rod tip — collect fish if any and reset
                if (newY >= rodTipPos.y)
                {
                    newY = rodTipPos.y;
                    if (hasCaughtFish) CollectFish();
                    ResetHook();
                    return;
                }
            }
            else
            {
                // Hook is sinking — player has full horizontal control
                newX += h * moveSpeed * Time.deltaTime;
                if (newY > maxDepth) newY -= sinkSpeed * Time.deltaTime;
            }

            // Clamp to playable area
            newX = Mathf.Clamp(newX, leftBorder, rightBorder);
            newY = Mathf.Clamp(newY, maxDepth, rodTipPos.y);
            transform.position = new Vector3(newX, newY, transform.position.z);
        }

        // ── LINE RENDERER ─────────────────────────────────────────────────────────
        // Draws the fishing line from rod tip to hook position every frame
        private void UpdateLine(Vector3 rodTipPos)
        {
            if (line != null && line.enabled)
            {
                line.SetPosition(0, rodTipPos); // line start — rod tip
                line.SetPosition(1, transform.position); // line end — hook
            }
        }

        // ── VISUALS ───────────────────────────────────────────────────────────────
        // Toggles both hook sprite and line renderer together
        // Keeps visual state consistent — they always show/hide as a pair
        private void SetVisuals(bool visible)
        {
            if (hookSprite != null) hookSprite.enabled = visible;
            if (line != null) line.enabled = visible;
        }

        // Toggles only the line — used when hook is visible at rod tip pre-cast
        private void SetLineVisible(bool visible)
        {
            if (line != null) line.enabled = visible;
        }

        // ── FISH COLLISION ────────────────────────────────────────────────────────
        // DYNAMIC BINDING: GetComponent("FishMovement") resolves at runtime
        // The exact FishMovement component found depends on the fish GameObject
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Fish")) return;

            if (!isHeavyHook)
            {
                // Small hook — one fish slot only
                if (caughtFishTransform == null)
                {
                    AttachFish(collision.transform, ref caughtFishTransform);
                    hasCaughtFish = true;
                }
            }
            else
            {
                // Heavy hook — two fish slots
                // First slot takes priority, second slot fills after
                if (caughtFishTransform == null)
                {
                    AttachFish(collision.transform, ref caughtFishTransform);
                    hasCaughtFish = true;
                }
                else if (caughtFishTransform2 == null)
                {
                    AttachFish(collision.transform, ref caughtFishTransform2);
                }
            }
        }

        // Stops the fish from swimming and parents it to the hook
        private void AttachFish(Transform fish, ref Transform slot)
        {
            slot = fish;

            // DYNAMIC BINDING: resolves FishMovement component at runtime
            // The component type is looked up by name — bound dynamically
            Component movement = fish.GetComponent("FishMovement");
            if (movement != null) (movement as MonoBehaviour).enabled = false;

            // Parent fish to hook so it follows automatically
            fish.SetParent(this.transform);
            fish.localPosition = Vector3.zero;
            fish.localRotation = Quaternion.identity; // keep fish upright
            Debug.Log("Fish Hooked!");
        }

        // ── RESET ─────────────────────────────────────────────────────────────────
        // Resets all state back to ready-to-cast
        // Also releases any attached fish back to their own control
        private void ResetHook()
        {
            isReadyToCast = true;
            canReel = false;
            hasCaughtFish = false;
            animationDelayDone = false;
            castingTimer = 0f;
            SetVisuals(false);

            // Release first fish slot
            if (caughtFishTransform != null)
            {
                Component movement = caughtFishTransform.GetComponent("FishMovement");
                if (movement != null) (movement as MonoBehaviour).enabled = true;
                caughtFishTransform.SetParent(null);
                caughtFishTransform = null;
            }

            // Release second fish slot (heavy hook only)
            if (caughtFishTransform2 != null)
            {
                Component movement = caughtFishTransform2.GetComponent("FishMovement");
                if (movement != null) (movement as MonoBehaviour).enabled = true;
                caughtFishTransform2.SetParent(null);
                caughtFishTransform2 = null;
            }
        }

        // ── COLLECT FISH ──────────────────────────────────────────────────────────
        // Destroys caught fish when hook reaches the surface
        // Called just before ResetHook when the hook returns home with a fish
        private void CollectFish()
        {
            if (caughtFishTransform != null)
            {
                Destroy(caughtFishTransform.gameObject);
                caughtFishTransform = null;
            }
            if (caughtFishTransform2 != null)
            {
                Destroy(caughtFishTransform2.gameObject);
                caughtFishTransform2 = null;
            }
            hasCaughtFish = false;
        }
    }
}