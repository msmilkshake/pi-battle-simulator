using UnityEngine;

public class WeibullDistribution : IDistribution
{
    private float _a;
    private float _b;
    private float _c;

    public WeibullDistribution(float a, float b, float c)
    {
        if (b <= 0)
        {
            throw new UnityException("b must be > 0");
        }
        if (c <= 0)
        {
            throw new UnityException("c must be > 0");
        }
        _a = a;
        _b = b;
        _c = c;
    }

    public float Get()
    {
        return _a + _b * Mathf.Pow(-Mathf.Log(1 - Random.value), 1 / _c);
    }
}