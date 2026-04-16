using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item")]
public class Item : ScriptableObject
{
    [SerializeField] private string itemID;
    [SerializeField] private string itemName;
    [SerializeField] private Rarities rarity;
    [SerializeField][TextArea] private string itemDescription;
    [SerializeField] private Sprite itemSprite;

    public string ItemID => itemID;
    public string ItemName => itemName;
    public Rarities Rarity => rarity;
    public string ItemDescription => itemDescription;
    public Sprite ItemSprite => itemSprite;
}
