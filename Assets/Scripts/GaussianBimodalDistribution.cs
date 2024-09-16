using UnityEngine;

public class GaussianBimodalDistribution : IDistribution
{
    private static float _erfConst = 0.1400122886867f;

    private float _mu1;
    private float _sigma1;
    private float _mu2;
    private float _sigma2;
    private float _weight;

    public GaussianBimodalDistribution(float mu1, float sigma1, float mu2, float sigma2,
        float weight)
    {
        _mu1 = mu1;
        _sigma1 = sigma1;
        _mu2 = mu2;
        _sigma2 = sigma2;
        _weight = weight;
    }

    public float Get()
    {
        float x;
        do
        {
            x = Random.value;
        } while (x == _weight);

        float mu = x < _weight ? _mu1 : _mu2;
        float sigma = x < _weight ? _sigma1 : _sigma2;
        float shift = x < _weight ? 0 : _weight;
        float weight = x < _weight ? _weight : 1 - _weight;

        return sigma * Mathf.Sqrt(2) * ErfInv(2 * (x - shift) / weight - 1) + mu;
    }
    

    public float Test(float x)
    {
        float mu = x < _weight ? _mu1 : _mu2;
        float sigma = x < _weight ? _sigma1 : _sigma2;
        float shift = x < _weight ? 0 : _weight;
        float weight = x < _weight ? _weight : 1 - _weight;

        return sigma * Mathf.Sqrt(2) * ErfInv(2 * (x - shift) / weight - 1) + mu;
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