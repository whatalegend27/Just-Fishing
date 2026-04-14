using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// This script handles the typewriter effect for basic dialogues in the game.
public class TypewriterEffect : MonoBehaviour, IPointerDownHandler
{
    [Header("Text Settings")]
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private GameObject dialogueBox; 

    [Header("Audio Settings")]
    [SerializeField] private AudioClip typeSound;      
    private AudioSource audioSource; 
    private string fullText;
    private bool isAnimating, isFinished = false;

    // Initialize variables and set up audio source
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

    //Start coroutine to animate text typing and play sound on pointer down. If text is finished, close dialogue box
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

    // Coroutine to animate text typing and play sound
    private IEnumerator TypeText()
    {
        isAnimating = true;
        isFinished = false;
        textComponent.text = "";

        if (typeSound != null)
        {
            audioSource.clip = typeSound;
            audioSource.time = 8f;
            
            // Set the speed of the sound before playing
            audioSource.pitch = 1.0f; 
            
            audioSource.Play();
        }

        for (int i = 0; i < fullText.Length; i++)
        {
            textComponent.text += fullText[i];
            yield return new WaitForSeconds(0.3f);
        }

        audioSource.Stop();

        isAnimating = false;
        isFinished = true;
    }

    // Reset the typewriter when enabled
    private void OnEnable()
    {
        isFinished = false;
        isAnimating = false;
        if (textComponent != null) textComponent.text = ""; 
    }
}