using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : Singleton<PlayerEnergy>
{

    [SerializeField] EnergyBar energyBar;

    [SerializeField] float overdrivingInterval = 0.1f;

    bool available = true;

    public const int MAX = 100;

    public const int PERCENT = 1;

    int energy;

    WaitForSeconds waitForOverdriveInterval;

    protected override void Awake(){
        base.Awake();

        waitForOverdriveInterval = new WaitForSeconds(overdrivingInterval);
    }

    void OnEnable(){
        PlayerOverdrive.on += PlayerOverdriveOn;
        PlayerOverdrive.off += PlayerOverdriveOff;
    }

    void OnDisable(){
        PlayerOverdrive.on -= PlayerOverdriveOn;
        PlayerOverdrive.off -= PlayerOverdriveOff;
    }

    void Start(){
        energyBar.Initialize(energy, MAX);
        Obtain(MAX);
    }

    public void Obtain(int value){
        if(energy == MAX || !available || !gameObject.activeSelf) return ;

        energy += value;
        energy = Mathf.Clamp(energy, 0, MAX);

        energyBar.UpdateStats(energy, MAX);
    }

    public void Use(int value){
        energy -= value;
        energyBar.UpdateStats(energy, MAX);

        if(energy == 0 && !available){
            PlayerOverdrive.off.Invoke();
        }

        // Debug.LogWarning(energy);
    }

    public bool IsEnough(int value) => energy >= value;

    void PlayerOverdriveOn(){
        // Debug.LogWarning("Overdrive on function works well");

        available = false;
        StartCoroutine(nameof(KeepUsingCoroutine));
    }

    void PlayerOverdriveOff(){
        available = true;
        StopCoroutine(nameof(KeepUsingCoroutine));
    }

    IEnumerator KeepUsingCoroutine(){
        while(gameObject.activeSelf && energy > 0){
            // Debug.LogWarning("Coroutine starts well");
            yield return waitForOverdriveInterval;

            //every 0.1s, use 1% max energy
            Use(PERCENT);
        }
    }

}
