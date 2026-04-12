using System;
using UnityEngine;

public class SharkHandler : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject ChoiceDialogue;
    [SerializeField] private GameObject FlirtDialogue;
    [SerializeField] private GameObject InsultDialogue;
    [SerializeField] private GameObject FightDialogue;

    // Change this to a public void so the Button can see it
    public void OnFlirtClicked()
    {
        if (animator != null)
        {
        animator.SetBool("IsBlushing", true);
        }
        ChoiceDialogue.SetActive(false);
        FlirtDialogue.SetActive(true);
    }

    public void OnInsultClicked()
    {
        if (animator != null)
        {
            animator.SetBool("IsInsulted", true);
        }
        ChoiceDialogue.SetActive(false);
        InsultDialogue.SetActive(true);
    }

    public void OnFightClicked()
    {
        ChoiceDialogue.SetActive(false);
        FightDialogue.SetActive(true);
        // You can add more logic here for the fight outcome
    }
}