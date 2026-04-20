using UnityEngine;

// This script manages the dialogue flow based on player preferences, specifically checking if the news has been seen and toggling the visibility of dialogue and question boxes accordingly.
public class HandleDialogue : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject questionBox;

    void Start() // Start is called before the first frame update
    {

        if (PlayerPrefs.GetInt("NewsSeen", 0) == 1) // Check if the news has been seen
        {
            questionBox.SetActive(true);
            dialogueBox.SetActive(false);
            return;
        }
        else
        {
            questionBox.SetActive(false);
            dialogueBox.SetActive(false);
        }
    }
}
