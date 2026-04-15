using UnityEngine;

public class HandleToolbox : MonoBehaviour
{
    [Header("Toolbox Settings")]
    public GameObject tbShow;
    public GameObject[] toolboxes;

    private IToolboxState currentState;

    // Reuse instances (important for consistency)
    private IToolboxState gameplayState = new GameplayState();
    private IToolboxState toolboxState = new ToolboxState();

    void Start()
    {
        toolboxes = GameObject.FindGameObjectsWithTag("Toolbox");
        SetState(gameplayState);
        foreach (GameObject toolbox in toolboxes){
            toolbox.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (currentState is GameplayState)
                SetState(toolboxState);
            else
                SetState(gameplayState);
        }
    }

    public void SetState(IToolboxState newState)
    {
        if (currentState == newState) return;

        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

    // 👇 REQUIRED for testing
    public IToolboxState GetCurrentState()
    {
        return currentState;
    }

    // 👇 Optional helpers (nice for tests / clarity)
    public void SetGameplayState() => SetState(gameplayState);
    public void SetToolboxState() => SetState(toolboxState);
}