using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Stores and initializes widely used variables. Resets static variables on game start or restart.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary> Stores the users current money </summary>
    public int money;
    /// <summary> Stores the users current lives </summary>
    public int lives;

    public int _prevWaveNumb = 2;
    
    /// <summary> Stores the number of enemies remaining in the current wave. 0 if wave is over </summary>
    public int enemiesRemaining;
    /// <summary> Stores the total number of layers popped in the current game </summary>
    public int layersPopped;

    /// <summary> Stores the starting money for the game </summary>
    [SerializeField]
    [Tooltip("Stores the starting money for the game")]
    private int startingMoney = 200;
    
    /// <summary> Stores the starting lives for the game </summary>
    [SerializeField]
    [Tooltip("Stores the starting lives for the game")]
    private int startingLives = 200;
    
    [SerializeField]
    private bool isAIEnabled;
    
    [Header("Object Links")]
    
    [SerializeField] private GameObject ai;

    [SerializeField] private WaveManager waveManager;
    
    [SerializeField] private GameObject loseCanvas;
    [SerializeField] private GameObject winCanvas;
    
    public delegate void MoneyUpdatedEvent();
    public static event MoneyUpdatedEvent OnMoneyUpdated;

    /// <summary>
    /// Unity method called on object creation.
    /// Sets the initial values for the class variables.
    /// </summary>
    private void Start()
    {
        ResetAll();
        if (isAIEnabled)
        {
            StartCoroutine(StartAI());
        }
    }

    public void ResetAll()
    {
        ResetValues();
    }

    /// <summary>
    /// Unity method called once per frame.
    /// </summary>
    private void Update()
    {
        if (!isAIEnabled)
        {
            ExitGameOnZeroLives();
            CheckWinGame();
        }
        
        if (waveManager.CurrentWaveNumber >= _prevWaveNumb)
        {
            ++_prevWaveNumb;
            money += 100 + waveManager.CurrentWaveNumber;
            OnMoneyUpdated?.Invoke();
        }
    }

    /// <summary>
    /// Checks if the users lives are 0 or less and exits the game if they are.
    /// Works in the unity editor and in the built game.
    /// </summary>
    private void ExitGameOnZeroLives()
    {
        if (lives <= 0)
        {
           // Deactivate other game elements?
           Time.timeScale = 0;
           loseCanvas.SetActive(true);
           
           //Application.Quit();
        }
    }
    
    private void CheckWinGame()
    {
        if (waveManager.isGameOver)
        {
            Time.timeScale = 0;
            winCanvas.SetActive(true);
        }
    }

    /// <summary>
    /// Resets the values of the class variables to their initial values.
    /// </summary>
    private void ResetValues()
    {
        money = startingMoney;
        lives = startingLives;
        _prevWaveNumb = 2;
        enemiesRemaining = 0;
        layersPopped = 0;
        Time.timeScale = 1;
    }

    /// <summary>
    /// Decrements the users lives by the count param.
    /// </summary>
    /// <param name="count"> The amount of lives to subtract from the users lives </param>
    public void SubtractLives(int count)
    {
        lives -= count;
    }
    
    private IEnumerator StartAI()
    {
        yield return new WaitForSeconds(1);
        EnableAI();
    }

    private void EnableAI()
    {
        ai.SetActive(true);
    }
    
    public int GetLives()
    {
        return lives;
    }

    // Cheats
    public void AddMoney(int amount)
    {
        money += amount;
    }
    
    public void AddLives(int amount)
    {
        lives += amount;
    }
    
    public void ToggleAutoplay(Toggle isAutoPlay)
    {
        waveManager.autoPlay = isAutoPlay.isOn;
    }
}
