using UnityEngine;

public class GaussianDistribution : IDistribution
{
    private static float _erfConst = 0.1400122886867f;

    private float _mu;
    private float _sigma;
    private float _cdfA = 0;
    private float _cdfB = 1;
    private float _bMinusA;

    public GaussianDistribution()
    {
    }

    public GaussianDistribution(float mu, float sigma,
        float left = float.NegativeInfinity, float right = float.PositiveInfinity)
    {
        _mu = mu;
        _sigma = sigma;

        if (!float.IsNegativeInfinity(left))
        {
            _cdfA = 0.5f * (1 + Erf((left - _mu) / (_sigma * Mathf.Sqrt(2))));
            // Debug.Log(_cdfA);
        }

        if (!float.IsPositiveInfinity(right))
        {
            _cdfB = 0.5f * (1 + Erf((right - _mu) / (_sigma * Mathf.Sqrt(2))));
            // Debug.Log(_cdfB);
        }

        _bMinusA = _cdfB - _cdfA;
    }

    public float Get()
    {
        float y = _cdfA + Random.value * _bMinusA;

        return _sigma * Mathf.Sqrt(2) * ErfInv(2 * y - 1) + _mu;
    }

    public float Get(float mu, float sigma)
    {
        return sigma * Mathf.Sqrt(2) * ErfInv(2 * Random.value - 1) + mu;
    }
    
    public float Get(float mu, float sigma, float left, float right)
    {
        _mu = mu;
        _sigma = sigma;
        _cdfA = 0.5f * (1 + Erf((left - _mu) / (_sigma * Mathf.Sqrt(2))));
        _cdfB = 0.5f * (1 + Erf((right - _mu) / (_sigma * Mathf.Sqrt(2))));
        _bMinusA = _cdfB - _cdfA;
        
        float y = _cdfA + Random.value * _bMinusA;

        return _sigma * Mathf.Sqrt(2) * ErfInv(2 * y - 1) + _mu;
    }

    private static float Erf(float x)
    {
        return Mathf.Sign(x) * Mathf.Sqrt(1 - Mathf.Exp(-(x * x)
            * (4 / Mathf.PI + _erfConst * x * x) / (1 + _erfConst * x * x)));
    }

    private static float ErfInv(float x)
    {
        float ln = LnFun(x);
        float b = ln / 2;
        float term1 = 2 / (Mathf.PI * _erfConst) + b;
        float term2 = Mathf.Pow(term1, 2);
        float term3 = ln / _erfConst;

        return Mathf.Sign(x) * Mathf.Sqrt(Mathf.Sqrt(term2 - term3) - term1);
    }

    private static float LnFun(float x)
    {
        return Mathf.Log(1 - x * x);
    }
}