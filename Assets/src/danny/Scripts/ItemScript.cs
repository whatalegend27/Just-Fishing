using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemScript", menuName = "Scriptable Objects/ItemScript")]
public class ItemScript : ScriptableObject
{
    public string itemName;
    [TextArea]public string itemDescription;
    public int price;
    public Sprite icon;

    public virtual string getDescription()
    {
        return itemDescription;
    }
}

// public class StackableItem : ItemScript
// {
//     int maxStack;

//     public override string getDescription()
//     {
//         return itemDescription + "\nStacks up to:" + maxStack;
//     }
// }