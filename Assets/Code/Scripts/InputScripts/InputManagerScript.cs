using UnityEngine;

public class InputManagerScript : MonoBehaviour
{

    [SerializeField] private bool isSpedUp;
    [SerializeField] private float speedUpFactor = 2.0f;
    private float _previousSpeedUpFactor = 1f;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject tutorialScreen;
    
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
            Time.timeScale = _previousSpeedUpFactor;
            pauseMenu.SetActive(false);
        }
        else
        {
            _previousSpeedUpFactor = Time.timeScale;
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }
    }
    
    public void ShowTutorial()
    {
        tutorialScreen.SetActive(true);
    }
    
    public void HideTutorial()
    {
        tutorialScreen.SetActive(false);
    }
}
