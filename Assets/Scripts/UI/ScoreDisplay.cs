using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    static Text scoreText;

    //initialize current function in Awake()
    void Awake(){
        scoreText = GetComponent<Text>();
    }

    //initialize other functions and call other functions in Start()
    void Start(){
        ScoreManager.Instance.ResetScore();
    }

    public static void UpdateText(int score) => scoreText.text = score.ToString();


    public static void ScaleText(Vector3 targetScale) => scoreText.rectTransform.localScale = targetScale;
}
