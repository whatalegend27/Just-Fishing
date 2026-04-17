using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{

  [SerializeField] private SpriteRenderer dialogueBoxRenderer;
  public bool isBoatActive = false;
  public float timeBetweenDrifts = 5f; // Seconds
  private Vector2 boatCurrentPosition;

  void Start()
  {
    if (isBoatActive)
    {
      boatCurrentPosition = new Vector2(transform.position.x, transform.position.y);
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

            Vector2 driftAmount = CalculateDriftOffset(boatCurrentPosition);
            boatCurrentPosition += driftAmount;
            
            transform.position = new Vector3(boatCurrentPosition.x, boatCurrentPosition.y, 0f);
        }
    }

}