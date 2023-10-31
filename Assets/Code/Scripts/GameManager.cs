using System;
using System.Collections;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Stores and initializes widely used variables. Resets static variables on game start or restart.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary> Stores the users current money </summary>
    public int Money;
    /// <summary> Stores the users current lives </summary>
    public int Lives;

    private int prevWaveNumb = 2;
    
    /// <summary> Stores the number of enemies remaining in the current wave. 0 if wave is over </summary>
    public int EnemiesRemaining = 0;
    /// <summary> Stores the total number of layers popped in the current game </summary>
    public int LayersPopped = 0;

    /// <summary> Stores the starting money for the game </summary>
    [SerializeField]
    [Tooltip("Stores the starting money for the game")]
    private int StartingMoney = 200;
    
    /// <summary> Stores the starting lives for the game </summary>
    [SerializeField]
    [Tooltip("Stores the starting lives for the game")]
    private int StartingLives = 200;
    
    [SerializeField]
    private bool isAIEnabled = false;
    
    [Header("Object Links")]
    
    [SerializeField] private GameObject AI;

    [SerializeField] private GenerateMapScript generateMapScript;
    
    [SerializeField] private WaveManager waveManager;

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
        }
        
        if (waveManager.CurrentWaveNumber == prevWaveNumb)
        {
            ++prevWaveNumb;
            Money += 100 + waveManager.CurrentWaveNumber;
        }
    }

    /// <summary>
    /// Checks if the users lives are 0 or less and exits the game if they are.
    /// Works in the unity editor and in the built game.
    /// </summary>
    private void ExitGameOnZeroLives()
    {
        if (Lives <= 0)
        {
            Debug.Log("Game Over");
            
            // TODO: Add game over screen
            
            //Exit unity editor
            #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
            #endif
            //Exit game
            Application.Quit();
        }
    }

    /// <summary>
    /// Resets the values of the class variables to their initial values.
    /// </summary>
    private void ResetValues()
    {
        Money = StartingMoney;
        Lives = StartingLives;
        EnemiesRemaining = 0;
        LayersPopped = 0;
    }

    /// <summary>
    /// Decrements the users lives by the count param.
    /// </summary>
    /// <param name="count"> The amount of lives to subtract from the users lives </param>
    public void SubtractLives(int count)
    {
        Lives -= count;
    }
    
    private IEnumerator StartAI()
    {
        yield return new WaitForSeconds(1);
        EnableAI();
    }

    private void EnableAI()
    {
        AI.SetActive(true);
    }
    
    public int GetLives()
    {
        return Lives;
    }

    // Cheats
    public void AddMoney(int amount)
    {
        Money += amount;
    }
    
    public void AddLives(int amount)
    {
        Lives += amount;
    }
}
