using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TypewriterEffect : MonoBehaviour, IPointerDownHandler
{
    public TMP_Text textComponent;
    public float typingSpeed = 0.05f;
    public GameObject dialogueBox; 

    [Header("Audio")]
    public AudioClip typeSound; 
    public float startTimeInClip = 8f; 
    [Range(0.1f, 3f)] 
    public float audioPitch = 1.0f; // New public variable to control speed
    
    private AudioSource audioSource; 
    private string fullText;
    private bool isAnimating = false;
    private bool isFinished = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.playOnAwake = false;

        if (textComponent != null)
        {
            fullText = textComponent.text;
            textComponent.text = ""; 
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isFinished)
        {
            dialogueBox.SetActive(false);
            return; 
        }

        if (!isAnimating)
        {
            StartCoroutine(TypeText());
        }
    }

    private IEnumerator TypeText()
    {
        isAnimating = true;
        isFinished = false;
        textComponent.text = "";

        if (typeSound != null)
        {
            audioSource.clip = typeSound;
            audioSource.time = startTimeInClip;
            
            // Set the speed of the sound before playing
            audioSource.pitch = audioPitch; 
            
            audioSource.Play();
        }

        for (int i = 0; i < fullText.Length; i++)
        {
            textComponent.text += fullText[i];
            yield return new WaitForSeconds(typingSpeed);
        }

        audioSource.Stop();

        isAnimating = false;
        isFinished = true;
    }

    private void OnEnable()
    {
        isFinished = false;
        isAnimating = false;
        if (textComponent != null) textComponent.text = ""; 
    }
}