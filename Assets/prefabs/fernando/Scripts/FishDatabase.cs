using UnityEngine;

[CreateAssetMenu(fileName = "FishDatabase", menuName = "Fishing/Fish Database")]
public class FishDatabase : ScriptableObject
{
    public FishSpecies[] allFish;
}