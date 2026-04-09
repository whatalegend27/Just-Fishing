using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boatController : MonoBehaviour
{
  public bool isBoatActive = false;
  public float timeBetweenDrifts = 5f; // Seconds
  public HashSet<Vector2> oceanRocksLocation; 

  [SerializeField] private SpriteRenderer dialogueBoxRenderer;

  private Vector2 currentPosition;

  void Start()
  {
    if (isBoatActive)
    {
      currentPosition = new Vector2(transform.position.x, transform.position.y);
      StartCoroutine(DriftRoutine());
    }
  }

    // Public for testing purposes, but could be private in the final version
    public Vector2 CalculateDriftOffset(Vector2 startPosition)
    {
        Vector2 driftOffset;
        Vector2 proposedPosition;
        int failsafe = 0;

        do
        {
            float driftX = Random.Range(-2f, 2f);
            float driftY = Random.Range(-2f, 2f);
            driftOffset = new Vector2(driftX, driftY);
            
            proposedPosition = startPosition + driftOffset;
            
            failsafe++;
            if (failsafe > 100) break;

        } while (proposedPosition.x < -20f || proposedPosition.x > 20f || proposedPosition.y < -20f || proposedPosition.y > 20f);

        return driftOffset;
    }

    IEnumerator DriftRoutine()
    {
        while (isBoatActive)
        {
            yield return new WaitForSeconds(timeBetweenDrifts);

            Vector2 driftAmount = CalculateDriftOffset(currentPosition);
            currentPosition += driftAmount;
            
            transform.position = new Vector3(currentPosition.x, currentPosition.y, 0f);
            CheckForCollisions();
        }
    }

  private void CheckForCollisions()
  {
    Vector2 roundedPosition = new Vector2(Mathf.Round(currentPosition.x), Mathf.Round(currentPosition.y));

    if (oceanRocksLocation != null && oceanRocksLocation.Contains(roundedPosition))
    {
      Debug.Log("Boat hit a rock!");
      // dialogueBoxRenderer.enabled = true; // Show dialogue box on collision
      // 
      // boat.Destroy();
    }
  }
}