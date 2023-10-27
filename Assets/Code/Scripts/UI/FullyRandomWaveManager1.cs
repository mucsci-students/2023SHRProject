using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FullyRandomWaveManager1 : MonoBehaviour
{
    [Tooltip("The spawn point for the bloons")]
    public Transform spawn;

    [SerializeField]
    [Tooltip("The time between waves in seconds")]
    private float timeBetweenWaves = 15f;

    [Tooltip("The path the bloons will follow. First point is the first point after spawn, last point is the end point.")]
    public List<Transform> path = new();

    [SerializeField]
    private BloonLookUpScript BLUS;

    [SerializeField]
    public List<WaveEvent> waves = new();

    [SerializeField]
    public List<GameObject> balloonPrefabs = new List<GameObject>();

    public int CurrentWaveNumber = 0;
    public static int enemiesRemaining = 0;
    public float TimerRef;

    // Private variables

    /// <summary>
    /// Stores whether the game is currently playing. Is true if any wave has ever started. False if all waves are over.
    /// </summary>
    private bool isPlaying = false;

    /// <summary>
    /// Stores whether the game is currently between rounds. Is true if isPlaying is true and bloon count < 0.
    /// </summary>
    private bool betweenRounds = false;

    //private bool autoPlay = false;

    private float timer = 0f;

    /*
     * Gets a random bloon prefab to generate bloon in the ranndom wave
     * You need to first put the bloon prefabs in the list in the inspector for this function to work
     * 
     * 
     * 
     * 
     */
    public GameObject GetRandomBloonPrefab()
    {
        if (balloonPrefabs.Count == 0)
        {
            Debug.LogError("No balloon prefabs available. Make sure to assign them in the inspector.");
            return null;
        }

        int randomIndex = Random.Range(0, balloonPrefabs.Count);
        return balloonPrefabs[randomIndex];
    }

    private void StartWave()
    {
        isPlaying = true;
        ++CurrentWaveNumber;
        if (waves.Count != 0)
        {
            Debug.Log("Wave " + CurrentWaveNumber + " started");
            waves[0].GenerateRandomWave(this); //This will generate a random wave
            enemiesRemaining = waves[0].StartWave();
        }
        else
        {
            Debug.Log("Event ended");
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) && !isPlaying)
            StartWave();

        if (!isPlaying)
            return;

        // If wave is over
        if (!betweenRounds && !waves[0].RunCurrentWave(path, spawn, null, BLUS) && enemiesRemaining == 0)
        {
            Debug.Log("Wave Ended");
            waves.RemoveAt(0);
            if (waves.Count == 0)
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
            TimerRef = timer; // used for ui
            if (timer > timeBetweenWaves || Input.GetKeyDown(KeyCode.Space))
            {
                ++CurrentWaveNumber;
                waves[0].GenerateRandomWave(this); // Generate a random wave for the next round
                enemiesRemaining = waves[0].StartWave();
                timer = 0;
                betweenRounds = false;
            }
        }
    }



    [System.Serializable]
    public class WaveEvent
    {

        public List<SpawnInfo> spawnInfos = new();

        /// <summary>
        /// Called once at the start of a wave to initialize spawning of bloons. 
        /// </summary>
        /// <returns>The total number of bloons in the wave</returns>
        public int StartWave()
        {
            int enemies = 0;
            foreach (var spawnInfo in spawnInfos)
            {
                int randomAmountToSpawn = Random.Range(5, 10); // Example range: 5 to 10 bloons per SpawnInfo
                spawnInfo.amountToSpawn = randomAmountToSpawn;

                spawnInfo.Start();
                enemies += spawnInfo.GetTotalBloonCount();
            }
            return enemies;
        }

        public void GenerateRandomWave(FullyRandomWaveManager1 manager)
        {
            spawnInfos.Clear();
            int numColors = Random.Range(1, 6); // Random number of colors (1 to 5)

            // Add spawn info for each color
            for (int i = 0; i < numColors; i++)
            {
                // Randomize the color
                Color bloonColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);

                // Create a new SpawnInfo with the selected color
                SpawnInfo spawnInfo = new SpawnInfo
                {
                    Bloon = manager.GetRandomBloonPrefab(), // Use the manager instance to call GetRandomBloonPrefab
                    amountToSpawn = Random.Range(5, 10), // Example range: 5 to 10 bloons per SpawnInfo
                    interval = Random.Range(1f, 3f), // Example range: 1 to 3 seconds between spawns
                    countdownToFirstBloonSpawn = 0f,
                    bloonColor = bloonColor
                };

                spawnInfos.Add(spawnInfo);
            }
        }



        /// <summary>
        /// Called each frame while a wave is active to spawn bloons.
        /// </summary>
        /// <param name="path">The path for the newly created bloons to follow</param>
        /// <param name="spawn">Where the bloons should spawn</param>
        /// <param name="Death"></param>
        /// <param name="BLUS">The bloon lookup script, so bloons can spawn new bloons when they take damage</param>
        /// <returns>True if the wave is still running and false if the wave is over</returns>
        public bool RunCurrentWave(List<Transform> path, Transform spawn, AudioSource Death, BloonLookUpScript BLUS)
        {
            if (spawnInfos.Count == 0)
                return false;

            for (var i = 0; i < spawnInfos.Count; i++)
            {
                spawnInfos[i].ReadyToSpawn(path, spawn, Death, BLUS);

                if (spawnInfos[i].isFinished())
                {
                    spawnInfos.RemoveAt(i--);
                }
            }

            return true;
        }

        [System.Serializable]
        public class SpawnInfo
        {

            public GameObject Bloon;

            [SerializeField]
            [Tooltip("The number of bloons to spawn")]
            public int amountToSpawn;

            [SerializeField]
            [Tooltip("Number of seconds between bloon spawns")]
            public float interval;

            [SerializeField]
            [Tooltip("The time before the first bloon spawns. If set to 0, the first bloon will spawn instantly.")]
            [Min(0.0f)]
            public float countdownToFirstBloonSpawn;

            [SerializeField]
            public Color bloonColor;

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

            public void ReadyToSpawn(List<Transform> path, Transform spawn, AudioSource Death, BloonLookUpScript BLUS)
            {
                if (Time.time - startTime < countdownToFirstBloonSpawn)
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
}
