using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] private string fishName;

    public string FishName => fishName;
}
