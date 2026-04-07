using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewsToggle : MonoBehaviour, IPointerDownHandler
{
    public GameObject NewsToMove;
    public GameObject darkOverlay; 
    public GameObject dialogueBox;
    public GameObject QuestionBox;

    [Header("Show Once Setting")]
    public string uniqueKey = "NewsSeen"; // SAME key for both sprites

    private bool isAnimating = false;

    void Start()
    {
        // 🚨 If already seen → hide immediately
        if (PlayerPrefs.GetInt(uniqueKey, 0) == 1)
        {
            NewsToMove.SetActive(false);
            // Optional: also hide overlay if needed
            if (darkOverlay != null)
                darkOverlay.SetActive(false);

            return;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isAnimating)
        {
            QuestionBox.SetActive(false); // Hide question box if it's visible
            StartCoroutine(AnimateAndHide());
        }
    }

    IEnumerator AnimateAndHide()
    {
        isAnimating = true;

        Vector3 startScale = NewsToMove.transform.localScale;
        Vector3 targetScale = new Vector3(2, 2, 2);

        Vector3 startPos = NewsToMove.transform.position;
        Vector3 targetPos = new Vector3(20, 20, 0);

        float duration = 2f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            NewsToMove.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            NewsToMove.transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        // Hide after animation
        NewsToMove.SetActive(false);

        // 🚨 Mark as seen FOREVER
        PlayerPrefs.SetInt(uniqueKey, 1);
        PlayerPrefs.Save();

        if (NewsToMove.CompareTag("Job"))
        {
            if (darkOverlay != null)
                darkOverlay.SetActive(false);

            if (dialogueBox != null)
                dialogueBox.SetActive(true);
        }

        isAnimating = false;
    }
}