using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] GameObject deathVFX;

    [Header("---- HEALTH ----")]

    [SerializeField] AudioData[] deathSFX; 

    [SerializeField] protected float maxHealth;

    [SerializeField] StatsBar onHeadHealthBar;

    [SerializeField] bool showOnHeadHealthBar = true;

    protected float health;

    //protected : child class can access this function
    //virtual : child class can rewrite this function
    protected virtual void OnEnable() {
        health = maxHealth;

        if(showOnHeadHealthBar){
            ShowOnHeadHealthBar();
        }else{
            HideOnHeadHealthBar();
        }
    }

    public void ShowOnHeadHealthBar(){
        onHeadHealthBar.gameObject.SetActive(true);
        onHeadHealthBar.Initialize(health, maxHealth);
    }

    public void HideOnHeadHealthBar(){
        onHeadHealthBar.gameObject.SetActive(false);
    }

    public virtual void TakeDamage(float damage){
        health -= damage;

        if(showOnHeadHealthBar && gameObject.activeSelf){
            onHeadHealthBar.UpdateStats(health, maxHealth);
        }

        if(health <= 0f){
            Die();
        }
    }

    public virtual void Die(){
        health = 0f;
        AudioManager.Instance.PlayRandomSFX(deathSFX);
        PoolManager.Release(deathVFX, transform.position);
        gameObject.SetActive(false);
    }

    public virtual void RestoreHealth(float value){
        if(health == maxHealth) return ;

        health = Mathf.Clamp(health+value, 0f, maxHealth);

        if(showOnHeadHealthBar){
            onHeadHealthBar.UpdateStats(health, maxHealth);
        }
    }

    protected IEnumerator HealthRegenerateCoroutine(WaitForSeconds waitTime, float percent){
        while(health < maxHealth){
            yield return waitTime;

            RestoreHealth(maxHealth * percent);
        }
    }

    protected IEnumerator DamageOverTimeCoroutine(WaitForSeconds waitTime, float percent){
        while(health > 0f){
            yield return waitTime;

            TakeDamage(maxHealth * percent);
        }
    }
}
