using UnityEngine;

public class HandleDialogue : MonoBehaviour
{
    public GameObject DialogueBox;
    public GameObject QuestionBox;
    void Start()
    {

    Debug.Log("NewsSeen value: " + PlayerPrefs.GetInt("NewsSeen", 0));

        if (PlayerPrefs.GetInt("NewsSeen", 0) == 1)
        {
            QuestionBox.SetActive(true);
            DialogueBox.SetActive(false);
            return;
        }
        else
        {
            QuestionBox.SetActive(false);
            DialogueBox.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
