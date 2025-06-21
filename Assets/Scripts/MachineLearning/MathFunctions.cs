using UnityEngine;

public static class MathFunctions
{
    public static float Tanh(float x)
    {
        return (Mathf.Exp(2 * x) - 1) / (Mathf.Exp(2 * x) + 1);
    }
}
