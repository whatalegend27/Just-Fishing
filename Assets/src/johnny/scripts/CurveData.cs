using UnityEngine;

[CreateAssetMenu(fileName = "newAnimationCurve", menuName = "Data/Animation Curve")]
public class CurveData : ScriptableObject
{
    public AnimationCurve savedCurve = AnimationCurve.Linear(0, 0, 1, 1);
}