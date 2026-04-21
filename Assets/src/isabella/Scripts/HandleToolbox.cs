using UnityEngine;

public class HandleToolbox : MonoBehaviour
{
    [Header("Toolbox Settings")]
    public GameObject[] toolboxes;
    public GameObject defaultToolbox; // 👈 ADD THIS

    private IToolboxState currentState;

    private readonly IToolboxState gameplayState = new GameplayState();

    void Start()
    {
        toolboxes = GameObject.FindGameObjectsWithTag("Toolbox");

        foreach (GameObject toolbox in toolboxes)
            toolbox.SetActive(false);

        SetState(gameplayState);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (currentState is GameplayState)
                SetState(new ToolboxState(defaultToolbox)); // 👈 DEFAULT OPEN
            else
                SetState(gameplayState);
        }
    }

    public void SetGameplayState() => SetState(gameplayState);

    public void OpenToolbox(GameObject toolbox)
    {
        SetState(new ToolboxState(toolbox));
    }

    public void SetState(IToolboxState newState)
    {
        if (currentState == newState) return;

        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

    public IToolboxState GetCurrentState() => currentState;
}