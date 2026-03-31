using UnityEngine;

namespace Saif.GamePlay
{
    public class FishingHook : MonoBehaviour
    {
        [Header("Speeds")]
        public float sinkSpeed = 2f;
        public float moveSpeed = 5f;
        public float reelSpeed = 3.5f;

        [Header("Borders & Limits")]
        public float maxDepth = -10f;
        public float surfaceLevel = 0f;
        public float leftBorder = -8f;
        public float rightBorder = 8f;

        [Header("References")]
        public LineRenderer line;
        public Transform rodTip;

        private Animator playerAnimator;
        private bool hasCaughtFish = false;
        private bool isReadyToCast = true;
        private bool canReel = false;
        private Transform caughtFishTransform;

        void Start()
        {
            // Find the player in the scene automatically using the Player tag
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerAnimator = player.GetComponent<Animator>();
            }

            if (playerAnimator == null)
            {
                Debug.LogError("FishingHook: Could not find player Animator! Make sure the player is tagged as Player");
            }
        }

        void Update()
        {
            // 1. Draw the Line
            if (line != null && rodTip != null)
            {
                line.SetPosition(0, rodTip.position);
                line.SetPosition(1, transform.position);
            }

            // 2. Check if fishing animation is active
            bool isCasting = playerAnimator != null && playerAnimator.GetBool("IsCasting");
            if (!isCasting)
            {
                if (!isReadyToCast)
                {
                    ResetHook();
                }
                return;
            }

            // 3. Cast
            if (isReadyToCast && Input.GetKeyDown(KeyCode.Space))
            {
                isReadyToCast = false;
                canReel = false;
                return;
            }

            if (isReadyToCast) return;

            // 4. Unlock reeling only after Space is fully released after casting
            if (!canReel && Input.GetKeyUp(KeyCode.Space))
            {
                canReel = true;
            }

            // 5. Inputs
            float h = Input.GetAxis("Horizontal");
            float newX = transform.position.x;
            float newY = transform.position.y;

            // 6. Movement Logic
            if (canReel && Input.GetKey(KeyCode.Space))
            {
                // HOLD Space to reel up
                newY += reelSpeed * Time.deltaTime;

                float distanceToSurface = surfaceLevel - newY;

                if (distanceToSurface < 3f)
                {
                    newX = Mathf.MoveTowards(newX, 0, (moveSpeed * 0.5f) * Time.deltaTime);
                    newX += h * (moveSpeed * 0.5f) * Time.deltaTime;
                }
                else
                {
                    newX += h * moveSpeed * Time.deltaTime;
                }

                if (newY >= surfaceLevel)
                {
                    newY = surfaceLevel;
                    newX = 0;
                    isReadyToCast = true;
                    canReel = false;

                    if (hasCaughtFish)
                    {
                        CollectFish();
                    }
                }
            }
            else
            {
                // Sinking — player can move horizontally
                newX += h * moveSpeed * Time.deltaTime;

                if (newY > maxDepth)
                {
                    newY -= sinkSpeed * Time.deltaTime;
                }
            }

            // Final Position Clamp
            newX = Mathf.Clamp(newX, leftBorder, rightBorder);
            newY = Mathf.Clamp(newY, maxDepth, surfaceLevel);
            transform.position = new Vector3(newX, newY, transform.position.z);
        }

        void ResetHook()
        {
            if (rodTip != null)
                transform.position = new Vector3(0, surfaceLevel, transform.position.z);

            hasCaughtFish = false;
            isReadyToCast = true;
            canReel = false;

            if (caughtFishTransform != null)
            {
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

                /*FishMovement movement = collision.GetComponent<FishMovement>();
                if (movement != null)
                {
                    movement.enabled = false;
                }*/

                caughtFishTransform.SetParent(this.transform);
                caughtFishTransform.localPosition = Vector2.zero;

                Debug.Log("Got one! Reeling in...");
            }
        }

        void CollectFish()
        {
            if (caughtFishTransform != null)
            {
                Destroy(caughtFishTransform.gameObject);
            }

            hasCaughtFish = false;
            caughtFishTransform = null;
            isReadyToCast = true;
            canReel = false;
        }
    }
}