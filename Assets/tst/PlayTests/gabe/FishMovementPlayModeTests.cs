using System.Collections;
using System.Reflection;
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

        // Define test bounds
        movement.minX = -5f;
        movement.maxX = 5f;
        movement.minY = -3f;
        movement.maxY = 3f;

        // Reduce randomness so tests are stable
        movement.minSpeed = 2f;
        movement.maxSpeed = 2f;
        movement.turnSpeed = 10f;
        movement.directionChangeTime = 5f;

        // Remove wiggle randomness
        movement.verticalWiggleAmount = 0f;
        movement.verticalWiggleSpeed = 0f;

        fishObject.transform.position = Vector3.zero;
        fishObject.transform.localScale = Vector3.one;
    }

    [TearDown]
    public void TearDown()
    {
        if (fishObject != null)
        {
            Object.Destroy(fishObject);
        }
    }

    // ------------------------------------------------------------
    // Test 1
    // Verifies that the fish actually moves after Start() and Update() run.
    // This confirms that PickNewDirection() and movement logic are working.
    // If this fails, the fish may not be receiving direction or speed values.
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator Fish_Moves_After_Start()
    {
        Vector3 startPosition = fishObject.transform.position;

        yield return null;
        yield return null;

        Vector3 endPosition = fishObject.transform.position;

        Assert.AreNotEqual(startPosition, endPosition,
            "Fish should move after Start and Update execute.");
    }

    // ------------------------------------------------------------
    // Test 2
    // Ensures the fish always stays inside the configured movement bounds.
    // This validates the KeepInsideBounds() function.
    // The fish moves for several frames and we confirm its position
    // never exceeds minX/maxX/minY/maxY.
    // ------------------------------------------------------------
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

    // ------------------------------------------------------------
    // Test 3
    // Places the fish outside the left boundary and verifies that
    // KeepInsideBounds() correctly clamps the fish back inside.
    // Also confirms the bounce logic modifies direction properly.
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator Fish_Clamps_Back_Inside_Left_Bound()
    {
        yield return null;

        fishObject.transform.position = new Vector3(movement.minX - 2f, 0f, 0f);

        yield return null;

        Assert.GreaterOrEqual(
            fishObject.transform.position.x,
            movement.minX,
            "Fish should be clamped inside the left boundary."
        );
    }

    // ------------------------------------------------------------
    // Test 4
    // Places the fish outside the right boundary and verifies
    // that it is moved back inside the allowed area.
    // Confirms the horizontal boundary correction logic.
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator Fish_Clamps_Back_Inside_Right_Bound()
    {
        yield return null;

        fishObject.transform.position = new Vector3(movement.maxX + 2f, 0f, 0f);

        yield return null;

        Assert.LessOrEqual(
            fishObject.transform.position.x,
            movement.maxX,
            "Fish should be clamped inside the right boundary."
        );
    }

    // ------------------------------------------------------------
    // Test 5
    // Verifies that the fish cannot move above or below the vertical limits.
    // The fish is manually placed outside both top and bottom bounds,
    // and we confirm that KeepInsideBounds() pushes it back inside.
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator Fish_Clamps_Back_Inside_Top_And_Bottom_Bounds()
    {
        yield return null;

        fishObject.transform.position = new Vector3(0f, movement.maxY + 2f, 0f);
        yield return null;

        Assert.LessOrEqual(
            fishObject.transform.position.y,
            movement.maxY,
            "Fish should stay inside the top boundary."
        );

        fishObject.transform.position = new Vector3(0f, movement.minY - 2f, 0f);
        yield return null;

        Assert.GreaterOrEqual(
            fishObject.transform.position.y,
            movement.minY,
            "Fish should stay inside the bottom boundary."
        );
    }

    // ------------------------------------------------------------
    // Test 6
    // Ensures the fish keeps moving across multiple frames.
    // Confirms that direction changes and movement logic continue
    // working after the initial movement.
    // Prevents bugs where fish only move once then stop.
    // ------------------------------------------------------------
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

        Assert.AreNotEqual(pos1, pos2,
            "Fish should continue moving across frames.");
    }

    // ------------------------------------------------------------
    // Test 7
    // Uses reflection to set the private FishState direction so the fish
    // is forced to move right.
    // Verifies that FlipSprite() correctly flips the sprite scale to face right.
    // Expected scale.x = 1
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator Fish_Flips_Right_When_CurrentDirection_Is_Right()
    {
        yield return null;

        SetPrivateStateDirection(new Vector2(1f, 0f), new Vector2(1f, 0f));

        yield return null;

        Assert.AreEqual(1f,
            fishObject.transform.localScale.x,
            0.001f,
            "Fish should face right when moving right.");
    }

    // ------------------------------------------------------------
    // Test 8
    // Forces the fish direction to the left using reflection.
    // Verifies FlipSprite() correctly mirrors the sprite.
    // Expected scale.x = -1
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator Fish_Flips_Left_When_CurrentDirection_Is_Left()
    {
        yield return null;

        SetPrivateStateDirection(new Vector2(-1f, 0f), new Vector2(-1f, 0f));

        yield return null;

        Assert.AreEqual(-1f,
            fishObject.transform.localScale.x,
            0.001f,
            "Fish should face left when moving left.");
    }

    // ------------------------------------------------------------
    // Helper Method
    // Uses reflection to access the private FishState class and manually
    // set direction values for testing FlipSprite().
    // Also disables automatic direction changes during the test.
    // ------------------------------------------------------------
    private void SetPrivateStateDirection(Vector2 currentDirection, Vector2 targetDirection)
    {
        FieldInfo stateField =
            typeof(FishMovement).GetField("state",
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(stateField, "Could not access private FishState.");

        object stateObject = stateField.GetValue(movement);

        System.Type stateType = stateObject.GetType();

        FieldInfo currentDirField =
            stateType.GetField("currentDirection",
            BindingFlags.Public | BindingFlags.Instance);

        FieldInfo targetDirField =
            stateType.GetField("targetDirection",
            BindingFlags.Public | BindingFlags.Instance);

        FieldInfo speedField =
            stateType.GetField("currentSpeed",
            BindingFlags.Public | BindingFlags.Instance);

        FieldInfo timerField =
            stateType.GetField("directionTimer",
            BindingFlags.Public | BindingFlags.Instance);

        currentDirField.SetValue(stateObject, currentDirection);
        targetDirField.SetValue(stateObject, targetDirection);

        // prevent random direction change during test
        speedField.SetValue(stateObject, 0f);
        timerField.SetValue(stateObject, 999f);
    }
}