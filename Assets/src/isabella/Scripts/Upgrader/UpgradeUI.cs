using UnityEngine;
using UnityEngine.SceneManagement;

// Manages the overall upgrade UI, refreshing it when the upgrader scene is loaded or when upgrades change.
public class UpgradeUI : MonoBehaviour
{
    // Refresh the UI when the upgrader is loaded.
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        RefreshUI();
    }


    // If disabled, stop listening for scene loads to prevent memory leaks.
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // When a new scene is loaded, check if it's the upgrader and refresh the UI accordingly.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshUI();
    }

    // Refresh UI based on the RodUpgradeManager.
    public void RefreshUI()
    {
        var mgr = RodUpgradeManager.Instance;
        if (mgr == null) return;

        // Example logs (replace with UI display logic)
        Debug.Log("Equipped Lure: " + mgr.equippedLure);
        Debug.Log("Equipped Bait: " + mgr.equippedBait);
        Debug.Log("Equipped Weight: " + mgr.equippedWeight);
    }
}