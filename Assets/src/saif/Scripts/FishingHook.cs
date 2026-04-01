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
        public Vector3 rodTipOffset = new Vector3(0.15f, 0.35f, 0f);

        private Animator playerAnimator;
        private SpriteRenderer playerSprite;
        private Transform playerTransform;

        public LineRenderer line;

        private bool hasCaughtFish = false;
        private bool isReadyToCast = true;
        private bool canReel = false;
        private Transform caughtFishTransform;

        void Start()
        {
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
            }

            // Fallback — scan all animators for IsCasting parameter
            if (playerAnimator == null)
            {
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
            }
        }

        // Returns the rod tip position in world space, flipping X when player faces left
        private Vector3 GetRodTipWorldPos()
        {
            if (playerTransform == null) return transform.position;
            float direction = (playerSprite != null && playerSprite.flipX) ? -1f : 1f;
            Vector3 offset = new Vector3(rodTipOffset.x * direction, rodTipOffset.y, rodTipOffset.z);
            return playerTransform.position + offset;
        }

        void LateUpdate()
        {
            // 1. Check if fishing animation is active
            bool isCasting = debugMode || (playerAnimator != null && playerAnimator.GetBool("IsCasting"));

            if (!isCasting)
            {
                if (!isReadyToCast) ResetHook();
                SetLineVisible(false);
                return;
            }

            // 2. Get rod tip every frame
            Vector3 rodTipPos = GetRodTipWorldPos();

            // 3. Before casting, hook sits on rod tip
            if (isReadyToCast)
            {
                transform.position = rodTipPos;
                SetLineVisible(false); // line hidden until cast

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isReadyToCast = false;
                    canReel = false;
                    SetLineVisible(true); // show line when cast
                }

                return;
            }

            // 4. Draw line from rod tip to hook
            if (line != null)
            {
                line.SetPosition(0, rodTipPos);
                line.SetPosition(1, transform.position);
            }

            // 5. Unlock reel after Space is released post-cast
            if (!canReel && Input.GetKeyUp(KeyCode.Space)) canReel = true;

            float h = Input.GetAxis("Horizontal");
            float newX = transform.position.x;
            float newY = transform.position.y;

            // 6. Movement Logic
            if (canReel && Input.GetKey(KeyCode.Space))
            {
                // Reel up and home toward rod tip X
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
                // Sinking
                newX += h * moveSpeed * Time.deltaTime;
                if (newY > maxDepth) newY -= sinkSpeed * Time.deltaTime;
            }

            newX = Mathf.Clamp(newX, leftBorder, rightBorder);
            newY = Mathf.Clamp(newY, maxDepth, surfaceLevel);
            transform.position = new Vector3(newX, newY, transform.position.z);
        }

        private void SetLineVisible(bool visible)
        {
            if (line != null) line.enabled = visible;
        }

        void ResetHook()
        {
            isReadyToCast = true;
            canReel = false;
            hasCaughtFish = false;
            SetLineVisible(false);

            if (playerTransform != null)
                transform.position = GetRodTipWorldPos();

            if (caughtFishTransform != null)
            {
                Component movement = caughtFishTransform.GetComponent("FishMovement");
                if (movement != null) (movement as MonoBehaviour).enabled = true;
                caughtFishTransform.SetParent(null);
                caughtFishTransform = null;
            }
        }

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