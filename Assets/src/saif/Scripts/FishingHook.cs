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

        private bool hasCaughtFish = false;
        private bool isReadyToCast = true;
        private bool isReeling = false;
        private Transform caughtFishTransform;

        void Update()
        {
            // 1. Draw the Line
            if (line != null && rodTip != null)
            {
                line.SetPosition(0, rodTip.position);
                line.SetPosition(1, transform.position);
            }

            // 2. Cast OR Reel using Space (if/else if to keep states separate)
            if (isReadyToCast && Input.GetKeyDown(KeyCode.Space))
            {
                // CAST: launch the hook into the water
                isReadyToCast = false;
                isReeling = false;
                return;
            }
            else if (!isReadyToCast && !isReeling && Input.GetKeyDown(KeyCode.Space))
            {
                // REEL: player pressed Space while hook is in water
                isReeling = true;
            }

            // Still waiting to cast — nothing else to do
            if (isReadyToCast) return;

            // 3. Inputs
            float h = Input.GetAxis("Horizontal");
            float newX = transform.position.x;
            float newY = transform.position.y;

            // 4. Movement Logic
            if (isReeling)
            {
                newY += reelSpeed * Time.deltaTime;

                float distanceToSurface = surfaceLevel - newY;

                // If close to the surface, handle the diagonal "Homing"
                if (distanceToSurface < 3f)
                {
                    newX = Mathf.MoveTowards(newX, 0, (moveSpeed * 0.5f) * Time.deltaTime);
                    newX += h * (moveSpeed * 0.5f) * Time.deltaTime;
                }
                else
                {
                    newX += h * moveSpeed * Time.deltaTime;
                }

                // Reset when home
                if (newY >= surfaceLevel)
                {
                    newY = surfaceLevel;
                    newX = 0;
                    isReadyToCast = true;
                    isReeling = false;

                    if (hasCaughtFish)
                    {
                        CollectFish();
                    }
                }
            }
            else
            {
                // Hook is sinking — player can move it horizontally
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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!hasCaughtFish && collision.CompareTag("Fish"))
            {
                hasCaughtFish = true;
                caughtFishTransform = collision.transform;

                FishMovement movement = collision.GetComponent<FishMovement>();
                if (movement != null)
                {
                    movement.enabled = false;
                }

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
            isReeling = false;
        }
    }
}