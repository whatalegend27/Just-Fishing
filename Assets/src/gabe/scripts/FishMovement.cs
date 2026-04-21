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
    public float minY = -9f;
    public float maxY = 3f;

    [Header("Sprite Direction")]
    public bool spriteFacesRight = true;

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

        currentDirection = Vector2.Lerp(
            currentDirection,
            targetDirection,
            turnSpeed * Time.deltaTime
        ).normalized;

        Vector2 move = currentDirection;
        move.y += Mathf.Sin(Time.time * verticalWiggleSpeed + wiggleOffset) * verticalWiggleAmount;

        transform.Translate(move.normalized * currentSpeed * Time.deltaTime, Space.World);

        KeepInsideBounds();
        FlipSprite();
    }

    void PickNewDirection(bool forceInitialDirection)
    {
        float horizontal = forceInitialDirection
            ? (Random.value < 0.5f ? -1f : 1f)
            : Mathf.Sign(currentDirection.x == 0 ? (Random.value < 0.5f ? -1f : 1f) : currentDirection.x);

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
        Vector3 scale = transform.localScale;
        float absX = Mathf.Abs(scale.x);

        if (currentDirection.x > 0.05f)
        {
            scale.x = spriteFacesRight ? absX : -absX;
        }
        else if (currentDirection.x < -0.05f)
        {
            scale.x = spriteFacesRight ? -absX : absX;
        }

        transform.localScale = scale;
    }
}