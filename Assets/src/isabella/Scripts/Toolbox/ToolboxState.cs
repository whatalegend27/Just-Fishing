using UnityEngine;

// This class represents the state of the toolbox when it is open, managing the visibility of the active toolbox and pausing the game.
public class ToolboxState : IToolboxState
{
    private GameObject activeToolbox;

    // Constructor to set the active toolbox when entering this state
    public ToolboxState(GameObject toolbox)
    {
        activeToolbox = toolbox;
    }

    // When entering the toolbox state, hide all toolboxes, show the active one, and pause the game
    public void Enter(HandleToolbox context)
    {
        foreach (GameObject tb in context.toolboxes)
            tb.SetActive(false);

        if (activeToolbox != null)
            activeToolbox.SetActive(true);

        Time.timeScale = 0f;
    }

    // When exiting the toolbox state, hide all toolboxes and resume the game
    public void Exit(HandleToolbox context)
    {
        Time.timeScale = 1f;

        foreach (GameObject tb in context.toolboxes)
            tb.SetActive(false);
    }
}