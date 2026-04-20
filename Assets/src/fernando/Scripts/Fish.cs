using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] private string fishName;
    [SerializeField] private ItemScript itemData;

    public string FishName => fishName;
    public ItemScript ItemData => itemData;
}
