using UnityEngine;

[CreateAssetMenu(fileName = "NewFish", menuName = "Fishing/FishingSpecies")]
public class FishSpecies : ScriptableObject
{
    public string FishID;
    public string Name;
    public Rarities rarity;
    public string ItemDescp;
    public Sprite fishSprite;


}
