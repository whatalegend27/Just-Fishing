using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class FightTypewriter : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TMP_Text textComponent;
    private float typingSpeed = 0.05f;
    [SerializeField] private GameObject dialogueBox; 

    [Header("Audio")]
    [SerializeField] private AudioClip typeSound; 
    private float startTimeInClip = 8f; 
    [Range(0.1f, 3f)] 
    private float audioPitch = 1.0f; // New public variable to control speed
    
    private AudioSource audioSource; 
    private string fullText;
    private bool isAnimating = false;
    private bool isFinished = false;

    [SerializeField] private GameObject FightBtn;
    [SerializeField] private GameObject FlirtBtn;
    [SerializeField] private GameObject InsultBtn;
    [SerializeField] private GameObject ChoiceDialogue;
    [SerializeField] private Animator animator;


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
        if (isFinished && textComponent.tag == "FightTxt")
        {
            FightBtn.SetActive(true);
            FlirtBtn.SetActive(true);
            InsultBtn.SetActive(true);
            return; 
        }
        else if (isFinished)
        {
            dialogueBox.SetActive(false);
            ChoiceDialogue.SetActive(true);
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
            FightBtn.SetActive(false);
            FlirtBtn.SetActive(false);
            InsultBtn.SetActive(false);
        }

        if (!isAnimating && !isFinished)
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