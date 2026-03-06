using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public float directionChangeTime = 2f;

    [Header("Movement Bounds")]
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 4f;

    private Vector2 moveDirection;
    private float directionTimer;

    void Start()
    {
        PickNewDirection();
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0f)
        {
            PickNewDirection();
        }

        KeepInsideBounds();
        FlipSprite();
    }

    void PickNewDirection()
    {
        moveDirection = Random.insideUnitCircle.normalized;
        directionTimer = directionChangeTime;
    }

    void KeepInsideBounds()
    {
        Vector3 pos = transform.position;

        if (pos.x < minX || pos.x > maxX)
        {
            moveDirection.x *= -1;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
        }

        if (pos.y < minY || pos.y > maxY)
        {
            moveDirection.y *= -1;
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
        }

        transform.position = pos;
    }

    void FlipSprite()
    {
        if (moveDirection.x > 0.05f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (moveDirection.x < -0.05f)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }
}