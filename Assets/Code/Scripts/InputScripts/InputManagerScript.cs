using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InputManagerScript : MonoBehaviour
{

    [SerializeField] private bool isSpedUp = false;
    [SerializeField] private float speedUpFactor = 2.0f;
    private float previousSpeedUpFactor = 1f;

    [SerializeField] private GameObject pauseMenu;
    
    // Update is called once per frame
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleSpeedUp();
        }
    }

    public void ToggleSpeedUp()
    {
        Time.timeScale = isSpedUp ? 1.0f : speedUpFactor;

        isSpedUp = !isSpedUp;
    }
    
    //should be re-named to toggleSettings, opening settings pauses the game
    public void TogglePause()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = previousSpeedUpFactor;
            pauseMenu.SetActive(false);
        }
        else
        {
            previousSpeedUpFactor = Time.timeScale;
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }
    }
    
    
}
