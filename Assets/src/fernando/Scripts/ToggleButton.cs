using System.Collections.Generic;
using UnityEngine;

public class ToggleButton : MonoBehaviour
{
    public GameObject catFish;
    public GameObject nemoFish;
    public GameObject orangeFish;
    public GameObject butterflyFish;
    public GameObject silverFish;
    public GameObject skellyFish;
    public GameObject bigbruceFish;

    private List<GameObject> allFish;
    private GameObject currentFish;

    private void Start()
    {
        allFish = new List<GameObject>
        {
            catFish, nemoFish, orangeFish, butterflyFish,
            silverFish, skellyFish, bigbruceFish
        };

        // Hide all fish at start
        foreach (GameObject fish in allFish)
            if (fish != null) fish.SetActive(false);
    }

    private void Show(GameObject fish)
    {
        // If clicking the same fish, hide it (toggle off)
        if (currentFish == fish)
        {
            fish.SetActive(false);
            currentFish = null;
            return;
        }

        // Hide current, show new
        if (currentFish != null)
            currentFish.SetActive(false);

        fish.SetActive(true);
        currentFish = fish;
    }

    public void OnCatButtonClicked() => Show(catFish);
    public void OnNemoButtonClicked() => Show(nemoFish);
    public void OnOrangeButtonClicked() => Show(orangeFish);
    public void OnButterflyButtonClicked() => Show(butterflyFish);
    public void OnSilverButtonClicked() => Show(silverFish);
    public void OnSkellyButtonClicked() => Show(skellyFish);
    public void OnBigBruceButtonClicked() => Show(bigbruceFish);
}