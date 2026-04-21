using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToggleButton : MonoBehaviour
{
    [System.Serializable]
    public struct FishDisplayEntry
    {
        public string fishName;
        public GameObject fishDisplay;
        public TextMeshProUGUI catchCountText;
    }

    [SerializeField] private List<FishDisplayEntry> fishDisplays;

    private GameObject currentFish;

    // Subscribes to the fish registered event
    private void OnEnable()
    {
        FishDatabaseManager.OnFishRegistered += UpdateCatchCount;
    }

    // Unsubscribes from the fish registered event
    private void OnDisable()
    {
        FishDatabaseManager.OnFishRegistered -= UpdateCatchCount;
    }

    // Hides all fish displays on startup
    private void Start()
    {
        foreach (FishDisplayEntry entry in fishDisplays)
            if (entry.fishDisplay != null) entry.fishDisplay.SetActive(false);
    }

    // Refreshes the catch count label if the currently shown fish matches the registered one
    private void UpdateCatchCount(string fishName)
    {
        if (currentFish == null) return;

        foreach (FishDisplayEntry entry in fishDisplays)
        {
            if (entry.fishName != fishName) continue;
            if (currentFish != entry.fishDisplay) return;

            if (entry.catchCountText != null)
            {
                FishData data = FishDatabaseManager.Instance.fishDatabase.Find(f => f.fishName == fishName);
                if (data != null)
                    entry.catchCountText.text = "Times Caught: " + data.catchCount.ToString();
            }
            return;
        }
    }

    // Toggles the display panel for the given fish, closing any previously open one
    public void OnFishButtonClicked(string fishName)
    {
        foreach (FishDisplayEntry entry in fishDisplays)
        {
            if (entry.fishName != fishName) continue;

            if (currentFish == entry.fishDisplay)
            {
                entry.fishDisplay.SetActive(false);
                currentFish = null;
                return;
            }

            if (currentFish != null) currentFish.SetActive(false);
            entry.fishDisplay.SetActive(true);
            currentFish = entry.fishDisplay;

            if (entry.catchCountText != null)
            {
                FishData data = FishDatabaseManager.Instance.fishDatabase.Find(f => f.fishName == fishName);
                if (data != null)
                    entry.catchCountText.text = "Times Caught: " + data.catchCount.ToString();
            }

            return;
        }
    }
}
