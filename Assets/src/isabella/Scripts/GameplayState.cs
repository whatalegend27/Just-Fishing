using UnityEngine;

public class GameplayState : IToolboxState
{
    public void Enter(HandleToolbox context)
    {
        if (context.tbShow != null)
            context.tbShow.SetActive(false);

        Time.timeScale = 1f;
    }

    public void Exit(HandleToolbox context) { }
}