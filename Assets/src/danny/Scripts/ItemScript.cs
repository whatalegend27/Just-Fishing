using UnityEngine;

[CreateAssetMenu(fileName = "ItemScript", menuName = "Scriptable Objects/ItemScript")]
public class ItemScript : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] [TextArea] public string itemDescription;
    [SerializeField] private int price;
    [SerializeField] private Sprite icon;

    //Getters that return values
    public string ItemName => itemName;
    public int Price => price;
    public Sprite Icon => icon;

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