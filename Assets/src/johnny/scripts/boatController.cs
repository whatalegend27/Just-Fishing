using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boatController : MonoBehaviour
{
  public bool isBoatActive = false;
  public float timeBetweenDrifts = 5f; // Seconds
  public HashSet<Vector2> oceanRocksLocation; 

  private Vector2 currentPosition;

  void Start()
  {
    if (isBoatActive)
    {
      currentPosition = new Vector2(transform.position.x, transform.position.y);
      StartCoroutine(DriftRoutine());
    }
  }

  IEnumerator DriftRoutine()
  {
    while (isBoatActive)
    {
      yield return new WaitForSeconds(timeBetweenDrifts);

      Vector2 newPosition;
      int failsafe = 0;

      do
      {
        float driftX = Random.Range(-2f, 2f);
        float driftY = Random.Range(-2f, 2f);

        newPosition = currentPosition + new Vector2(driftX, driftY);

        failsafe++;
        if (failsafe > 100) break;

      } while (newPosition.x < -20f || newPosition.x > 20f || newPosition.y < -20f || newPosition.y > 20f);

      currentPosition = newPosition;
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
      // boat.Destroy();
      // player.LoseGame();
      isBoatActive = false;
    }
  }
}