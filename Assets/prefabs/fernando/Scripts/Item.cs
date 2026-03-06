using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item")]

public class Item : ScriptableObject
{
    public string ItemID;
    public string Name;
    public Rarities rarity;
    [TextArea]
    public string ItemDescp;
    public Sprite itemSprite;




}
