using UnityEngine;

namespace Saif.GamePlay 
{
    public class HookSelector : MonoBehaviour
    {
        public GameObject smallHookPrefab;
        public GameObject heavyHookPrefab;
        public Transform spawnPoint;
        public Transform debugSpawnPoint;

        private GameObject currentHook;
        private bool isHeavyHook = false;
        private CameraFollow cameraFollow;

        void Start()
        {
            cameraFollow = Camera.main.GetComponent<CameraFollow>();
            SpawnHook(smallHookPrefab);
        }

        void Update()
        {
            // Only allow hook swap when hook is not cast
            if (Input.GetKeyDown(KeyCode.Z))
            {
                FishingHook hookScript = currentHook != null ? currentHook.GetComponent<FishingHook>() : null;

                // Block swap if hook is currently cast
                if (hookScript != null && hookScript.IsHookCast) return;

                if (isHeavyHook)
                {
                    isHeavyHook = false;
                    SpawnHook(smallHookPrefab);
                }
                else
                {
                    isHeavyHook = true;
                    SpawnHook(heavyHookPrefab);
                }
            }
        }

        void SpawnHook(GameObject hookPrefab)
        {
            if (currentHook != null) 
                Destroy(currentHook);

            Transform spawnPos = (debugSpawnPoint != null) ? debugSpawnPoint : spawnPoint;
            currentHook = Instantiate(hookPrefab, spawnPos.position, Quaternion.identity);

            FishingHook hookScript = currentHook.GetComponent<FishingHook>();
            if (hookScript != null)
            {
                hookScript.isHeavyHook = isHeavyHook;
                hookScript.debugSpawnOverride = spawnPos.position;
            }

            if (cameraFollow != null)
                cameraFollow.target = currentHook.transform;
        }

        public void SelectSmallHook()
        {
            FishingHook hookScript = currentHook != null ? currentHook.GetComponent<FishingHook>() : null;
            if (hookScript != null && hookScript.IsHookCast) return;

            isHeavyHook = false;
            SpawnHook(smallHookPrefab);
        }

        public void SelectHeavyHook()
        {
            FishingHook hookScript = currentHook != null ? currentHook.GetComponent<FishingHook>() : null;
            if (hookScript != null && hookScript.IsHookCast) return;

            isHeavyHook = true;
            SpawnHook(heavyHookPrefab);
        }
    }
}