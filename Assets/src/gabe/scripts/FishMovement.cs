using UnityEngine;

// This script controls how each fish moves in the water.
// The fish swims smoothly, changes direction occasionally,
// wiggles slightly up and down, stays inside boundaries,
// and flips to face the direction it is moving.
public class FishMovement : MonoBehaviour
{
    // ===== BASIC MOVEMENT SETTINGS =====
    [Header("Movement")]

    public float minSpeed = 1.5f;

    public float maxSpeed = 2.5f;

    // higher = faster turning
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
    // Groups all runtime movement values together.
    // These values change frequently during gameplay,
    // one structure for organization.
    private FishState state = new FishState();


    // This class stores the fish's current movement condition.
    private class FishState
    {

        public Vector2 currentDirection;

        public Vector2 targetDirection;

        public float currentSpeed;


        public float directionTimer;


        public float wiggleOffset;
    }

    // Runs once when the fish is first created
    void Start()
    {
        // Randomize wiggle timing , don't move identically
        state.wiggleOffset = Random.Range(0f, 10f);

        PickNewDirection(true);
    }


    void Update()
    {
        // Decrease direction timer
        state.directionTimer -= Time.deltaTime;

        // When timer reaches 0, new direction
        if (state.directionTimer <= 0f)
        {
            PickNewDirection(false);
        }

        // Smoothly rotate current direction toward the target direction
        // prevent sharp robotic turning
        state.currentDirection = Vector2.Lerp(
            state.currentDirection,
            state.targetDirection,
            turnSpeed * Time.deltaTime
        ).normalized;



        Vector2 move = state.currentDirection;

        // create a natural swimming wiggle
        move.y += Mathf.Sin(
            Time.time * verticalWiggleSpeed + state.wiggleOffset
        ) * verticalWiggleAmount;


        transform.Translate(
            move.normalized * state.currentSpeed * Time.deltaTime,
            Space.World
        );


        KeepInsideBounds();

        FlipSprite();
    }


    // ===== PICK NEW DIRECTION =====
    void PickNewDirection(bool forceInitialDirection)
    {
        // prevents spinning in random circles
        float horizontal = forceInitialDirection
            ? (Random.value < 0.5f ? -1f : 1f)

            : Mathf.Sign(
                state.currentDirection.x == 0
                ? (Random.value < 0.5f ? -1f : 1f)
                : state.currentDirection.x
              );


        // Small vertical movement
        float vertical = Random.Range(-0.4f, 0.4f);


        // Set the new desired direction
        state.targetDirection = new Vector2(horizontal, vertical).normalized;


        // Choose a random speed within allowed range
        state.currentSpeed = Random.Range(minSpeed, maxSpeed);


        // Reset timer until next direction change
        state.directionTimer =
            directionChangeTime + Random.Range(-0.5f, 0.5f);


        // If this is the first direction, apply immediately
        if (forceInitialDirection)
        {
            state.currentDirection = state.targetDirection;
        }
    }


    // ===== KEEP FISH INSIDE BOUNDS =====
    void KeepInsideBounds()
    {
        Vector3 pos = transform.position;


        // Horizontal boundary check
        if (pos.x < minX)
        {
            // move fish back inside
            pos.x = minX;

            // force fish to swim right
            state.targetDirection.x = Mathf.Abs(state.targetDirection.x);
            state.currentDirection.x = Mathf.Abs(state.currentDirection.x);
        }

        else if (pos.x > maxX)
        {
            // move fish back inside
            pos.x = maxX;

            // force fish to swim left
            state.targetDirection.x = -Mathf.Abs(state.targetDirection.x);
            state.currentDirection.x = -Mathf.Abs(state.currentDirection.x);
        }


        // Vertical boundary check
        if (pos.y < minY)
        {
            pos.y = minY;

            // push fish upward
            state.targetDirection.y = Mathf.Abs(state.targetDirection.y);
            state.currentDirection.y = Mathf.Abs(state.currentDirection.y);
        }

        else if (pos.y > maxY)
        {
            pos.y = maxY;

            // push fish downward
            state.targetDirection.y = -Mathf.Abs(state.targetDirection.y);
            state.currentDirection.y = -Mathf.Abs(state.currentDirection.y);
        }


        // Apply corrected position
        transform.position = pos;
    }


    // ===== SPRITE FLIPPING =====
    //is now redunant, sprites fixed
    void FlipSprite()
    {
        // If moving right, face right
        if (state.currentDirection.x > 0.05f)
            transform.localScale = new Vector3(1f, 1f, 1f);

        // If moving left, face left
        else if (state.currentDirection.x < -0.05f)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }
}