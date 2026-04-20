using UnityEngine;

namespace Saif.GamePlay
{
    // ─── DYNAMIC BINDING NOTE ────────────────────────────────────────────────────
    // Dynamic binding is demonstrated through AttachFish being virtual.
    // The 1 vs 2 fish logic lives INSIDE AttachFish so that removing "virtual"
    // actually changes runtime behaviour:
    //
    // WITH "virtual":    HeavyHook.AttachFish runs → 2 fish caught
    // WITHOUT "virtual": base AttachFish always runs → 1 fish caught (SmallHook behaviour)
    //
    // Other dynamic examples:
    // 2. GetComponent("FishMovement") resolved at runtime based on what is attached.
    // 3. FindPlayerReference() scans Animators at runtime to find the player.
    // ─────────────────────────────────────────────────────────────────────────────
    public class FishingHook : MonoBehaviour
    {
        // ── TESTING ──────────────────────────────────────────────────────────────
        [Header("TESTING - Check this to test without a player")]
        [SerializeField] private bool debugMode = false;

        // ── SPEEDS ───────────────────────────────────────────────────────────────
        [Header("Speeds")]
        [SerializeField] private float sinkSpeed = 3f;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float reelSpeed = 7f;

        // ── BORDERS & LIMITS ─────────────────────────────────────────────────────
        [Header("Borders & Limits")]
        [SerializeField] private float maxDepth = -10f;
        [SerializeField] private float surfaceLevel = 1.77f;
        [SerializeField] private float leftBorder = -3f;
        [SerializeField] private float rightBorder = 3f;

        // ── ROD TIP OFFSET ───────────────────────────────────────────────────────
        [Header("Rod Tip Offset - Tweak these to align with rod tip")]
        [SerializeField] private Vector3 rodTipOffset = new Vector3(0.44f, 0f, 0f);

        // ── ANIMATION DELAY ──────────────────────────────────────────────────────
        [Header("Animation Delay")]
        [Tooltip("How long to wait after X is pressed before showing the hook. Adjust to match cast animation length.")]
        [SerializeField] private float castAnimationDelay = 0.5f;

        // ── REFERENCES ───────────────────────────────────────────────────────────
        [Header("References")]
        [SerializeField] private LineRenderer line;
        [SerializeField] private SpriteRenderer hookSprite;

        [HideInInspector] public Vector3 debugSpawnOverride;

        public bool IsHookCast => !isReadyToCast;

        private Animator playerAnimator;
        private SpriteRenderer playerSprite;
        private Transform playerTransform;

        protected bool hasCaughtFish = false;
        private bool isReadyToCast = true;
        private bool canReel = false;
        private bool animationDelayDone = false;
        private float castingTimer = 0f;

        protected Transform caughtFishTransform;
        protected Transform caughtFishTransform2;

        private Vector3 debugStartPosition;

        public void SetDebugSpawnOverride(Vector3 position) { debugSpawnOverride = position; }

        void Start()
        {
            debugStartPosition = (debugSpawnOverride != Vector3.zero)
                ? debugSpawnOverride
                : transform.position;

            SetVisuals(false);

            if (!debugMode)
                FindPlayerReference();
        }

        private void FindPlayerReference()
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
                playerAnimator = playerObj.GetComponent<Animator>();
                playerSprite = playerObj.GetComponent<SpriteRenderer>();
                return;
            }

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

        private Vector3 GetRodTipWorldPos()
        {
            if (debugMode) return debugStartPosition;
            if (playerTransform == null) return transform.position;

            float direction = (playerSprite != null && playerSprite.flipX) ? -1f : 1f;
            Vector3 offset = new Vector3(rodTipOffset.x * direction, rodTipOffset.y, rodTipOffset.z);
            return playerTransform.position + offset;
        }

        void LateUpdate()
        {
            if (debugMode)
                HandleDebugMode();
            else
                HandleNormalMode();
        }

        private void HandleDebugMode()
        {
            Vector3 rodTipPos = debugStartPosition;

            if (isReadyToCast)
            {
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

        private void HandleNormalMode()
        {
            bool isCasting = playerAnimator != null && playerAnimator.GetBool("IsCasting");

            if (!isCasting)
            {
                ResetHook();
                SetVisuals(false);
                transform.position = GetRodTipWorldPos();
                return;
            }

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

            if (isReadyToCast)
            {
                transform.position = rodTipPos;
                SetVisuals(true);
                SetLineVisible(false);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isReadyToCast = false;
                    canReel = false;
                    SetVisuals(true);
                    SetLineVisible(true);
                }
                return;
            }

            UpdateLine(rodTipPos);
            HandleFishingPhysics(rodTipPos);
        }

        private void HandleFishingPhysics(Vector3 rodTipPos)
        {
            if (!canReel && Input.GetKeyUp(KeyCode.Space)) canReel = true;

            float h = Input.GetAxis("Horizontal");
            float newX = transform.position.x;
            float newY = transform.position.y;

            if (canReel && Input.GetKey(KeyCode.Space))
            {
                newY += reelSpeed * Time.deltaTime;

                float distanceRatio = 1f - Mathf.Clamp01((rodTipPos.y - newY) / Mathf.Abs(maxDepth));
                float homingStrength = Mathf.Lerp(0.5f, moveSpeed * 2f, distanceRatio);
                newX = Mathf.MoveTowards(newX, rodTipPos.x, homingStrength * Time.deltaTime);
                newX += h * (moveSpeed * 0.5f) * (1f - distanceRatio * 0.8f) * Time.deltaTime;

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
                newX += h * moveSpeed * Time.deltaTime;
                if (newY > maxDepth) newY -= sinkSpeed * Time.deltaTime;
            }

            newX = Mathf.Clamp(newX, leftBorder, rightBorder);
            newY = Mathf.Clamp(newY, maxDepth, rodTipPos.y);
            transform.position = new Vector3(newX, newY, transform.position.z);
        }

        private void UpdateLine(Vector3 rodTipPos)
        {
            if (line != null && line.enabled)
            {
                line.SetPosition(0, rodTipPos);
                line.SetPosition(1, transform.position);
            }
        }

        private void SetVisuals(bool visible)
        {
            if (hookSprite != null) hookSprite.enabled = visible;
            if (line != null) line.enabled = visible;
        }

        private void SetLineVisible(bool visible)
        {
            if (line != null) line.enabled = visible;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Fish")) return;

            // One call — AttachFish itself decides how many fish are allowed.
            // WITH virtual:    the correct subclass version runs (SmallHook = 1, HeavyHook = 2)
            // WITHOUT virtual: this base version always runs → 1 fish only, no matter the prefab
            AttachFish(collision.transform);
        }

        // ── DYNAMIC BINDING ───────────────────────────────────────────────────────
        // Base behaviour = SmallHook: only 1 fish allowed.
        // HeavyHook overrides this to allow 2.
        // Remove "virtual" → HeavyHook override is ignored → base always runs → 1 fish only.
        protected virtual void AttachFish(Transform fish)
        {
            // Already holding a fish — SmallHook only takes 1
            if (caughtFishTransform != null) return;

            caughtFishTransform = fish;
            hasCaughtFish = true;

            Component movement = fish.GetComponent("FishMovement");
            if (movement != null) (movement as MonoBehaviour).enabled = false;

            fish.SetParent(this.transform);
            fish.localPosition = Vector3.zero;
            fish.localRotation = Quaternion.identity;

            Debug.Log("[FishingHook] base AttachFish — SmallHook behaviour, 1 fish max. " +
                      "If you see this on a HeavyHook prefab, virtual was removed!");
        }

        protected void ResetHook()
        {
            isReadyToCast = true;
            canReel = false;
            hasCaughtFish = false;
            animationDelayDone = false;
            castingTimer = 0f;
            SetVisuals(false);

            if (caughtFishTransform != null)
            {
                Component movement = caughtFishTransform.GetComponent("FishMovement");
                if (movement != null) (movement as MonoBehaviour).enabled = true;
                caughtFishTransform.SetParent(null);
                caughtFishTransform = null;
            }

            if (caughtFishTransform2 != null)
            {
                Component movement = caughtFishTransform2.GetComponent("FishMovement");
                if (movement != null) (movement as MonoBehaviour).enabled = true;
                caughtFishTransform2.SetParent(null);
                caughtFishTransform2 = null;
            }
        }

        protected void CollectFish()
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