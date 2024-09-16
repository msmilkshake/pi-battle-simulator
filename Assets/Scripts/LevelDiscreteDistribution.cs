using UnityEngine;

public class LevelDiscreteDistribution : IDistribution
{
    private static float NormalizeConst =
        1f + 9f / 13f + 6f / 13f + 4f / 13f + 3f / 13f;
    private static float _v1 = 1f / NormalizeConst;
    private static float _v2 = _v1 + 9f / 13f / NormalizeConst;
    private static float _v3 = _v2 + 6f / 13f / NormalizeConst;
    private static float _v4 = _v3 + 4f / 13f / NormalizeConst;

    public LevelDiscreteDistribution()
    {
    }
    public float Get()
    {
        float u = Random.value;
        // Debug.Log(u);

        if (u < _v1)
        {
            return 1;
        }

        if (u < _v2)
        {
            return 2;
        }

        if (u < _v3)
        {
            return 3;
        }

        if (u < _v4)
        {
            return 4;
        }

        return 5;
    }
}