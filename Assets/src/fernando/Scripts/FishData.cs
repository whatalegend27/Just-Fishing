using UnityEngine;

public enum FishRarity { Common, Rare, Legendary }

[System.Serializable]
public class FishData
{
    public string fishName;
    public bool fishKnown;
    public int catchCount;
    public FishRarity rarity;
}