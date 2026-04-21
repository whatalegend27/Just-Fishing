using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeUI : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        RefreshUI();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshUI();
    }

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