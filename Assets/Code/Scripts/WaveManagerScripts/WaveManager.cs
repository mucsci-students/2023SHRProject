using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

/// <summary>
/// Manages the spawning of bloons and waves.
/// Semi-randomly generates waves based on a list of RBEs.
/// </summary>
public class WaveManager : MonoBehaviour
{

    #region Vars

    [Header("Settings")]

    #region Settings

    [SerializeField]
    [Tooltip("The time between waves in seconds")]
    private float timeBetweenWaves = 15f;
    
    [SerializeField]
    private bool useRandomWaves;

    [SerializeField] 
    private int randomSeed = 123213;

    [Tooltip("The spawn point for the bloons")]
    public Transform spawn;

    [SerializeField] private bool autoPlay;

    [SerializeField] private List<int> RBES = new();

    [Tooltip("The path the bloons will follow. First point is the first point after spawn, last point is the end point.")]
    public List<Transform> path = new();

    [SerializeField] private List<BloonGroup> bloonGroups = new();
    private List<float> _bloonGroupIntervalsCopy;

    [Header("Bloon Prefabs")]
    [SerializeField] private GameObject RedBloonPrefab;
    [SerializeField] private GameObject BlueBloonPrefab;
    [SerializeField] private GameObject GreenBloonPrefab;
    [SerializeField] private GameObject YellowBloonPrefab;
    [SerializeField] private GameObject PinkBloonPrefab;
    [SerializeField] private GameObject BlackBloonPrefab;
    [SerializeField] private GameObject WhiteBloonPrefab;
    //[SerializeField] private GameObject LeadBloonPrefab;
    //[SerializeField] private GameObject ZebraBloonPrefab;
    //[SerializeField] private GameObject RainbowBloonPrefab;
    //[SerializeField] private GameObject CeramicBloonPrefab;
    //[SerializeField] private GameObject MOABPrefab;
    //[SerializeField] private GameObject BFBPrefab;
    //[SerializeField] private GameObject ZOMGPrefab;

    #endregion

    #region Debugging

    [Header("Debugging")]
    public List<GameObject> possibleEnemies = new();
    public int CurrentWaveNumber;
    public int enemiesRemaining;
    public float timerRef;

    #endregion

    [Header("Object Links")]
    [SerializeField] private BloonLookUpScript BLUS;
    [SerializeField] private GameManager gameManager;


    // Private variables

    /// <summary>
    /// Stores whether the game is currently playing. Is true if any wave has ever started. False if all waves are over.
    /// </summary>
    private bool isPlaying = false;
    
    [NonSerialized]
    public bool isGameOver = false;

    /// <summary>
    /// Stores whether the game is currently between rounds. Is true if isPlaying is true and bloon count less than 0.
    /// </summary>
    private bool betweenRounds = false;

    private float _timer = 0f;
    
    private Random _random = new Random();

    #endregion

    /// <summary>
    /// Unity method called once before the first frame.
    /// </summary>
    private void Start()
    {
        if (!useRandomWaves)
        {
            _random = new Random(randomSeed);
        }
        
        // Reset all static variables
        enemiesRemaining = 0;
        CurrentWaveNumber = 0;
        isGameOver = false;
        _bloonGroupIntervalsCopy = new List<float>();
        foreach (var bloonGroup in bloonGroups)
        {
            _bloonGroupIntervalsCopy.Add(bloonGroup.interval);
        }
    }

    public void ResetAll()
    {
        if (!useRandomWaves)
        {
            _random = new Random(randomSeed);
        }
        enemiesRemaining = 0;
        CurrentWaveNumber = 0;
        isGameOver = false;
        betweenRounds = false;
        _timer = 0f;
        possibleEnemies.Clear();
        foreach (var bloonGroup in bloonGroups)
        {
            bloonGroup.amountToSpawn = 0;
            bloonGroup.interval = _bloonGroupIntervalsCopy[bloonGroups.IndexOf(bloonGroup)];
        }
    }

    /// <summary>
    /// Called at the start of a wave.
    /// Updates the list of possible enemies and generates a random wave.
    /// </summary>
    public void StartWave()
    {
        isPlaying = true;
        ++CurrentWaveNumber;

        UpdatePossibleEnemies();
        GenerateSemiRandomWave();
    }

    /// <summary>
    /// Unity method called once per frame.
    /// </summary>
    private void Update()
    {
        // Do not run if the number of waves exceeds the number of RBEs
        if (CurrentWaveNumber == RBES.Count + 1 && enemiesRemaining <= 0)
            return;

        // Start the wave manager if the user presses space and the game is not already playing
        if (Input.GetKeyDown(KeyCode.Space) && !isPlaying && CurrentWaveNumber == 0)
            StartWave();

        if (!isPlaying)
            return;

        // Run once when wave is first over
        if (!betweenRounds && !RunCurrentWave() && transform.childCount == 0)
        {
            Debug.Log("Wave Ended");
            //give round bonus & update round number
            //GameManager.UpdateWaveNumber();
            if (CurrentWaveNumber > RBES.Count - 1)
            {
                Debug.Log("Waves over");
                isPlaying = false;
                isGameOver = true;
            }
            else
            {
                betweenRounds = true;
            }
        }

        // Run every frame when between rounds
        if (transform.childCount == 0 && betweenRounds)
        {
            enemiesRemaining = 0;
            _timer += Time.unscaledDeltaTime;
            timerRef = _timer; // used for ui
            if (_timer > timeBetweenWaves || Input.GetKeyDown(KeyCode.Space) || autoPlay)
            {
                StartWave();
                _timer = 0;
                betweenRounds = false;
            }
        }
    }

    /// <summary>
    /// Update the list of enemies to spawn based on a wave number.
    /// Prevents wave one from spawning moabs.
    /// </summary>
    private void UpdatePossibleEnemies()
    {
        if (CurrentWaveNumber == 1)
        {
            possibleEnemies.Add(RedBloonPrefab);
        }
        else if (CurrentWaveNumber == 2)
        {
            possibleEnemies.Add(BlueBloonPrefab);
        }
        else if (CurrentWaveNumber == 6)
        {
            possibleEnemies.Add(GreenBloonPrefab);
        }
        else if (CurrentWaveNumber == 11)
        {
            possibleEnemies.Add(YellowBloonPrefab);
        }
        else if (CurrentWaveNumber == 15)
        {
            possibleEnemies.Add(PinkBloonPrefab);
        }
        else if (CurrentWaveNumber == 23)
        {
            possibleEnemies.Add(WhiteBloonPrefab);
        }
        else if (CurrentWaveNumber == 23)
        {
            possibleEnemies.Add(BlackBloonPrefab);
        }
    }

    /// <summary>
    /// Runs the current wave, checks if any bloon groups are still spawning bloons.
    /// </summary>
    /// <returns>True if wave is still running, otherwise false</returns>
    private bool RunCurrentWave()
    {
        bool flag = false;

        foreach (var bloonGroup in bloonGroups)
        {
            bloonGroup.SpawnBloon(path, spawn, BLUS, gameObject.transform, gameManager, this);
            // If any bloon group is not finished, set flag to true meaning that at least one bloon group is still spawning
            if (!bloonGroup.isFinished())
                flag = true;
        }

        return flag;
    }

    /// <summary>
    /// Updates the bloon groups with a random number of bloons to spawn.
    /// </summary>
    private void GenerateSemiRandomWave()
    {
        int RBE = RBES[CurrentWaveNumber - 1];

        while (RBE > 0)
        {
            GameObject bloonToSpawn;

            // Hardcode certain RBEs to spawn certain bloons to prevent infinite loops
            bloonToSpawn = RBE == 1 ? RedBloonPrefab : GetRandomBloon();

            // Do not go over RBE, choose new bloon if this happens
            if (bloonToSpawn.GetComponent<BloonScript>().GetHealth() > RBE) continue;

            // Find relevant bloon group based on bloon to spawn and add it to that bloon group.
            foreach (var bloonGroup in bloonGroups)
            {
                if (bloonGroup.Bloon == bloonToSpawn)
                {
                    bloonGroup.amountToSpawn++;
                    enemiesRemaining++;
                    RBE -= bloonToSpawn.GetComponent<BloonScript>().GetHealth();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Returns a random bloon from the list of possible enemies.
    /// </summary>
    /// <returns> A random bloon from the list of possible enemies. </returns>
    private GameObject GetRandomBloon()
    {
        // Generate me a random int
        int randomNum = _random.Next(0, possibleEnemies.Count);
        return possibleEnemies[randomNum];
    }

    public void DestroyAllBloons()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    [Serializable]
    public class BloonGroup
    {

        public GameObject Bloon;

        [SerializeField]
        [Tooltip("The number of bloons to spawn")]
        public int amountToSpawn;

        [SerializeField]
        [Tooltip("Number of seconds between bloon spawns")]
        public float interval;

        [SerializeField]
        [Tooltip("Initial interval between bloon spawns at the beginning of the game.")]
        public float initialInterval;

        [SerializeField]
        [Tooltip("The time before the first bloon spawns. If set to 0, the first bloon will spawn instantly.")]
        [Min(0.0f)]
        private float countdownToFirstBloonSpawn;

        private float lastTime;
        private float startTime;
        private bool spawnInstant = true;

        public void Start()
        {
            lastTime = Time.time;
            startTime = Time.time;
            if (countdownToFirstBloonSpawn == 0.0f)
                spawnInstant = false;
        }

        /// <summary>
        /// Spawns a bloon if it has been at least interval seconds since the last bloon was spawned.
        /// Does not spawn bloons if amountToSpawn is 0.
        /// </summary>
        /// <param name="path">The path the bloon will follow. </param>
        /// <param name="spawn">The spawn point of the bloon.</param>
        /// <param name="BLUS">The BloonLookUpScript to pass to the bloon.</param>
        /// <param name="parent">The parent transform to attach each bloon to</param>
        /// <param name="gameManager">The gameManager reference</param>
        /// <param name="waveManager">The waveManager reference</param>
        public void SpawnBloon(List<Transform> path, Transform spawn, BloonLookUpScript BLUS, Transform parent, GameManager gameManager, WaveManager waveManager)
        {
            if (Time.time - startTime < countdownToFirstBloonSpawn || amountToSpawn <= 0)
                return;

            if (spawnInstant || Time.time - lastTime >= interval)
            {
                GameObject bloon = Instantiate(Bloon, parent);
                bloon.transform.SetPositionAndRotation(spawn.position, spawn.rotation);

                var bloonScript = bloon.GetComponent<BloonScript>();
                bloonScript.SetBloonLookUpScript(BLUS);
                bloonScript.SetGameManager(gameManager);
                bloonScript.SetWaveManager(waveManager);
                var pathFollowingScript = bloon.GetComponent<PathFollowingScript>();
                pathFollowingScript.SetBloonPath(path);
                pathFollowingScript.SetGameManager(gameManager);
                pathFollowingScript.SetWaveManager(waveManager);

                --amountToSpawn;
                lastTime = Time.time;
                spawnInstant = false;

            }
            
            if (amountToSpawn == 0)
                UpdateSpawnIntervals();
        }

        private void UpdateSpawnIntervals()
        {
            if (interval <= 0.1f)
            {
                if (interval <= 0.01f)
                {
                    return;
                }
                initialInterval = 0.01f;
                interval -= initialInterval;

                return;
            }
            initialInterval = 0.025f;
            interval -= initialInterval;
        }

        // Setters and getters

        /// <summary>
        /// Checks if the BloonGroup is finished spawning bloons.
        /// </summary>
        /// <returns>True if the BloonGroup is finished spawning bloons, otherwise false</returns>
        public bool isFinished()
        {
            return amountToSpawn == 0;
        }

        /// <summary>
        /// Returns the total number of bloons left to bloons spawn.
        /// </summary>
        /// <returns>The total number of bloons left to bloons spawn.</returns>
        public int GetTotalBloonCount()
        {
            return amountToSpawn;
        }
    }
}
