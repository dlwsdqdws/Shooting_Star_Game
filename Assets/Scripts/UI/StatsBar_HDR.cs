using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar_HDR : StatsBar
{
    [SerializeField] Text percentText;

    void SetPercentText(){
        percentText.text = targetFillAmount.ToString("P0");
    }

    public override void Initialize(float currentValue, float maxValue){
        base.Initialize(currentValue, maxValue);

        SetPercentText();
    }

    protected override IEnumerator BufferedFillingCoroutine(Image image){
        SetPercentText();
        return base.BufferedFillingCoroutine(image);
    }
}
