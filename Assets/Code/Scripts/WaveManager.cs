using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Transform spawn;
    [SerializeField] private float WaveTimer = 15f;
    private float timer = 0f;
    [SerializeField] private List<Transform> path;

    [SerializeField] private BloonLookUpScript BLUS;

    public List<WaveEvent> events = new();

    public int WaveNumber = 0;
    public static int enemiesRemaining = 0;
    public float TimerRef;

    private bool isPlaying = false;
    private bool betweenRounds = false;
    private bool autoPlay = false;

    

    public void StartWave()
    {
        isPlaying = true;
        ++WaveNumber;
        if (events.Count != 0)
        {
            enemiesRemaining = events[0].StartEvent(WaveNumber);
        }
        else
        {
            Debug.Log("Event ended");
        }
    }

    void Update()
    {


        if (Input.GetKeyDown(KeyCode.Space) && !isPlaying)
            StartWave();

        if (!isPlaying)
            return;

        if (!betweenRounds && !events[0].RunEvent(path, spawn, null, BLUS) && enemiesRemaining == 0)
        {
            Debug.Log("Wave Ended");
            events.RemoveAt(0);
            if (events.Count == 0)
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
            // Hide bug
            enemiesRemaining = 0;
            timer += Time.unscaledDeltaTime;
            TimerRef = timer; // used for ui
            if (timer > WaveTimer || Input.GetKeyDown(KeyCode.Space))
            {
                ++WaveNumber;
                enemiesRemaining = events[0].StartEvent(WaveNumber);
                timer = 0;
                betweenRounds = false;
            }
        }
    }

    [System.Serializable]
    public class WaveEvent
    {

        public List<SpawnInfo> spawnInfos = new();

        public int StartEvent(int waveNumber)
        {
            Debug.Log("Wave " + waveNumber + " started");
            int enemies = 0;
            foreach (var spawnInfo in spawnInfos)
            {
                spawnInfo.Start();
                enemies += spawnInfo.amount;
            }
            return enemies;
        }

        public bool RunEvent(List<Transform> path, Transform spawn, AudioSource Death, BloonLookUpScript BLUS)
        {
            if (spawnInfos.Count == 0)
                return false;

            for (int i = 0; i < spawnInfos.Count; i++)
            {
                spawnInfos[i].ReadyToSpawn(path, spawn, Death, BLUS);

                if (spawnInfos[i].amount == 0)
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
            public int amount;
            public float interval;
            public float timeToStart;

            private float lastTime;
            private float startTime;
            private bool spawnInstant = true;

            public void Start()
            {
                lastTime = Time.time;
                startTime = Time.time;
                if (timeToStart == 0.0f)
                    spawnInstant = false;
            }

            public void ReadyToSpawn(List<Transform> path, Transform spawn, AudioSource Death, BloonLookUpScript BLUS)
            {
                if (Time.time - startTime < timeToStart)
                    return;

                if (spawnInstant || Time.time - lastTime >= interval)
                {
                    GameObject bloon = Instantiate(Bloon);
                    bloon.transform.position = spawn.position;
                    bloon.transform.rotation = spawn.rotation;


                    bloon.GetComponent<BloonScript>().SetBloonLookUpScript(BLUS);
                    bloon.GetComponent<PathFollowingScript>().SetBloonPath(path);
                    

                    --amount;
                    lastTime = Time.time;
                    spawnInstant = false;
                }
            }

        }

    }

}
