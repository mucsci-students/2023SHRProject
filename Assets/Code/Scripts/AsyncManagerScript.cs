using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AsyncManagerScript : MonoBehaviour
{
    [Header("Menu Screens")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;
    
    [Header("Slider")]
    [SerializeField] private Slider loadingSlider;

    public void LoadLevelBtn(string levelToLoad)
    {
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);
        StartCoroutine(LoadLevelAsync(levelToLoad));
    }
    IEnumerator LoadLevelAsync(string levelToLoad) //Coroutine function - code can run async over multiple frames
    {
        //loads specified level asynchronously based on name and stores it in a async operation that we can use later
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        while (!loadOperation.isDone)
        {
            //inside here we are basically updating slider bar based on the progress of the loading operation.
            //and clamping will ensure that the progress value stays between a specified range
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            
            //just updating slider bar 
            loadingSlider.value = progressValue;

            //allows unity to update frame and continue loop, also makes sure game doesnt freeze during a load 
            yield return null;
        }
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
