// IToolboxState.cs

// Defines the interface for toolbox states, ensuring consistent behavior across different states
public interface IToolboxState
{
    void Enter(HandleToolbox context);
    void Exit(HandleToolbox context);
}