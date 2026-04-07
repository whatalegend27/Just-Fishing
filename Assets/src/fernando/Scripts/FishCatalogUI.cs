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

    public List<FishButtonEntry> fishButtons;

    void Start()
    {
        foreach (FishButtonEntry entry in fishButtons)
        {
            if (entry.button == null) continue;
            entry.button.SetActive(false);
        }

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
}
