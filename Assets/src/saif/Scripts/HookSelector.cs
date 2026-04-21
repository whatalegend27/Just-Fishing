using UnityEngine; // Standard Unity library

namespace Saif.GamePlay // Organizing this into my gameplay folder
{
    public class HookSelector : MonoBehaviour // Class for switching between hook types
    {
        // SINGLETON PATTERN: Creating a static variable that holds the "one and only" version of this script
        public static HookSelector instance; // This allows any script (like MobileInputBridge) to find this script instantly

        [Header("Hook Prefabs")]
        [SerializeField] private GameObject small_hook_prefab; // Private slot for the Small Hook object
        [SerializeField] private GameObject heavy_hook_prefab; // Private slot for the Heavy Hook object

        [Header("Spawn Points")]
        [SerializeField] private Transform spawn_point; // Where to spawn the hook relative to the player
        [SerializeField] private Transform debug_spawn_point; // A backup spawn point for testing

        private GameObject current_hook;   // Reference to the hook object currently in the game
        private bool is_heavy_hook = false; // Flag to track which hook type is currently active
        private CameraFollow camera_follow; // Reference to the camera script so we can tell it what to follow

        void Awake() // Awake runs BEFORE Start, perfect for setting up Singletons
        {
            if( instance == null ) // If the "instance" slot is empty
            {
                instance = this; // Fill it with this specific script
            }
        }

        void Start() // Runs when the game begins
        {
            camera_follow = Camera.main.GetComponent<CameraFollow>(); // Find the CameraFollow script on the Main Camera
            spawn_hook(small_hook_prefab); // Spawn the default small hook at the start
        }

        void Update() // Runs every frame
        {
            if (Input.GetKeyDown(KeyCode.Z)) // If the user presses the 'Z' key
            {
                toggle_hook(); // Call the function to switch hooks
            }
        }

        public void toggle_hook() // Function to flip-flop between small and heavy
        {
            FishingHook hook_script = (current_hook != null) // Get the current hook's script
                ? current_hook.GetComponent<FishingHook>() 
                : null;

            if (hook_script != null && hook_script.IsHookCast) // Logic Check: If the hook is currently in the water
            {
                return; // Stop here! We don't want to swap hooks while the player is mid-fishing
            }

            if (is_heavy_hook) // If we currently have the heavy hook
            {
                is_heavy_hook = false; // Set flag to small
                spawn_hook(small_hook_prefab); // Swap it out
            }
            else // If we currently have the small hook
            {
                is_heavy_hook = true; // Set flag to heavy
                spawn_hook(heavy_hook_prefab); // Swap it out
            }
        }

        private void spawn_hook(GameObject hook_prefab) // Internal logic for deleting the old hook and making a new one
        {
            if (current_hook != null) // If there is an old hook in the scene
            {
                Destroy(current_hook); // Delete it from the game
            }

            // Decide which spawn point to use based on whether debug is assigned
            Transform spawn_pos = (debug_spawn_point != null) ? debug_spawn_point : spawn_point;
            
            // INSTANTIATE: This is the Unity command to create a new object from a prefab
            current_hook = Instantiate(hook_prefab, spawn_pos.position, Quaternion.identity);

            FishingHook hook_script = current_hook.GetComponent<FishingHook>(); // Get the script on the new hook
            if (hook_script != null) // If the script exists
            {
                hook_script.SetDebugSpawnOverride(spawn_pos.position); // Tell the hook where it was born
            }

            if (camera_follow != null) // If we have a camera reference
            {
                camera_follow.target = current_hook.transform; // Tell the camera to stop following the old hook and follow the new one
            }
        }

        public void select_small_hook() // Specifically switch to small (used by UI buttons)
        {
            FishingHook hook_script = (current_hook != null) 
                ? current_hook.GetComponent<FishingHook>() 
                : null;
                
            if (hook_script != null && hook_script.IsHookCast) // Check if mid-fishing
            {
                return; // Don't allow swap
            }

            is_heavy_hook = false; // Force flag to small
            spawn_hook(small_hook_prefab); // Run spawn
        }

        public void select_heavy_hook() // Specifically switch to heavy (used by UI buttons)
        {
            FishingHook hook_script = (current_hook != null) 
                ? current_hook.GetComponent<FishingHook>() 
                : null;
                
            if (hook_script != null && hook_script.IsHookCast) // Check if mid-fishing
            {
                return; // Don't allow swap
            }

            is_heavy_hook = true; // Force flag to heavy
            spawn_hook(heavy_hook_prefab); // Run spawn
        }
    }
}