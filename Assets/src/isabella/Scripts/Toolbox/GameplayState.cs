using UnityEngine;

public class GameplayState : IToolboxState
{
    public void Enter(HandleToolbox context)
    {
        // Close ALL toolboxes (clean reset)
        foreach (GameObject tb in context.toolboxes)
            tb.SetActive(false);

        // Resume gameplay
        Time.timeScale = 1f;
    }

    // Needed to implement the interface.
    public void Exit(HandleToolbox context)
    {
    }

}