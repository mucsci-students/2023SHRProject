using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    [Header("Settings")]

    [SerializeField] [Tooltip("The time between waves in seconds")]
    private float timeBetweenWaves = 15f;
    
    [Tooltip("The spawn point for the bloons")]
    public Transform spawn;
    
    private bool autoPlay = false;
    
    [SerializeField] private List<int> RBES = new();

    [Tooltip("The path the bloons will follow. First point is the first point after spawn, last point is the end point.")]
    public List<Transform> path = new();

    [SerializeField] private List<BloonGroup> bloonGroups = new();

    [Header("Bloon Prefabs")]
    public GameObject RedBloonPrefab;
    public GameObject BlueBloonPrefab;
    public GameObject GreenBloonPrefab;
    public GameObject YellowBloonPrefab;
    public GameObject PinkBloonPrefab;
    
    [Header("Debugging")]
    public List<GameObject> possible_enemies = new();
    public int CurrentWaveNumber = 0;
    public float timerRef;
    public int EnemiesRemaining;
    
    [Header("Object Links")]
    [SerializeField] private BloonLookUpScript BLUS;

    //public GameObject BlackBloonPrefab;
    //public GameObject WhiteBloonPrefab;
    //public GameObject LeadBloonPrefab;
    //public GameObject ZebraBloonPrefab;
    //public GameObject RainbowBloonPrefab;
    //public GameObject CeramicBloonPrefab;
    //public GameObject MOABPrefab;
    //public GameObject BFBPrefab;
    //public GameObject ZOMGPrefab;


    // Private variables

    /// <summary>
    /// Stores whether the game is currently playing. Is true if any wave has ever started. False if all waves are over.
    /// </summary>
    private bool isPlaying = false;
    
    public static int enemiesRemaining = 0;

    /// <summary>
    /// Stores whether the game is currently between rounds. Is true if isPlaying is true and bloon count < 0.
    /// </summary>
    private bool betweenRounds = false;

    private float timer = 0f;

    private void StartWave()
    {
        isPlaying = true;
        ++CurrentWaveNumber;
        // TODO: Update bloon groups to spawn at faster intervals as the game progresses
        UpdatePossibleEnemies();
        GenerateWave();
    }

    private void Update()
    {
        EnemiesRemaining = enemiesRemaining;

        if (Input.GetKeyDown(KeyCode.Space) && !isPlaying)
            StartWave();

        if (!isPlaying)
            return;

        // If wave is over
        if (!betweenRounds && !RunCurrentWave() && enemiesRemaining == 0)
        {
            Debug.Log("Wave Ended");
            if (CurrentWaveNumber > RBES.Count - 1)
            {
                Debug.Log("Waves over");
                isPlaying = false;
            }
            else
            {
                betweenRounds = true;
            }
        }

        if (enemiesRemaining <= 0)
        {
            enemiesRemaining = 0;
            timer += Time.unscaledDeltaTime;
            timerRef = timer; // used for ui
            if (timer > timeBetweenWaves || Input.GetKeyDown(KeyCode.Space))
            {
                StartWave();
                timer = 0;
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
            possible_enemies.Add(RedBloonPrefab);
        }
        else if (CurrentWaveNumber == 2)
        {
            possible_enemies.Add(BlueBloonPrefab);
        }
        else if (CurrentWaveNumber == 3)
        {
            possible_enemies.Add(GreenBloonPrefab);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>True if wave is still running, otherwise false</returns>
    private bool RunCurrentWave()
    {
        bool flag = false;

        // If any bloon group is not finished, set flag to true meaning that at least one bloon group is still spawning
        foreach (var bloonGroup in bloonGroups)
        {
            bloonGroup.ReadyToSpawn(path, spawn, BLUS);
            if (!bloonGroup.isFinished())
                flag = true;
        }

        return flag;
    }

    private void GenerateWave()
    {
        int RBE = RBES[CurrentWaveNumber - 1];

        while (RBE > 0)
        {
            GameObject bloonToSpawn;
            
            // Hardcode certain RBEs to spawn certain bloons to prevent infinite loops
            if (RBE == 1)
            {
                bloonToSpawn = RedBloonPrefab;
            }
            else
            {
                bloonToSpawn = GetRandomBloon();
            }
            
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

    private GameObject GetRandomBloon()
    {
        // Generate me a random int
        int randomNum = Random.Range(0, possible_enemies.Count);
        return possible_enemies[randomNum];
    }

    [System.Serializable]
    public class BloonGroup
    {

        public GameObject Bloon;

        [SerializeField]
        [Tooltip("The number of bloons to spawn")]
        public int amountToSpawn;

        [SerializeField]
        [Tooltip("Number of seconds between bloon spawns")]
        private float interval;

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

        public void ReadyToSpawn(List<Transform> path, Transform spawn, BloonLookUpScript BLUS)
        {
            if (Time.time - startTime < countdownToFirstBloonSpawn || amountToSpawn <= 0)
                return;

            if (spawnInstant || Time.time - lastTime >= interval)
            {
                GameObject bloon = Instantiate(Bloon);
                bloon.transform.position = spawn.position;
                bloon.transform.rotation = spawn.rotation;

                bloon.GetComponent<BloonScript>().SetBloonLookUpScript(BLUS);
                bloon.GetComponent<PathFollowingScript>().SetBloonPath(path);

                --amountToSpawn;
                lastTime = Time.time;
                spawnInstant = false;
            }
        }

        // Setters and getters

        public int GetTotalBloonCount()
        {
            return amountToSpawn;
        }

        /// <summary>
        /// Checks if the spawnInfo is finished spawning bloons.
        /// </summary>
        /// <returns>True if the spawnInfo is finished spawning bloons, otherwise false</returns>
        public bool isFinished()
        {
            return amountToSpawn == 0;
        }

    }

}

