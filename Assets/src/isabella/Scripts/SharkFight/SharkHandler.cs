using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// This script should be attached to the Shark GameObject in the scene. It handles the player's interactions with the shark, such as flirting, insulting, or fighting. Each method corresponds to a button click in the UI, and it updates the Animator parameters and dialogue accordingly.
public class SharkHandler : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Animator animator;

    [Header("Dialogue Settings")]
    [SerializeField] private GameObject ChoiceDialogue;
    [SerializeField] private GameObject FlirtDialogue;
    [SerializeField] private GameObject InsultDialogue;
    [SerializeField] private GameObject FightDialogue;

    [SerializeField] private string[] flirtTexts;
    [SerializeField] private string[] insultTexts;
    [SerializeField] private string[] fightTexts;

    [SerializeField] private FightTypewriter flirtTypewriter;
    [SerializeField] private FightTypewriter insultTypewriter;
    [SerializeField] private FightTypewriter fightTypewriter;


    private int choiceCount, flirtCount, insultCount, fightCount;
    
    private void Start()
    {
        choiceCount = 0;
        flirtCount = 0;
        insultCount = 0;
        fightCount = 0;
    }

    private void Update()
    {
        choiceCount = flirtCount + insultCount + fightCount;
        if (choiceCount > 3)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    //  On FlirtClicked, we set the "IsBlushing" parameter to true in the Animator, which will trigger the blushing animation. We also hide the choice dialogue and show the flirt dialogue.
    public void OnFlirtClicked()
    {
        flirtCount++;

        ChoiceDialogue.SetActive(false);
        FlirtDialogue.SetActive(true);
        Debug.Log("The new text should be: " + flirtTexts[flirtCount - 1]);

        flirtTypewriter.UpdateText(flirtTexts[flirtCount - 1]);

        animator.SetBool("IsBlushing", true);
    }

    // On InsultClicked, we set the "IsInsulted" parameter to true in the Animator, which will trigger the insult animation. We also hide the choice dialogue and show the insult dialogue.
    public void OnInsultClicked()
    {
        insultCount++;

        ChoiceDialogue.SetActive(false);
        InsultDialogue.SetActive(true);

        insultTypewriter.UpdateText(insultTexts[insultCount - 1]);


        animator.SetBool("IsInsulted", true);
    }

    // On FightClicked, we hide the choice dialogue and show the fight dialogue. You can expand this method to include more complex logic for handling the fight outcome, such as randomizing the result or updating player stats.
    public void OnFightClicked()
    {
        fightCount++;

        ChoiceDialogue.SetActive(false);
        FightDialogue.SetActive(true);

        fightTypewriter.UpdateText(fightTexts[fightCount - 1]);

    }
}