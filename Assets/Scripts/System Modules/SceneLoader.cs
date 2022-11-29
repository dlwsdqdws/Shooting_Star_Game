using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : PersistentSingleton<SceneLoader>
{

    [SerializeField] UnityEngine.UI.Image transitionImage;

    [SerializeField] float fadeTime = 3.5f;

    Color color;

    const string GAMEPLAY = "Gameplay";

    void Load(string sceneName){
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator LoadCoroutine(string sceneName){

        //load new scene in the background
        var loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        //set scene inactive
        loadingOperation.allowSceneActivation = false;

        //fade out
        transitionImage.gameObject.SetActive(true);

        while(color.a < 1){
            color.a = Mathf.Clamp01(color.a + Time.unscaledDeltaTime / fadeTime);

            transitionImage.color = color;

            yield return null;
        }

        //load new scene
        // Load(sceneName);
        loadingOperation.allowSceneActivation = true;

        //fade in
        while(color.a > 0){
            color.a = Mathf.Clamp01(color.a - Time.unscaledDeltaTime / fadeTime);

            transitionImage.color = color;

            yield return null;
        }

        transitionImage.gameObject.SetActive(false);
    }

    public void LoadGamePlayScene(){
        StartCoroutine(LoadCoroutine(GAMEPLAY));
    }
}
