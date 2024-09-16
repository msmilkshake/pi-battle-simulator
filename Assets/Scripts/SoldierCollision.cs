using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierCollision : MonoBehaviour
{
    //Collision events
    public Soldier soldier;
    private void OnCollisionEnter(Collision other)
    {
        //If collider is not a bullet, exits
        if (!other.gameObject.CompareTag("Bullet"))
        {
            // Debug.Log("Not a bullet");
            return;
        }
        
        Bullet bullet = other.transform.GetComponent<Bullet>();
        if (bullet.ParentSoldier.CompareTag(soldier.tag))
        {
            return;
        }

        // Debug.Log(name + " got hit by a bullet");
        soldier.BulletHit();
        
    }
}
