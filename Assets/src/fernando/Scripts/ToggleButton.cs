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

    private void OnEnable()
    {
        FishDatabaseManager.OnFishRegistered += UpdateCatchCount;
    }

    private void OnDisable()
    {
        FishDatabaseManager.OnFishRegistered -= UpdateCatchCount;
    }

    private void Start()
    {
        foreach (FishDisplayEntry entry in fishDisplays)
            if (entry.fishDisplay != null) entry.fishDisplay.SetActive(false);
    }

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
