using UnityEngine;

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

    //  On FlirtClicked, we set the "IsBlushing" parameter to true in the Animator, which will trigger the blushing animation. We also hide the choice dialogue and show the flirt dialogue.
    public void OnFlirtClicked()
    {
        if (animator != null)
        {
        animator.SetBool("IsBlushing", true);
        }
        ChoiceDialogue.SetActive(false);
        FlirtDialogue.SetActive(true);
    }

    // On InsultClicked, we set the "IsInsulted" parameter to true in the Animator, which will trigger the insult animation. We also hide the choice dialogue and show the insult dialogue.
    public void OnInsultClicked()
    {
        if (animator != null)
        {
            animator.SetBool("IsInsulted", true);
        }
        ChoiceDialogue.SetActive(false);
        InsultDialogue.SetActive(true);
    }

    // On FightClicked, we hide the choice dialogue and show the fight dialogue. You can expand this method to include more complex logic for handling the fight outcome, such as randomizing the result or updating player stats.
    public void OnFightClicked()
    {
        ChoiceDialogue.SetActive(false);
        FightDialogue.SetActive(true);
        // You can add more logic here for the fight outcome
    }
}