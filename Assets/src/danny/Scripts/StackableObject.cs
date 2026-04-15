using UnityEngine;

[CreateAssetMenu(fileName = "StackableItem", menuName = "Scriptable Objects/Stackable Item")]
public class StackableItem : ItemScript
{
    // public new bool CanStack()
    public override bool CanStack()
    {
        return true;
    }
}