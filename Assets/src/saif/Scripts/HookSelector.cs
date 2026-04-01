using UnityEngine;

namespace Saif.GamePlay 
{
    public class HookSelector : MonoBehaviour
    {
        public GameObject smallHookPrefab;
        public GameObject heavyHookPrefab;
        public Transform spawnPoint;
        
        private GameObject currentHook;

        public void SelectSmallHook()
        {
            SpawnHook(smallHookPrefab);
        }

        public void SelectHeavyHook()
        {
            SpawnHook(heavyHookPrefab);
        }

        void SpawnHook(GameObject hookPrefab)
        {
            // 1. Remove the old hook if there is one
            if (currentHook != null) 
                Destroy(currentHook);

            // 2. Create the new hook at the spawn point
            currentHook = Instantiate(hookPrefab, spawnPoint.position, Quaternion.identity);

            // 3. No need to set rodTip anymore — FishingHook finds the player automatically
        }
    }
}