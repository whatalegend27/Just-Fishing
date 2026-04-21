using UnityEngine;

public class ToolboxState : IToolboxState
{
    private GameObject activeToolbox;

    public ToolboxState(GameObject toolbox)
    {
        activeToolbox = toolbox;
    }

    public void Enter(HandleToolbox context)
    {
        foreach (GameObject tb in context.toolboxes)
            tb.SetActive(false);

        if (activeToolbox != null)
            activeToolbox.SetActive(true);

        Time.timeScale = 0f;
    }

    public void Exit(HandleToolbox context)
    {
        Time.timeScale = 1f;

        foreach (GameObject tb in context.toolboxes)
            tb.SetActive(false);
    }
}