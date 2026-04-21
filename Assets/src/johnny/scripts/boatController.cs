using System.Collections;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer dialogueBoxRenderer;
    public bool isBoatActive = false;
    public float timeBetweenDrifts = 5f; // Seconds
    private Vector2 boatCurrentPosition;

    public float Speed { get; set; } = 0f;
    public float CurrentForce { get; set; } = 0f;
    public float WindForce { get; set; } = 0f;
    // -------------------------------------------------------------------------

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

        float baseDrift = 2f;
        float driftIntensity = baseDrift + Mathf.Abs(Speed) + Mathf.Abs(CurrentForce) + Mathf.Abs(WindForce);
        
        driftIntensity = Mathf.Clamp(driftIntensity, 0f, 15f);

        do
        {
            float driftX = Random.Range(-driftIntensity, driftIntensity);
            float driftY = Random.Range(-driftIntensity, driftIntensity);
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