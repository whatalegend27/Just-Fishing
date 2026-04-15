using UnityEngine;

public class ToolboxState : IToolboxState
{
    public void Enter(HandleToolbox context)
    {
        foreach (GameObject tb in context.toolboxes)
            tb.SetActive(false);

        if (context.tbShow != null)
            context.tbShow.SetActive(true);

        Time.timeScale = 0f;
    }

    public void Exit(HandleToolbox context) { }
}