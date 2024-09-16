using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Soldier _parentSoldier;

    private const float DestroyTimer = 3f;

    //TODO: balance destroy timer according to parent soldier level
    public Soldier ParentSoldier
    {
        get => _parentSoldier;
        set => _parentSoldier = value;
    }

    void OnCollisionEnter(Collision other)
    {
        // Debug.Log("TAG: Other: " + other.transform.tag +", Parent: " + _parentSoldier.transform.tag);

        Transform t = other.transform.parent;
        if (other.transform.CompareTag("Bullet") ||
            (t != null && _parentSoldier.CompareTag(t.tag)))
        {
            // Debug.Log("Bullet on Bullet!");
            // Debug.Log("TAG: Other: " + t.tag +", Parent: " + _parentSoldier.transform.tag);

            return;
        }
        //On collision with any collider, destroys bullet
        Destroy(gameObject);
    }

    public void Shoot(float modifier)
    {
        Destroy(gameObject, DestroyTimer + modifier);
    }

    void Start()
    {
        
    }
}
