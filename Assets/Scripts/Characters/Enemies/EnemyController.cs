using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    [Header("---- MOVE ----")]
    [SerializeField] float paddingX;
    [SerializeField] float paddingY;

    [SerializeField] float moveSpeed = 2f;

    [SerializeField] float moveRotationAngle = 25f;


    [Header("---- FIRE ----")]
    [SerializeField] float minFireInterval;

    [SerializeField] float maxFireInterval;

    [SerializeField] GameObject[] projectiles;

    [SerializeField] AudioData[] projectileLaunchSFX;

    [SerializeField] Transform muzzle;

    float maxMoveDistancePerFrame;

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    void Awake(){
        maxMoveDistancePerFrame = moveSpeed * Time.fixedDeltaTime;
    }


    //enemies are managered by pool system
    void OnEnable() {
        StartCoroutine(nameof(RandomlyMovingCoroutine));
        StartCoroutine(nameof(RandomlyFireCoroutine));
    }

    void OnDisable() {
        StopAllCoroutines();
    }

    IEnumerator RandomlyMovingCoroutine(){
        //generate an enemy
        transform.position = Viewport.Instance.RandomEnemySpawnPosition(paddingX, paddingY);

        Vector3 targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);

        while(gameObject.activeSelf){
            //if enemy has not arrived target position -> keep moving to target position
            if(Vector3.Distance(transform.position, targetPosition) >= maxMoveDistancePerFrame){
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, maxMoveDistancePerFrame);
                //make enemy rotate with x-axis while moving
                transform.rotation = Quaternion.AngleAxis((targetPosition - transform.position).normalized.y * moveRotationAngle, Vector3.right);
            }
            //else -> set a new target position
            else{
                targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);
            }

            yield return waitForFixedUpdate;
        }
    }

    IEnumerator RandomlyFireCoroutine(){
        while(gameObject.activeSelf){
            yield return new WaitForSeconds(Random.Range(minFireInterval, maxFireInterval));

            foreach (var projectile in projectiles)
            {
                PoolManager.Release(projectile, muzzle.position);
            }

            AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);
        }
    }
}
