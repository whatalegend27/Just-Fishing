using UnityEngine;

// Base class that ALL fish types use.
// This defines general movement behavior.
// Subclasses (like FastFishMovement) will override parts of this.
public class FishMovement : MonoBehaviour
{
    // ===== BASIC MOVEMENT SETTINGS =====
    [Header("Movement")]

    public float minSpeed = 1.5f;
    public float maxSpeed = 2.5f;

    // How fast the fish turns (higher = smoother/faster turning)
    public float turnSpeed = 2f;

    public float directionChangeTime = 2.5f;


    // ===== NATURAL SWIMMING SETTINGS =====
    [Header("Swim Motion")]

    public float verticalWiggleAmount = 0.3f;

    public float verticalWiggleSpeed = 2f;


    // ===== MOVEMENT AREA LIMITS =====
    [Header("Movement Bounds")]

    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 4f;


    // ===== PRIVATE DATA CLASS =====
    // Holds all runtime values for movement.
    // Keeps things clean and organized instead of many separate variables.
    private FishState state = new FishState();

    private class FishState
    {
        public Vector2 currentDirection; // where fish currently moving
        public Vector2 targetDirection;  // where fish want to move
        public float currentSpeed;       // current movement speed
        public float directionTimer;     // countdown to direction change
        public float wiggleOffset;       // fish don't wiggle identically
    }


    // ==== START =====
    void Start()
    {
        //all fish don't wiggle the same
        state.wiggleOffset = Random.Range(0f, 10f);

        // Pick the first direction immediately
        PickNewDirection(true);
    }


    // ===== UPDATE =====
    void Update()
    {
        // Count down until we change direction
        state.directionTimer -= Time.deltaTime;

        // If timer runs out, pick a new direction
        if (state.directionTimer <= 0f)
        {
            PickNewDirection(false);
        }

        // Smoothly rotate toward target direction
        // This prevents sharp robotic turning
        state.currentDirection = Vector2.Lerp(
            state.currentDirection,
            state.targetDirection,
            turnSpeed * Time.deltaTime
        ).normalized;

        // Base movement direction
        Vector2 move = state.currentDirection;

        // Add vertical wiggle to simulate swimming
        move.y += Mathf.Sin(
            Time.time * verticalWiggleSpeed + state.wiggleOffset
        ) * verticalWiggleAmount;

        // Move the fish
        transform.Translate(
            move.normalized * state.currentSpeed * Time.deltaTime,
            Space.World
        );

        // Keep fish inside boundaries
        KeepInsideBounds();

        // Flip sprite to match direction
        FlipSprite();
    }


    // ===== PICK NEW DIRECTION =====
    void PickNewDirection(bool forceInitialDirection)
    {
        // Choose horizontal direction (left/right)
        float horizontal = forceInitialDirection
            ? (Random.value < 0.5f ? -1f : 1f)
            : Mathf.Sign(
                state.currentDirection.x == 0
                ? (Random.value < 0.5f ? -1f : 1f)
                : state.currentDirection.x
              );

        // Small vertical variation
        float vertical = Random.Range(-0.4f, 0.4f);

        // Set new target direction
        state.targetDirection = new Vector2(horizontal, vertical).normalized;

        // ===== DYNAMIC BINDING HAPPENS HERE =====
        // This calls ChooseSpeed(), BUT:
        // If the object is a subclass (FastFishMovement),
        // it will call the OVERRIDDEN version instead.
        state.currentSpeed = ChooseSpeed();

        // Reset timer for next direction change
        state.directionTimer =
            directionChangeTime + Random.Range(-0.5f, 0.5f);

        // Apply immediately if first time
        if (forceInitialDirection)
        {
            state.currentDirection = state.targetDirection;
        }
    }


    // ===== DYNAMIC METHOD =====
    // This is VIRTUAL → can be overridden by subclasses
    // Default behavior: normal fish speed
    protected virtual float ChooseSpeed()
    {
        return Random.Range(minSpeed, maxSpeed);
    }


    // ===== KEEP FISH INSIDE BOUNDS =====
    void KeepInsideBounds()
    {
        Vector3 pos = transform.position;

        // Left boundary
        if (pos.x < minX)
        {
            pos.x = minX;
            state.targetDirection.x = Mathf.Abs(state.targetDirection.x);
            state.currentDirection.x = Mathf.Abs(state.currentDirection.x);
        }
        // Right boundary
        else if (pos.x > maxX)
        {
            pos.x = maxX;
            state.targetDirection.x = -Mathf.Abs(state.targetDirection.x);
            state.currentDirection.x = -Mathf.Abs(state.currentDirection.x);
        }

        // Bottom boundary
        if (pos.y < minY)
        {
            pos.y = minY;
            state.targetDirection.y = Mathf.Abs(state.targetDirection.y);
            state.currentDirection.y = Mathf.Abs(state.currentDirection.y);
        }
        // Top boundary
        else if (pos.y > maxY)
        {
            pos.y = maxY;
            state.targetDirection.y = -Mathf.Abs(state.targetDirection.y);
            state.currentDirection.y = -Mathf.Abs(state.currentDirection.y);
        }

        transform.position = pos;
    }


    // ===== STATICALLY BOUND METHOD =====
    // NOT virtual → cannot be overridden
    // Always uses THIS version, even in subclasses
    void FlipSprite()
    {
        if (state.currentDirection.x > 0.05f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (state.currentDirection.x < -0.05f)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }
}