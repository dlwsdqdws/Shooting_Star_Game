using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerOverdrive : MonoBehaviour
{
    public static UnityAction on = delegate {};
    
    public static UnityAction off = delegate {};

    [SerializeField] GameObject triggerVFX;

    [SerializeField] GameObject engineVFXNormal;

    [SerializeField] GameObject engineVFXOverride;

    [SerializeField] AudioData onSFX;

    [SerializeField] AudioData offSFX;

    void Awake(){
        on += On;
        off += Off;
    }

    void OnDestroy() {
        on -= On;
        off -= Off;
    }

    void On(){
        triggerVFX.SetActive(true);
        engineVFXNormal.SetActive(false);
        engineVFXOverride.SetActive(true);
        AudioManager.Instance.PlayRandomSFX(onSFX);
    }

    void Off(){
        engineVFXOverride.SetActive(false);
        engineVFXNormal.SetActive(true);
        AudioManager.Instance.PlayRandomSFX(offSFX);
    }
}
