using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// This script handles the typewriter effect for fight, flirt, and insult dialogues in the shark encounter, as well as managing button visibility and animations based on player interactions.
public class FightTypewriter : MonoBehaviour, IPointerDownHandler
{
    [Header("Text Settings")]
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private GameObject dialogueBox; 
    [SerializeField] private GameObject choiceDialogue;


    [Header("Audio")]
    [SerializeField] private AudioClip typeSound; 

    [Header("Fight Buttons")]
    [SerializeField] private GameObject fightBtn;
    [SerializeField] private GameObject flirtBtn;
    [SerializeField] private GameObject insultBtn;

    [Header("Animation Settings")]
    [SerializeField] private Animator animator;

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

    // Handle pointer down events
    public void OnPointerDown(PointerEventData eventData) 
    {
        if (isFinished && textComponent.tag == "FightTxt")
        {
            fightBtn.SetActive(true);
            flirtBtn.SetActive(true);
            insultBtn.SetActive(true);
            return; 
        }
        else if (isFinished)
        {
            dialogueBox.SetActive(false);
            choiceDialogue.SetActive(true);
            animator.SetBool("ReturnTxt", true);
            if (textComponent.tag == "FlirtTxt")
            {
                animator.SetBool("IsBlushing", false);
            }
            else if (textComponent.tag == "InsultTxt")
            {
                animator.SetBool("IsInsulted", false);
            }
            animator.SetBool("ReturnTxt", true);
            fightBtn.SetActive(false);
            flirtBtn.SetActive(false);
            insultBtn.SetActive(false);
        }

        if (!isAnimating && !isFinished)
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