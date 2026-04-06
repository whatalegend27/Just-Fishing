using UnityEngine;

namespace Saif.GamePlay
{
    public class FishingHook : MonoBehaviour
    {
        [Header("TESTING - Check this to test without a player")]
        public bool debugMode = false;

        [Header("Hook Type")]
        public bool isHeavyHook = false;

        [Header("Speeds")]
        public float sinkSpeed = 2f;
        public float moveSpeed = 5f;
        public float reelSpeed = 3.5f;

        [Header("Borders & Limits")]
        public float maxDepth = -10f;
        public float surfaceLevel = 0f;
        public float leftBorder = -8f;
        public float rightBorder = 8f;

        [Header("Rod Tip Offset - Tweak these to align with rod tip")]
        public Vector3 rodTipOffset = new Vector3(0.44f, 0f, 0f);

        [Header("Animation Delay")]
        [Tooltip("How long to wait after X is pressed before showing the hook. Adjust to match cast animation length.")]
        public float castAnimationDelay = 0.5f;

        [Header("References")]
        public LineRenderer line;
        public SpriteRenderer hookSprite;

        [HideInInspector] public Vector3 debugSpawnOverride;

        public bool IsHookCast => !isReadyToCast;

        // Mobile input state — set by UI buttons
        private bool mobileSpaceHeld = false;
        private bool mobileSpaceDown = false;
        private bool mobileSpaceUp = false;
        private float mobileHorizontal = 0f;

        private Animator playerAnimator;
        private SpriteRenderer playerSprite;
        private Transform playerTransform;

        private bool hasCaughtFish = false;
        private bool isReadyToCast = true;
        private bool canReel = false;
        private bool animationDelayDone = false;
        private float castingTimer = 0f;

        private Transform caughtFishTransform;
        private Transform caughtFishTransform2;

        private Vector3 debugStartPosition;

        // ─── UI BUTTON METHODS ───────────────────────────────────────────────────────

        // Call this from the Cast/Reel button OnPointerDown
        public void OnCastReelPress()
        {
            mobileSpaceDown = true;
            mobileSpaceHeld = true;
        }

        // Call this from the Cast/Reel button OnPointerUp
        public void OnCastReelRelease()
        {
            mobileSpaceHeld = false;
            mobileSpaceUp = true;
        }

        // Call these from Left button OnPointerDown/OnPointerUp
        public void OnLeftPress() { mobileHorizontal = -1f; }
        public void OnLeftRelease() { if (mobileHorizontal < 0) mobileHorizontal = 0f; }

        // Call these from Right button OnPointerDown/OnPointerUp
        public void OnRightPress() { mobileHorizontal = 1f; }
        public void OnRightRelease() { if (mobileHorizontal > 0) mobileHorizontal = 0f; }

        // ─── INPUT HELPERS ───────────────────────────────────────────────────────────
        private bool GetSpaceDown() => Input.GetKeyDown(KeyCode.Space) || mobileSpaceDown;
        private bool GetSpaceHeld() => Input.GetKey(KeyCode.Space) || mobileSpaceHeld;
        private bool GetSpaceUp() => Input.GetKeyUp(KeyCode.Space) || mobileSpaceUp;
        private float GetHorizontal() => Input.GetAxis("Horizontal") + mobileHorizontal;

        void Start()
        {
            debugStartPosition = (debugSpawnOverride != Vector3.zero) ? debugSpawnOverride : transform.position;
            SetVisuals(false);

            if (!debugMode)
                FindPlayerReference();
        }

        void LateUpdate()
        {
            if (debugMode)
                HandleDebugMode();
            else
                HandleNormalMode();

            // Reset one-frame mobile inputs at end of frame
            mobileSpaceDown = false;
            mobileSpaceUp = false;
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

        // ─── DEBUG MODE ──────────────────────────────────────────────────────────────
        private void HandleDebugMode()
        {
            Vector3 rodTipPos = debugStartPosition;

            if (isReadyToCast)
            {
                transform.position = rodTipPos;
                SetVisuals(false);

                if (GetSpaceDown())
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

        // ─── NORMAL MODE ─────────────────────────────────────────────────────────────
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

                if (GetSpaceDown())
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

        // ─── SHARED PHYSICS ──────────────────────────────────────────────────────────
        private void HandleFishingPhysics(Vector3 rodTipPos)
        {
            if (!canReel && GetSpaceUp()) canReel = true;

            float h = GetHorizontal();
            float newX = transform.position.x;
            float newY = transform.position.y;

            if (canReel && GetSpaceHeld())
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

        // ─── FISH COLLISION ──────────────────────────────────────────────────────────
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Fish")) return;

            if (!isHeavyHook)
            {
                if (caughtFishTransform == null)
                {
                    AttachFish(collision.transform, ref caughtFishTransform);
                    hasCaughtFish = true;
                }
            }
            else
            {
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

        private void AttachFish(Transform fish, ref Transform slot)
        {
            slot = fish;
            Component movement = fish.GetComponent("FishMovement");
            if (movement != null) (movement as MonoBehaviour).enabled = false;
            fish.SetParent(this.transform);
            fish.localPosition = Vector3.zero;
            fish.localRotation = Quaternion.identity;
            Debug.Log("Fish Hooked!");
        }

        // ─── RESET ───────────────────────────────────────────────────────────────────
        void ResetHook()
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

        void CollectFish()
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