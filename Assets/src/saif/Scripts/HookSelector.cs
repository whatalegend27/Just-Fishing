/********************************
hook_selector.cs
Saif Badwan
Handles hook swapping and spawning. Implements Singleton pattern.
********************************/
using UnityEngine;

namespace Saif.GamePlay 
{
    public class HookSelector : MonoBehaviour
    {
        // PATTERN: Singleton for easy access from UI
        public static HookSelector instance;

        [Header("Hook Prefabs")]
        [SerializeField] private GameObject small_hook_prefab;
        [SerializeField] private GameObject heavy_hook_prefab;

        [Header("Spawn Points")]
        [SerializeField] private Transform spawn_point;
        [SerializeField] private Transform debug_spawn_point;

        private GameObject current_hook;   
        private bool is_heavy_hook = false; 
        private CameraFollow camera_follow; 

        void Awake()
        {
            if( instance == null )
            {
                instance = this;
            }
        }

        void Start()
        {
            camera_follow = Camera.main.GetComponent<CameraFollow>();
            spawn_hook(small_hook_prefab);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                toggle_hook();
            }
        }

        public void toggle_hook()
        {
            FishingHook hook_script = (current_hook != null) 
                ? current_hook.GetComponent<FishingHook>() 
                : null;

            if (hook_script != null && hook_script.IsHookCast) 
            {
                return;
            }

            if (is_heavy_hook)
            {
                is_heavy_hook = false;
                spawn_hook(small_hook_prefab);
            }
            else
            {
                is_heavy_hook = true;
                spawn_hook(heavy_hook_prefab);
            }
        }

        private void spawn_hook(GameObject hook_prefab)
        {
            if (current_hook != null)
            {
                Destroy(current_hook);
            }

            Transform spawn_pos = (debug_spawn_point != null) ? debug_spawn_point : spawn_point;
            current_hook = Instantiate(hook_prefab, spawn_pos.position, Quaternion.identity);

            FishingHook hook_script = current_hook.GetComponent<FishingHook>();
            if (hook_script != null)
            {
                // Fixed: Removed SetHookType call to stop the CS1061 error
                hook_script.SetDebugSpawnOverride(spawn_pos.position);
            }

            if (camera_follow != null)
            {
                camera_follow.target = current_hook.transform;
            }
        }

        public void select_small_hook()
        {
            FishingHook hook_script = (current_hook != null) 
                ? current_hook.GetComponent<FishingHook>() 
                : null;
                
            if (hook_script != null && hook_script.IsHookCast) 
            {
                return;
            }

            is_heavy_hook = false;
            spawn_hook(small_hook_prefab);
        }

        public void select_heavy_hook()
        {
            FishingHook hook_script = (current_hook != null) 
                ? current_hook.GetComponent<FishingHook>() 
                : null;
                
            if (hook_script != null && hook_script.IsHookCast) 
            {
                return;
            }

            is_heavy_hook = true;
            spawn_hook(heavy_hook_prefab);
        }
    }
}