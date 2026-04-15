using UnityEngine;

[CreateAssetMenu(fileName = "ItemScript", menuName = "Scriptable Objects/ItemScript")]
public class ItemScript : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField][TextArea] private string itemDescription;
    [SerializeField] private int price;
    [SerializeField] private Sprite icon;

    //Getters that return values
    public string ItemName => itemName;
    public int Price => price;
    public Sprite Icon => icon;
    public string ItemDescription => itemDescription;

    // public bool CanStack()
    public virtual bool CanStack()
    {
        return false;
    }
}