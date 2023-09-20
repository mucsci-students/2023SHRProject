using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InputManagerScript : MonoBehaviour
{

    [SerializeField] private bool isSpedUp = false;
    [SerializeField] private float speedUpFactor = 2.0f;
    private float previousSpeedUpFactor;
    
    // Update is called once per frame
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleSpeedUp();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Enemies Remaining: " + WaveManager.enemiesRemaining);
        }
    }

    public void ToggleSpeedUp()
    {
        Time.timeScale = isSpedUp ? 1.0f : speedUpFactor;

        isSpedUp = !isSpedUp;
    }

    public void TogglePause()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = previousSpeedUpFactor;
        }
        else
        {
            previousSpeedUpFactor = Time.timeScale;
            Time.timeScale = 0f;
        }
    }
    
    
}
