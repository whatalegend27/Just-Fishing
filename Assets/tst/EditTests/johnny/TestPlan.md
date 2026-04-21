# Test Plan

**Author: Johnathan Van Vliet**

---

## Scope

This test plan will lay out the testing system for my features within the Just Fishing video game by Starfish Studios.

---

## Edit Tests:
Located in `Assets/src/tst/EditTests`

These ensure that the statistical implementations are valid and have enough allowed variance to be "fair".
- `DriftTests.cs` has 30 distinct test cases documented within the file.
- `StatisticalTests.cs` has 2 distinct test cases.

---

## Play Tests:
Located in `Assets/src/tst/PlayTests`

This ensures that the player speed is not high enough to break the bounding boxes preventing them from leaving the boat.

Test is located in `CollisionStressTests.cs` and has 1 distinct test case.