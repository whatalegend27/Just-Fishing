using UnityEngine;

public class HandleSharkChoices : MonoBehaviour
{
    [SerializeField] private GameObject FightBtn;
    [SerializeField] private GameObject FlirtBtn;
    [SerializeField] private GameObject InsultBtn;

    [SerializeField] private GameObject ChoiceDialogue;
    [SerializeField] private GameObject FlirtDialogue;
    [SerializeField] private GameObject InsultDialogue;
    [SerializeField] private GameObject FightDialogue;

    void Start()
    {
        FightBtn.SetActive(false);
        FlirtBtn.SetActive(false);
        InsultBtn.SetActive(false);
        ChoiceDialogue.SetActive(true);
        FlirtDialogue.SetActive(false);
        InsultDialogue.SetActive(false);
        FightDialogue.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
