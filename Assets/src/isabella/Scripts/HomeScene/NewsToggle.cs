using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

// Handles the news item interaction, including animation and showing related UI elements
public class NewsToggle : MonoBehaviour, IPointerDownHandler
{
    [Header("News Settings")]
    [SerializeField] private GameObject newsToMove;
    [SerializeField] private GameObject darkOverlay; 
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject questionBox;

    [Header("Show Once Setting")]
    [SerializeField] private string uniqueKey = "NewsSeen"; // SAME key for both sprites

    private bool isAnimating = false;

    // Check if the news has already been seen and hide it immediately if so
    void Start()
    {
        // If already seen, then hide immediately.
        if (PlayerPrefs.GetInt(uniqueKey, 0) == 1)
        {
            newsToMove.SetActive(false);
            if (darkOverlay != null)
                darkOverlay.SetActive(false);

            return;
        }
    }

    // Handle click/tap on the news item
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isAnimating)
        {
            questionBox.SetActive(false); // Hide question box if it's visible
            StartCoroutine(AnimateAndHide());
        }
    }

    // Coroutine to animate the news item and then hide it
    IEnumerator AnimateAndHide()
    {
        isAnimating = true;

        Vector3 startScale = newsToMove.transform.localScale;
        Vector3 targetScale = new Vector3(2, 2, 2);

        Vector3 startPos = newsToMove.transform.position;
        Vector3 targetPos = new Vector3(20, 20, 0);

        float duration = 2f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            newsToMove.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            newsToMove.transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        // Hide after animation
        newsToMove.SetActive(false);

        // Mark as seen FOREVER
        PlayerPrefs.SetInt(uniqueKey, 1);
        PlayerPrefs.Save();

        if (newsToMove.CompareTag("Job"))
        {
            if (darkOverlay != null)
                darkOverlay.SetActive(false);

            if (dialogueBox != null)
                dialogueBox.SetActive(true);
        }

        isAnimating = false;
    }
}