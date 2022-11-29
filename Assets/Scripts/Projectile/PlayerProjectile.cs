using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : Projectile
{
    TrailRenderer trail;

    void Awake() {
        //trail compenents are on children objects
        trail = GetComponentInChildren<TrailRenderer>();

        if(moveDirection != Vector2.right){
            transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector2.right, moveDirection);
        }
    }

    void OnDisable(){
        //when projectile is set inactive, delete its trail
        trail.Clear();
    }

    protected override void OnCollisionEnter2D(Collision2D collision){
        base.OnCollisionEnter2D(collision);

        //every time hit an enemy, add 1 energy
        PlayerEnergy.Instance.Obtain(PlayerEnergy.PERCENT);
    }
}
