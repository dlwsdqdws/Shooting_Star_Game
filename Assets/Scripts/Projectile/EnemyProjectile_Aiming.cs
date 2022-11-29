using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile_Aiming : Projectile
{
    void Awake(){
        target = GameObject.FindGameObjectWithTag("Player");
    }

    protected override void OnEnable(){
        StartCoroutine(nameof(MoveDirectionCoroutine));
        base.OnEnable();
    }

    IEnumerator MoveDirectionCoroutine(){
        //wait for one frame to get an accurate value
        yield return null;

        if(target.activeSelf){
            moveDirection = (target.transform.position - transform.position).normalized;
        }
    }
}
