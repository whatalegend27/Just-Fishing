using UnityEngine;

namespace Saif.GamePlay 
{
    public class HookSelector : MonoBehaviour
    {
        public GameObject smallHookPrefab;
        public GameObject heavyHookPrefab;
        public Transform spawnPoint; // This is where the hook starts (the boat/rod tip)
        
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
            // Remove the old hook if there is one
            if (currentHook != null) 
                Destroy(currentHook);

            // Create the new hook at the spawn point
            currentHook = Instantiate(hookPrefab, spawnPoint.position, Quaternion.identity);
            
            // Tell the hook where the rod tip is for the line
            currentHook.GetComponent<FishingHook>().rodTip = spawnPoint;
        }
    }
}