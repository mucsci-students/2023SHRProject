using UnityEngine;

/// <summary>
/// Stores and initializes widely used variables. Resets static variables on game start or restart.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary> Stores the users current money </summary>
    public static int Money;
    /// <summary> Stores the users current lives </summary>
    public static int Lives;
    /// <summary> Stores the current wave number </summary>
    public static int WaveNumber = 0;
    /// <summary> Stores the number of enemies remaining in the current wave. 0 if wave is over </summary>
    public static int EnemiesRemaining = 0;
    /// <summary> Stores the total number of layers popped in the current game </summary>
    public static int LayersPopped = 0;

    /// <summary> Stores the starting money for the game </summary>
    [SerializeField]
    [Tooltip("Stores the starting money for the game")]
    private int StartingMoney = 200;
    
    /// <summary> Stores the starting lives for the game </summary>
    [SerializeField]
    [Tooltip("Stores the starting lives for the game")]
    private int StartingLives = 200;

    /// <summary>
    /// Unity method called on object creation.
    /// Sets the initial values for the class variables.
    /// </summary>
    private void Start()
    {
        ResetValues();
    }

    /// <summary>
    /// Resets the values of the class variables to their initial values.
    /// </summary>
    private void ResetValues()
    {
        Money = StartingMoney;
        Lives = StartingLives;
        WaveNumber = 0;
        EnemiesRemaining = 0;
        LayersPopped = 0;
    }
}
