using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WaveManager : MonoBehaviour
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

    private bool autoPlay = false;

    private float timer = 0f;

    private void StartWave()
    {
        isPlaying = true;
        ++CurrentWaveNumber;
        if (waves.Count != 0)
        {
            Debug.Log("Wave " + CurrentWaveNumber + " started");
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
        public GameObject RedBloonPrefab;
        public GameObject BlueBloonPrefab;
        public GameObject GreenBloonPrefab;
        public GameObject YellowBloonPrefab;
        public GameObject PinkBloonPrefab;
        public int WaveRBE;
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

                spawnInfo.Initialize(this); // Pass the reference to the parent WaveEvent

                spawnInfo.Start();
                enemies += spawnInfo.GetTotalBloonCount() * GetBloonRBE(spawnInfo.Bloon); // This was added to Calculate RBE for this spawnInfo;
            }

            return enemies;
        }

        private int GetBloonRBE(GameObject bloon)
        {
            // Create a dictionary to map prefab references to RBE values
            Dictionary<GameObject, int> bloonRBE = new Dictionary<GameObject, int>
            {
                { RedBloonPrefab, 1 },
                { BlueBloonPrefab, 2 },
                { GreenBloonPrefab, 3 },
                { YellowBloonPrefab, 4 },
                { PinkBloonPrefab, 5 },
        // Add more bloon types and prefab references as needed
                };

            if (bloonRBE.ContainsKey(bloon))
            {
                return bloonRBE[bloon];
            }

            return 0; // Default RBE value if the bloon type is not found
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
            private float interval;

            [SerializeField]
            [Tooltip("The time before the first bloon spawns. If set to 0, the first bloon will spawn instantly.")]
            [Min(0.0f)]
            private float countdownToFirstBloonSpawn;

            private float lastTime;
            private float startTime;
            private bool spawnInstant = true;

            private WaveEvent waveEvent; // Add this field

            public void Initialize(WaveEvent waveEvent)
            {
                this.waveEvent = waveEvent;
            }

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

                    // Calculate the RBE value of the spawned bloon and subtract it from the wave's RBE
                    
                 
                    waveEvent.WaveRBE -= bloon.GetComponent<BloonScript>().GetHealth();
                    Debug.Log(waveEvent.WaveRBE);



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

