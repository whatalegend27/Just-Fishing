using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewsToggle : MonoBehaviour, IPointerDownHandler
{
    public GameObject NewsToMove;
    public GameObject darkOverlay; 

    public GameObject dialogueBox;

    private SpriteRenderer spriteRenderer;

    private bool isAnimating = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isAnimating)
        {
            StartCoroutine(AnimateAndHide());
        }
    }

    IEnumerator AnimateAndHide()
    {
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

        // After animation finishes, turn it off
        NewsToMove.SetActive(false);
        if (NewsToMove.tag == "Job")
        {
            darkOverlay.SetActive(false);
            dialogueBox.SetActive(true);
        }
    }

}
