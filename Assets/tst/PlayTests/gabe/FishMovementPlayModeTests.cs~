using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class FishMovementPlayModeTests
{
    private GameObject fishObject;
    private FishMovement movement;

    [SetUp]
    public void SetUp()
    {
        fishObject = new GameObject("TestFish");
        movement = fishObject.AddComponent<FishMovement>();

        // Set simple test bounds
        movement.minX = -5f;
        movement.maxX = 5f;
        movement.minY = -3f;
        movement.maxY = 3f;

        // Make movement predictable enough for testing
        movement.minSpeed = 2f;
        movement.maxSpeed = 2f;
        movement.turnSpeed = 10f;
        movement.directionChangeTime = 5f;

        // Reduce wiggle effect so tests are more stable
        movement.verticalWiggleAmount = 0f;
        movement.verticalWiggleSpeed = 0f;

        fishObject.transform.position = Vector3.zero;
        fishObject.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    [TearDown]
    public void TearDown()
    {
        if (fishObject != null)
        {
            Object.Destroy(fishObject);
        }
    }

    [UnityTest]
    public IEnumerator Fish_Moves_After_Start()
    {
        Vector3 startPosition = fishObject.transform.position;

        yield return null;
        yield return null;

        Vector3 endPosition = fishObject.transform.position;

        Assert.AreNotEqual(startPosition, endPosition, "Fish should move after Start and Update run.");
    }

    [UnityTest]
    public IEnumerator Fish_Stays_Within_Bounds()
    {
        fishObject.transform.position = Vector3.zero;

        for (int i = 0; i < 120; i++)
        {
            yield return null;
        }

        Vector3 pos = fishObject.transform.position;

        Assert.GreaterOrEqual(pos.x, movement.minX);
        Assert.LessOrEqual(pos.x, movement.maxX);
        Assert.GreaterOrEqual(pos.y, movement.minY);
        Assert.LessOrEqual(pos.y, movement.maxY);
    }

    [UnityTest]
    public IEnumerator Fish_Clamps_Back_Inside_Left_Bound()
    {
        yield return null;

        fishObject.transform.position = new Vector3(movement.minX - 2f, 0f, 0f);

        yield return null;

        Assert.GreaterOrEqual(fishObject.transform.position.x, movement.minX,
            "Fish should be clamped back inside the left bound.");
    }

    [UnityTest]
    public IEnumerator Fish_Clamps_Back_Inside_Right_Bound()
    {
        yield return null;

        fishObject.transform.position = new Vector3(movement.maxX + 2f, 0f, 0f);

        yield return null;

        Assert.LessOrEqual(fishObject.transform.position.x, movement.maxX,
            "Fish should be clamped back inside the right bound.");
    }

    [UnityTest]
    public IEnumerator Fish_Clamps_Back_Inside_Top_And_Bottom_Bounds()
    {
        yield return null;

        fishObject.transform.position = new Vector3(0f, movement.maxY + 2f, 0f);
        yield return null;
        Assert.LessOrEqual(fishObject.transform.position.y, movement.maxY,
            "Fish should be clamped back inside the top bound.");

        fishObject.transform.position = new Vector3(0f, movement.minY - 2f, 0f);
        yield return null;
        Assert.GreaterOrEqual(fishObject.transform.position.y, movement.minY,
            "Fish should be clamped back inside the bottom bound.");
    }

    [UnityTest]
    public IEnumerator Fish_Continues_Moving_Over_Time()
    {
        yield return null;
        Vector3 pos1 = fishObject.transform.position;

        for (int i = 0; i < 30; i++)
        {
            yield return null;
        }

        Vector3 pos2 = fishObject.transform.position;

        Assert.AreNotEqual(pos1, pos2, "Fish should continue moving over multiple frames.");
    }
}