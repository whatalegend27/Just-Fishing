using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

// BREAKING POINT:
// Varies depending on random circumstances, but should reliably fail at some point between 1000 and 5000 units of speed on the player. Failures at as low as 22.5 were seen during early testing.

public class CollisionStressTests
{
    private GameObject playerObject;
    private GameObject boatObject;

    [UnityTest]
    public IEnumerator Player_CannotTunnelThroughBoatWalls_AtHighSpeeds()
    {
        
        GameObject playerPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/prefabs/isabella/Player.prefab");
        GameObject boatPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/prefabs/johnny/Boat_Sprite.prefab");

        Assert.IsNotNull(playerPrefab, "Player prefab not found!");
        Assert.IsNotNull(boatPrefab, "Boat prefab not found!");

        boatObject = Object.Instantiate(boatPrefab, Vector3.zero, Quaternion.identity);
        playerObject = Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        MonoBehaviour movementScript = playerObject.GetComponent("PlayerMovement") as MonoBehaviour;
        if (movementScript != null) movementScript.enabled = false; 

        Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb, "Player prefab is missing a Rigidbody2D component!");

        float currentSpeed = 10f;
        float speedMultiplier = 1.5f;
        float maxSpeed = 5000f;
        
        float escapeDistance = 10f; 
        Vector2 boatCenter = boatObject.transform.position;

        while (currentSpeed < maxSpeed)
        {
            // Reset position to the center of the boat
            rb.position = boatCenter;
            rb.linearVelocity = Vector2.zero;
            
            // Wait one physics frame to ensure the reset is applied before launching
            yield return new WaitForFixedUpdate();

            // Pick a random direction and launch
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            rb.linearVelocity = randomDirection * currentSpeed;

            // Wait half a second to allow the collision (or tunneling) to happen
            yield return new WaitForSeconds(0.5f);

            // Check if the player is now outside the boat
            float distanceFromCenter = Vector2.Distance(rb.position, boatCenter);

            if (distanceFromCenter > escapeDistance)
            {
                // Assert.Fail instantly stops the test and marks it as a failure in the Test Runner
                Assert.Fail($"BREACH: Player tunneled through the wall at speed {currentSpeed}!");
            }

            Debug.Log($"Containment held at speed: {currentSpeed}. Increasing speed...");
            currentSpeed *= speedMultiplier;
        }

        Debug.Log($"SUCCESS: Walls withstood speeds up to {maxSpeed} without tunneling.");
    }

    [TearDown]
    public void TearDown()
    {
        if (playerObject != null) Object.DestroyImmediate(playerObject);
        if (boatObject != null) Object.DestroyImmediate(boatObject);
    }
}