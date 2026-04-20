using UnityEngine;

// This script manages the visibility of fight, flirt, and insult buttons, as well as the corresponding dialogues based on player choices in the shark encounter.
public class HandleSharkChoices : MonoBehaviour
{
    [SerializeField] private GameObject fightBtn;
    [SerializeField] private GameObject flirtBtn;
    [SerializeField] private GameObject insultBtn;

    [SerializeField] private GameObject choiceDialogue;
    [SerializeField] private GameObject flirtDialogue;
    [SerializeField] private GameObject insultDialogue;
    [SerializeField] private GameObject fightDialogue;

    void Start() // Initialize by hiding all buttons and dialogues
    {
        fightBtn.SetActive(false);
        flirtBtn.SetActive(false);
        insultBtn.SetActive(false);
        choiceDialogue.SetActive(true);
        flirtDialogue.SetActive(false);
        insultDialogue.SetActive(false);
        fightDialogue.SetActive(false);
    }

}
