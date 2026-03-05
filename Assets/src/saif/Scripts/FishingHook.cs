using UnityEngine;

public class FishingHook : MonoBehaviour
{
    [Header("Speeds")]
    public float sinkSpeed = 2f;
    public float moveSpeed = 5f;
    public float reelSpeed = 3.5f; 

    [Header("Borders & Limits")]
    public float maxDepth = -10f;
    public float surfaceLevel = 0f; 
    public float leftBorder = -8f;
    public float rightBorder = 8f;

    [Header("References")]
    public LineRenderer line;
    public Transform rodTip;

    private bool hasCaughtFish = false;
    private bool isReadyToCast = true; 
    private Transform caughtFishTransform;

    void Update()
    {
        // 1. Draw the Line
        if (line != null && rodTip != null)
        {
            line.SetPosition(0, rodTip.position);
            line.SetPosition(1, transform.position);
        }

        // 2. Casting Logic
        if (isReadyToCast)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                isReadyToCast = false;
            }
            return; 
        }

        // 3. Inputs
        float h = Input.GetAxis("Horizontal");
        float newX = transform.position.x;
        float newY = transform.position.y;

        // 4. Movement Logic
        if (Input.GetKey(KeyCode.Space))
        {
            newY += reelSpeed * Time.deltaTime;

            float distanceToSurface = surfaceLevel - newY;
            
            // If close to the surface, handle the diagonal "Homing"
            if (distanceToSurface < 3f)
            {
                // Move toward 0 (the boat) but allow player to nudge it
                newX = Mathf.MoveTowards(newX, 0, (moveSpeed * 0.5f) * Time.deltaTime);
                newX += h * (moveSpeed * 0.5f) * Time.deltaTime;
            }
            else 
            {
                newX += h * moveSpeed * Time.deltaTime;
            }

            // Reset when home
            if (newY >= surfaceLevel)
            {
                newY = surfaceLevel;
                newX = 0; 
                isReadyToCast = true;
                if (hasCaughtFish) CollectFish();
            }
        }
        else 
        {
            newX += h * moveSpeed * Time.deltaTime;

            if (!hasCaughtFish && newY > maxDepth)
            {
                newY -= sinkSpeed * Time.deltaTime;
            }
        }

        // Final Position Clamp
        newX = Mathf.Clamp(newX, leftBorder, rightBorder);
        newY = Mathf.Clamp(newY, maxDepth, surfaceLevel);
        transform.position = new Vector3(newX, newY, transform.position.z);

        if (hasCaughtFish && caughtFishTransform != null)
        {
            caughtFishTransform.position = transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasCaughtFish && collision.CompareTag("Fish"))
        {
            hasCaughtFish = true;
            caughtFishTransform = collision.transform;
        }
    }

    void CollectFish()
    {
        if (caughtFishTransform != null) Destroy(caughtFishTransform.gameObject);
        hasCaughtFish = false;
        caughtFishTransform = null;
    }
}