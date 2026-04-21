using UnityEngine;

public class BaseAttraction : IFishAttraction
{
    // BaseAttraction provides a default implementation of IFishAttraction.
    public int GetAttraction()
    {
        return 0;
    }
}