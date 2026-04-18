using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [Header("Movement")]
    public float minSpeed = 1.5f;
    public float maxSpeed = 2.5f;
    public float turnSpeed = 2f;
    public float directionChangeTime = 2.5f;

    [Header("Swim Motion")]
    public float verticalWiggleAmount = 0.3f;
    public float verticalWiggleSpeed = 2f;

    [Header("Movement Bounds")]
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 4f;

    private Vector2 currentDirection;
    private Vector2 targetDirection;
    private float currentSpeed;
    private float directionTimer;
    private float wiggleOffset;

    void Start()
    {
        wiggleOffset = Random.Range(0f, 10f);
        PickNewDirection(true);
    }

    void Update()
    {
        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0f)
        {
            PickNewDirection(false);
        }

        // Smoothly turn toward target direction
        currentDirection = Vector2.Lerp(
            currentDirection,
            targetDirection,
            turnSpeed * Time.deltaTime
        ).normalized;

        // Add gentle up/down swimming motion
        Vector2 move = currentDirection;
        move.y += Mathf.Sin(Time.time * verticalWiggleSpeed + wiggleOffset) * verticalWiggleAmount;

        // Move fish
        transform.Translate(move.normalized * currentSpeed * Time.deltaTime, Space.World);

        KeepInsideBounds();
        FlipSprite();
    }

    void PickNewDirection(bool forceInitialDirection)
    {
        // Fish should mostly move left or right, not in totally random circles
        float horizontal = forceInitialDirection
            ? (Random.value < 0.5f ? -1f : 1f)
            : Mathf.Sign(currentDirection.x == 0 ? (Random.value < 0.5f ? -1f : 1f) : currentDirection.x);

        // Small vertical drift instead of full random vertical movement
        float vertical = Random.Range(-0.4f, 0.4f);

        targetDirection = new Vector2(horizontal, vertical).normalized;

        currentSpeed = Random.Range(minSpeed, maxSpeed);
        directionTimer = directionChangeTime + Random.Range(-0.5f, 0.5f);

        if (forceInitialDirection)
        {
            currentDirection = targetDirection;
        }
    }

    void KeepInsideBounds()
    {
        Vector3 pos = transform.position;

        // Bounce horizontally
        if (pos.x < minX)
        {
            pos.x = minX;
            targetDirection.x = Mathf.Abs(targetDirection.x);
            currentDirection.x = Mathf.Abs(currentDirection.x);
        }
        else if (pos.x > maxX)
        {
            pos.x = maxX;
            targetDirection.x = -Mathf.Abs(targetDirection.x);
            currentDirection.x = -Mathf.Abs(currentDirection.x);
        }

        // Push back vertically
        if (pos.y < minY)
        {
            pos.y = minY;
            targetDirection.y = Mathf.Abs(targetDirection.y);
            currentDirection.y = Mathf.Abs(currentDirection.y);
        }
        else if (pos.y > maxY)
        {
            pos.y = maxY;
            targetDirection.y = -Mathf.Abs(targetDirection.y);
            currentDirection.y = -Mathf.Abs(currentDirection.y);
        }

        transform.position = pos;
    }

    void FlipSprite()
    {
        if (currentDirection.x > 0.05f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (currentDirection.x < -0.05f)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }
}