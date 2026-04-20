using UnityEngine;

namespace Saif.GamePlay
{
    // ─── DYNAMIC BINDING NOTE ────────────────────────────────────────────────────
    // WITH "virtual":    HeavyHook.AttachFish runs → 2 fish caught
    // WITHOUT "virtual": base AttachFish always runs → 1 fish caught (SmallHook behaviour)
    // ─────────────────────────────────────────────────────────────────────────────
    public class FishingHook : MonoBehaviour
    {
        [Header("TESTING - Check this to test without a player")]
        [SerializeField] private bool debugMode = false;

        [Header("Speeds")]
        [SerializeField] private float sinkSpeed = 3f;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float reelSpeed = 7f;

        [Header("Borders & Limits")]
        [SerializeField] private float maxDepth = -10f;
        [SerializeField] private float surfaceLevel = 1.77f;
        [SerializeField] private float leftBorder = -3f;
        [SerializeField] private float rightBorder = 3f;

        [Header("Rod Tip Offset - Tweak these to align with rod tip")]
        [SerializeField] private Vector3 rodTipOffset = new Vector3(0.44f, 0f, 0f);

        [Header("Animation Delay")]
        [Tooltip("How long to wait after X is pressed before showing the hook.")]
        [SerializeField] private float castAnimationDelay = 0.5f;

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

        // ── MOBILE INPUT FLAGS ────────────────────────────────────────────────
        // Set by MobileInputBridge each frame — read alongside keyboard input
        [HideInInspector] public float mobileHorizontal = 0f;
        [HideInInspector] public bool mobileSpaceDown = false;
        [HideInInspector] public bool mobileSpaceHeld = false;
        [HideInInspector] public bool mobileSpaceUp = false;

        // Called by MobileInputBridge when Cast/Reel button is pressed
        public void MobileCastReelPress()   { mobileSpaceDown = true; mobileSpaceHeld = true; }
        // Called by MobileInputBridge when Cast/Reel button is released
        public void MobileCastReelRelease() { mobileSpaceHeld = false; mobileSpaceUp = true; }
        // Called by MobileInputBridge every frame while left/right is held
        public void MobileSetHorizontal(float value) { mobileHorizontal = value; }

        // Helpers — check keyboard OR mobile so both work at the same time
        private bool GetSpaceDown() => Input.GetKeyDown(KeyCode.Space) || mobileSpaceDown;
        private bool GetSpaceHeld() => Input.GetKey(KeyCode.Space)     || mobileSpaceHeld;
        private bool GetSpaceUp()   => Input.GetKeyUp(KeyCode.Space)   || mobileSpaceUp;
        private float GetHorizontal()
        {
            float kb = Input.GetAxis("Horizontal");
            return Mathf.Abs(kb) > 0.01f ? kb : mobileHorizontal;
        }

        public void SetDebugSpawnOverride(Vector3 position) { debugSpawnOverride = position; }

        void Start()
        {
            debugStartPosition = (debugSpawnOverride != Vector3.zero)
                ? debugSpawnOverride : transform.position;

            SetVisuals(false);
            if (!debugMode) FindPlayerReference();
        }

        private void FindPlayerReference()
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
                playerAnimator  = playerObj.GetComponent<Animator>();
                playerSprite    = playerObj.GetComponent<SpriteRenderer>();
                return;
            }

            Animator[] anims = Object.FindObjectsByType<Animator>(FindObjectsSortMode.None);
            foreach (Animator a in anims)
                foreach (var param in a.parameters)
                    if (param.name == "IsCasting")
                    {
                        playerAnimator  = a;
                        playerTransform = a.transform;
                        playerSprite    = a.GetComponent<SpriteRenderer>();
                        return;
                    }

            Debug.LogWarning("FishingHook: Could not find player with IsCasting parameter!");
        }

        private Vector3 GetRodTipWorldPos()
        {
            if (debugMode) return debugStartPosition;
            if (playerTransform == null) return transform.position;
            float direction = (playerSprite != null && playerSprite.flipX) ? -1f : 1f;
            return playerTransform.position + new Vector3(rodTipOffset.x * direction, rodTipOffset.y, rodTipOffset.z);
        }

        void LateUpdate()
        {
            if (debugMode) HandleDebugMode();
            else           HandleNormalMode();

            // Clear one-frame mobile flags
            mobileSpaceDown = false;
            mobileSpaceUp   = false;
        }

        private void HandleDebugMode()
        {
            Vector3 rodTipPos = debugStartPosition;

            if (isReadyToCast)
            {
                transform.position = rodTipPos;
                SetVisuals(false);
                if (GetSpaceDown()) { isReadyToCast = false; canReel = false; SetVisuals(true); }
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
                if (castingTimer >= castAnimationDelay) animationDelayDone = true;
                return;
            }

            Vector3 rodTipPos = GetRodTipWorldPos();

            if (isReadyToCast)
            {
                transform.position = rodTipPos;
                SetVisuals(true);
                SetLineVisible(false);
                if (GetSpaceDown()) { isReadyToCast = false; canReel = false; SetVisuals(true); SetLineVisible(true); }
                return;
            }

            UpdateLine(rodTipPos);
            HandleFishingPhysics(rodTipPos);
        }

        private void HandleFishingPhysics(Vector3 rodTipPos)
        {
            if (!canReel && GetSpaceUp()) canReel = true;

            float h    = GetHorizontal();
            float newX = transform.position.x;
            float newY = transform.position.y;

            if (canReel && GetSpaceHeld())
            {
                newY += reelSpeed * Time.deltaTime;
                float distanceRatio   = 1f - Mathf.Clamp01((rodTipPos.y - newY) / Mathf.Abs(maxDepth));
                float homingStrength  = Mathf.Lerp(0.5f, moveSpeed * 2f, distanceRatio);
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
            AttachFish(collision.transform);
        }

        // ── DYNAMIC BINDING ───────────────────────────────────────────────────
        // Base = SmallHook (1 fish). HeavyHook overrides to allow 2.
        // Remove "virtual" → override ignored → always 1 fish.
        protected virtual void AttachFish(Transform fish)
        {
            if (caughtFishTransform != null) return;

            caughtFishTransform = fish;
            hasCaughtFish = true;

            Component movement = fish.GetComponent("FishMovement");
            if (movement != null) (movement as MonoBehaviour).enabled = false;

            fish.SetParent(this.transform);
            fish.localPosition = Vector3.zero;
            fish.localRotation = Quaternion.identity;
            Debug.Log("[FishingHook] base AttachFish — 1 fish max.");
        }

        protected void ResetHook()
        {
            isReadyToCast = true; canReel = false; hasCaughtFish = false;
            animationDelayDone = false; castingTimer = 0f;
            SetVisuals(false);

            if (caughtFishTransform != null)
            {
                Component m = caughtFishTransform.GetComponent("FishMovement");
                if (m != null) (m as MonoBehaviour).enabled = true;
                caughtFishTransform.SetParent(null);
                caughtFishTransform = null;
            }

            if (caughtFishTransform2 != null)
            {
                Component m = caughtFishTransform2.GetComponent("FishMovement");
                if (m != null) (m as MonoBehaviour).enabled = true;
                caughtFishTransform2.SetParent(null);
                caughtFishTransform2 = null;
            }
        }

        protected void CollectFish()
        {
            if (caughtFishTransform  != null) { Destroy(caughtFishTransform.gameObject);  caughtFishTransform  = null; }
            if (caughtFishTransform2 != null) { Destroy(caughtFishTransform2.gameObject); caughtFishTransform2 = null; }
            hasCaughtFish = false;
        }
    }
}











