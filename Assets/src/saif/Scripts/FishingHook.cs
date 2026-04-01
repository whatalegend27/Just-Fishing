using UnityEngine;

namespace Saif.GamePlay
{
    public class FishingHook : MonoBehaviour
    {
        [Header("TESTING - Check this to test without a player")]
        public bool debugMode = false;

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
        public SpriteRenderer hookSprite; // drag the hook's SpriteRenderer here in inspector

        // Player references — found automatically at runtime
        private Animator playerAnimator;
        private SpriteRenderer playerSprite;
        private Transform playerTransform;

        // State
        private bool hasCaughtFish = false;
        private bool isReadyToCast = true;
        private bool canReel = false;
        private bool animationDelayDone = false;
        private float castingTimer = 0f;
        private Transform caughtFishTransform;

        // Starting position used by debug mode as the "rod tip"
        private Vector3 debugStartPosition;

        void Start()
        {
            debugStartPosition = transform.position;
            SetVisuals(false); // hide everything at start

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

            // Fallback — scan all animators for IsCasting parameter
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

        // ─── DEBUG MODE ──────────────────────────────────────────────────────────────
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

        // ─── NORMAL MODE ─────────────────────────────────────────────────────────────
        private void HandleNormalMode()
        {
            bool isCasting = playerAnimator != null && playerAnimator.GetBool("IsCasting");

            // Player stopped the animation — reset everything and hide
            if (!isCasting)
            {
                ResetHook();
                SetVisuals(false);
                transform.position = GetRodTipWorldPos();
                return;
            }

            // Animation just became active — start delay timer, keep hidden
            if (!animationDelayDone)
            {
                castingTimer += Time.deltaTime;
                transform.position = GetRodTipWorldPos();
                SetVisuals(false);

                if (castingTimer >= castAnimationDelay)
                    animationDelayDone = true;

                return;
            }

            // Delay done — show hook at rod tip, wait for Space
            Vector3 rodTipPos = GetRodTipWorldPos();

            if (isReadyToCast)
            {
                transform.position = rodTipPos;
                SetVisuals(true);  // hook visible at rod tip
                SetLineVisible(false); // but line stays hidden until cast

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

        // ─── SHARED PHYSICS ──────────────────────────────────────────────────────────
        private void HandleFishingPhysics(Vector3 rodTipPos)
        {
            if (!canReel && Input.GetKeyUp(KeyCode.Space)) canReel = true;

            float h = Input.GetAxis("Horizontal");
            float newX = transform.position.x;
            float newY = transform.position.y;

            if (canReel && Input.GetKey(KeyCode.Space))
            {
                newY += reelSpeed * Time.deltaTime;

                if (surfaceLevel - newY < 3f)
                {
                    newX = Mathf.MoveTowards(newX, rodTipPos.x, (moveSpeed * 0.5f) * Time.deltaTime);
                    newX += h * (moveSpeed * 0.5f) * Time.deltaTime;
                }
                else
                {
                    newX += h * moveSpeed * Time.deltaTime;
                }

                if (newY >= surfaceLevel)
                {
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
            newY = Mathf.Clamp(newY, maxDepth, surfaceLevel);
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
        }

        // ─── FISH COLLISION ──────────────────────────────────────────────────────────
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!hasCaughtFish && collision.CompareTag("Fish"))
            {
                hasCaughtFish = true;
                caughtFishTransform = collision.transform;
                Component movement = collision.GetComponent("FishMovement");
                if (movement != null) (movement as MonoBehaviour).enabled = false;
                caughtFishTransform.SetParent(this.transform);
                caughtFishTransform.localPosition = Vector3.zero;
                Debug.Log("Fish Hooked!");
            }
        }

        void CollectFish()
        {
            if (caughtFishTransform != null) Destroy(caughtFishTransform.gameObject);
            hasCaughtFish = false;
            caughtFishTransform = null;
        }
    }
}