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
            if (!debugMode) FindPlayerReference();
        }

        private void FindPlayerReference()
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null) playerAnimator = playerObj.GetComponent<Animator>();
            
            if (playerAnimator == null)
            {
                // Fallback scan for any animator with the right parameter
                Animator[] anims = Object.FindObjectsByType<Animator>(FindObjectsSortMode.None);
                foreach (Animator a in anims)
                {
                    foreach (var param in a.parameters)
                    {
                        if (param.name == "IsCasting") { playerAnimator = a; return; }
                    }
                }
            }
        }

        void Update()
        {
            // 1. Draw Line (Only if we have a rodTip assigned)
            if (line != null && rodTip != null)
            {
                line.SetPosition(0, rodTip.position);
                line.SetPosition(1, transform.position);
            }

            // 2. The Toggle Logic
            bool isCasting = debugMode; // If debugging, we are always "casting"
            if (!debugMode && playerAnimator != null)
            {
                isCasting = playerAnimator.GetBool("IsCasting");
            }

            if (!isCasting)
            {
                if (!isReadyToCast) ResetHook();
                return;
            }

            // 3. Cast (Press Space to start sinking)
            if (isReadyToCast && Input.GetKeyDown(KeyCode.Space))
            {
                isReadyToCast = false;
                canReel = false;
                return;
            }

            if (isReadyToCast) return;

            // 4. Movement & Physics
            if (!canReel && Input.GetKeyUp(KeyCode.Space)) canReel = true;

            float h = Input.GetAxis("Horizontal");
            float newX = transform.position.x;
            float newY = transform.position.y;

            if (canReel && Input.GetKey(KeyCode.Space))
            {
                newY += reelSpeed * Time.deltaTime;
                if (surfaceLevel - newY < 3f)
                {
                    newX = Mathf.MoveTowards(newX, 0, (moveSpeed * 0.5f) * Time.deltaTime);
                    newX += h * (moveSpeed * 0.5f) * Time.deltaTime;
                }
                else newX += h * moveSpeed * Time.deltaTime;

                if (newY >= surfaceLevel)
                {
                    if (hasCaughtFish) CollectFish();
                    ResetHook();
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

        void ResetHook()
        {
            // In debug mode, don't fly back to a missing rod tip
            if (rodTip != null) transform.position = rodTip.position;
            else if (debugMode) transform.position = new Vector3(0, surfaceLevel, 0);

            hasCaughtFish = false;
            isReadyToCast = true;
            canReel = false;

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