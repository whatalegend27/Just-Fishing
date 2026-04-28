using UnityEngine;

// This is a subclass of FishMovement
// inherits all behavior, but overrides speed logic
public class FastFishMovement : FishMovement
{
    [Header("Fast Fish Settings")]

    // Faster speed range than normal fish
    public float fastMinSpeed = 3.5f;
    public float fastMaxSpeed = 5.5f;


    // ===== OVERRIDE METHOD =====
    // This replaces the base class version of ChooseSpeed()
    // only for objects using FastFishMovement
    protected override float ChooseSpeed()
    {
        // Return a faster speed than normal fish
        return Random.Range(fastMinSpeed, fastMaxSpeed);
    }
}