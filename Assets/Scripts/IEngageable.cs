using UnityEngine;

public interface IEngageable
{
    Transform ObjectTransform { get; }
    bool IsDead();
}
