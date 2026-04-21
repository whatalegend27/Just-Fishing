using UnityEngine;

[CreateAssetMenu(fileName = "ItemScript", menuName = "Scriptable Objects/ItemScript")]
public class ItemScript : ScriptableObject
{

    public enum ItemType { None, Rod, Lure, Bait, Weight, Clothes , Food, Potions, Fish }
    [SerializeField] private string itemName;
    [SerializeField][TextArea] private string itemDescription;
    [SerializeField] private int price;
    [SerializeField] private Sprite icon;
    [SerializeField] private ItemType itemType;

    //for Isabella
    public int attractionValue;

    //Getters that return values
    public string ItemName => itemName;
    public int Price => price;
    public Sprite Icon => icon;
    public string ItemDescription => itemDescription;
    public ItemType Type => itemType;

    // public bool CanStack()
    public virtual bool CanStack()
    {
        return false;
    }
}