using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar : MonoBehaviour
{

    [SerializeField] Image fillImageBack;

    [SerializeField] Image fillImageFront;

    [SerializeField] float fillSpeed = 0.1f;

    [SerializeField] bool delayFill = true;
    [SerializeField] float fillDelay = 0.5f;

    float currentFillAmount;

    float previousFillAmount;

    protected float targetFillAmount;

    float t;

    WaitForSeconds waitForDelayFill;

    Canvas canvas;

    Coroutine buffererdFillingCoroutine;

    void Awake(){
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        waitForDelayFill = new WaitForSeconds(fillDelay);
    }

    void OnDisable(){
        StopAllCoroutines();
    }

    public virtual void Initialize(float currentValue, float maxValue){
        currentFillAmount = currentValue / maxValue;
        targetFillAmount = currentFillAmount;

        fillImageBack.fillAmount = currentFillAmount;
        fillImageFront.fillAmount = currentFillAmount;
    }

    public void UpdateStats(float currentValue, float maxValue){
        targetFillAmount = currentValue / maxValue;

        if(buffererdFillingCoroutine != null){
            StopCoroutine(buffererdFillingCoroutine);
        }

        //if stats reduces -> 
        if(currentFillAmount > targetFillAmount){
            //fill image front = target fill amount
            fillImageFront.fillAmount = targetFillAmount;
            //fill image back slowly reduce
            buffererdFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageBack));
        }

        // if status increase->
        else if(currentFillAmount < targetFillAmount){
            //fill image back = target fill amount
            fillImageBack.fillAmount = targetFillAmount;
            //fill image from slowly increase
            buffererdFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageFront));
        }
    }

    protected virtual IEnumerator BufferedFillingCoroutine(Image image){

        if(delayFill){
            yield return waitForDelayFill;
        }

        t = 0;

        previousFillAmount = currentFillAmount;

        while(t < 1f){
            t += Time.deltaTime * fillSpeed;
            currentFillAmount = Mathf.Lerp(previousFillAmount, targetFillAmount, t);
            image.fillAmount = currentFillAmount;

            yield return null;
        }
        
    }
}
