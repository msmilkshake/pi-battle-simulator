using UnityEngine;

public class MixedExpDistribution : IDistribution
{
    private float _v1;
    
    private ExponentialDistribution _exp;
    
    public MixedExpDistribution()
    {
        _v1 = 0.4f;
        _exp = new ExponentialDistribution(0.5f, 0.5f, 0.5f, 1.5f);
    }
    public float Get()
    {
        float u = Random.value;
        if (u < _v1)
        {
            return 0;
        }

        return _exp.Get();
    }
}