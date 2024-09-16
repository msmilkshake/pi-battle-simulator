using UnityEngine;

public class ExponentialDistribution : IDistribution
{
    private float _scale;
    private float _location;
    private float _cdfA = 0;
    private float _cdfB = 1;
    private float _bMinusA;

    public ExponentialDistribution(float scale, float location,
        float left = float.NegativeInfinity, float right = float.PositiveInfinity)
    {
        _scale = scale;
        _location = location;

        if (!float.IsNegativeInfinity(left))
        {
            _cdfA = 1 - Mathf.Exp(-(left - location) / _scale);
            // Debug.Log(("A: " + _cdfA).Replace(",", "."));
        }

        if (!float.IsPositiveInfinity(right))
        {
            _cdfB = 1 - Mathf.Exp(-(right - location) / _scale);
            // Debug.Log(("B: " + _cdfB).Replace(",", "."));
        }

        _bMinusA = _cdfB - _cdfA;
    }

    public float Get()
    {
        float y = _cdfA + Random.value * _bMinusA;
        return _location - _scale * Mathf.Log(1 - y);
    }
}