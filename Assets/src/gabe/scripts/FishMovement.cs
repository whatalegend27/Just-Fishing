using UnityEngine;
using Saif.GamePlay;

public class FishMovement : MonoBehaviour
{
    // ===== BASIC MOVEMENT SETTINGS =====
    [Header("Movement")]
    public float minSpeed = 1.5f;
    public float maxSpeed = 2.5f;
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

    // ===== BAIT FOLLOW SETTINGS =====
    [Header("Bait Following")]
    public bool followBait = true;
    public float baitDetectionRange = 5f;
    public float baitFollowSpeedMultiplier = 1.4f;

    private HookBait targetBait;

    // ===== PRIVATE DATA CLASS =====
    private FishState state = new FishState();

    private class FishState
    {
        public Vector2 currentDirection;
        public Vector2 targetDirection;
        public float currentSpeed;
        public float directionTimer;
        public float wiggleOffset;
    }

    // ===== START =====
    void Start()
    {
        state.wiggleOffset = Random.Range(0f, 10f);
        PickNewDirection(true);

        // Find bait in scene
        targetBait = Object.FindFirstObjectByType<HookBait>();
    }

    // ===== UPDATE =====
    void Update()
    {
        if (ShouldFollowBait())
        {
            FollowBait();
        }
        else
        {
            NormalSwimMovement();
        }

        KeepInsideBounds();
        FlipSprite();
    }

    // ===== NORMAL MOVEMENT =====
    void NormalSwimMovement()
    {
        state.directionTimer -= Time.deltaTime;

        if (state.directionTimer <= 0f)
        {
            PickNewDirection(false);
        }

        state.currentDirection = Vector2.Lerp(
            state.currentDirection,
            state.targetDirection,
            turnSpeed * Time.deltaTime
        ).normalized;

        Vector2 move = state.currentDirection;

        move.y += Mathf.Sin(
            Time.time * verticalWiggleSpeed + state.wiggleOffset
        ) * verticalWiggleAmount;

        transform.Translate(
            move.normalized * state.currentSpeed * Time.deltaTime,
            Space.World
        );
    }

    // ===== CHECK IF SHOULD FOLLOW BAIT =====
    bool ShouldFollowBait()
    {
        if (!followBait) return false;
        if (targetBait == null) return false;

        // Get the bait sprite renderer WITHOUT modifying HookBait
        SpriteRenderer baitSprite = targetBait.GetComponent<SpriteRenderer>();

        if (baitSprite == null) return false;
        if (!baitSprite.enabled) return false;

        float distanceToBait = Vector2.Distance(
            transform.position,
            targetBait.GetBaitWorldPosition()
        );

        return distanceToBait <= baitDetectionRange;
    }

    // ===== FOLLOW BAIT =====
    void FollowBait()
    {
        Vector2 baitPosition = targetBait.GetBaitWorldPosition();
        Vector2 fishPosition = transform.position;

        Vector2 directionToBait = (baitPosition - fishPosition).normalized;

        state.currentDirection = Vector2.Lerp(
            state.currentDirection,
            directionToBait,
            turnSpeed * Time.deltaTime
        ).normalized;

        Vector2 move = state.currentDirection;

        // keep wiggle so it still looks natural
        move.y += Mathf.Sin(
            Time.time * verticalWiggleSpeed + state.wiggleOffset
        ) * verticalWiggleAmount;

        float baitSpeed = state.currentSpeed * baitFollowSpeedMultiplier;

        transform.Translate(
            move.normalized * baitSpeed * Time.deltaTime,
            Space.World
        );
    }

    // ===== PICK NEW DIRECTION =====
    void PickNewDirection(bool forceInitialDirection)
    {
        float horizontal = forceInitialDirection
            ? (Random.value < 0.5f ? -1f : 1f)
            : Mathf.Sign(
                state.currentDirection.x == 0
                ? (Random.value < 0.5f ? -1f : 1f)
                : state.currentDirection.x
              );

        float vertical = Random.Range(-0.4f, 0.4f);

        state.targetDirection = new Vector2(horizontal, vertical).normalized;

        // ===== DYNAMIC BINDING =====
        state.currentSpeed = ChooseSpeed();

        state.directionTimer =
            directionChangeTime + Random.Range(-0.5f, 0.5f);

        if (forceInitialDirection)
        {
            state.currentDirection = state.targetDirection;
        }
    }

    // ===== DYNAMIC METHOD =====
    protected virtual float ChooseSpeed()
    {
        return Random.Range(minSpeed, maxSpeed);
    }

    // ===== KEEP INSIDE BOUNDS =====
    void KeepInsideBounds()
    {
        Vector3 pos = transform.position;

        if (pos.x < minX)
        {
            pos.x = minX;
            state.targetDirection.x = Mathf.Abs(state.targetDirection.x);
            state.currentDirection.x = Mathf.Abs(state.currentDirection.x);
        }
        else if (pos.x > maxX)
        {
            pos.x = maxX;
            state.targetDirection.x = -Mathf.Abs(state.targetDirection.x);
            state.currentDirection.x = -Mathf.Abs(state.currentDirection.x);
        }

        if (pos.y < minY)
        {
            pos.y = minY;
            state.targetDirection.y = Mathf.Abs(state.targetDirection.y);
            state.currentDirection.y = Mathf.Abs(state.currentDirection.y);
        }
        else if (pos.y > maxY)
        {
            pos.y = maxY;
            state.targetDirection.y = -Mathf.Abs(state.targetDirection.y);
            state.currentDirection.y = -Mathf.Abs(state.currentDirection.y);
        }

        transform.position = pos;
    }

    // ===== FLIP SPRITE =====
    void FlipSprite()
    {
        if (state.currentDirection.x > 0.05f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (state.currentDirection.x < -0.05f)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }
}