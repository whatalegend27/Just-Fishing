using System.Collections.Generic;
using UnityEngine;

public class ToggleButton : MonoBehaviour
{
    [System.Serializable]
    public struct FishDisplayEntry
    {
        public string fishName;
        public GameObject fishDisplay;
    }

    [SerializeField] private List<FishDisplayEntry> fishDisplays;

    private GameObject currentFish;

    private void Start()
    {
        foreach (FishDisplayEntry entry in fishDisplays)
            if (entry.fishDisplay != null) entry.fishDisplay.SetActive(false);
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
            return;
        }
    }
}
