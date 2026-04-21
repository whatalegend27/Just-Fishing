using UnityEngine;
using System.Collections.Generic;

public class FishCatalogUI : MonoBehaviour
{
    [System.Serializable]
    public struct FishButtonEntry
    {
        public string fishName;
        public GameObject button;
    }

    [SerializeField] private List<FishButtonEntry> fishButtons;

    // Subscribes to the fish registered event
    private void OnEnable()
    {
        FishDatabaseManager.OnFishRegistered += ShowButton;
    }

    // Unsubscribes from the fish registered event
    private void OnDisable()
    {
        FishDatabaseManager.OnFishRegistered -= ShowButton;
    }

    // Hides all buttons then re-shows any fish already known from a previous session
    void Start()
    {
        foreach (FishButtonEntry entry in fishButtons)
        {
            if (entry.button == null) continue;
            entry.button.SetActive(false);
        }

        if (FishDatabaseManager.Instance == null) return;

        foreach (FishData fish in FishDatabaseManager.Instance.fishDatabase)
        {
            if (!fish.fishKnown) continue;

            foreach (FishButtonEntry entry in fishButtons)
            {
                if (entry.fishName == fish.fishName && entry.button != null)
                {
                    entry.button.SetActive(true);
                    break;
                }
            }
        }
    }

    // Makes the catalog button visible for the newly registered fish
    private void ShowButton(string fishName)
    {
        foreach (FishButtonEntry entry in fishButtons)
        {
            if (entry.fishName == fishName && entry.button != null)
            {
                entry.button.SetActive(true);
                break;
            }
        }
    }
}
