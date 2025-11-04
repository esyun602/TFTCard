using UnityEngine;

public static class AnimationCurveExtensions
{
    public static float GetTangentAt(this AnimationCurve curve, float t)
    {
        var h = 0.01f;
        return (curve.Evaluate(t + h) - curve.Evaluate(t)) / h;
    }
}