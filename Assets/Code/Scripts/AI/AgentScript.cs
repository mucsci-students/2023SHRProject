using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

/// <summary>
/// Used to control and communicate with the agent.
/// </summary>
public class AgentScript : Agent
{
    private int[,] _map;
    private Tile[,] _tiles;

    [SerializeField] private GenerateMapScript generateMapScript;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private MonkeySpawner storeManagerScript;
    [SerializeField] private DartMonkeyScript dartMonkeyScript;
    [SerializeField] private SniperMonkeyScript sniperMonkeyScript;

    public int previousWave = 1;

    private int DartMonkeysPlaced = 0;
    private int SniperMonkeysPlaced = 0;
    private int PlaceMonkeyCorrectly = 0;
    private int PlaceMonkeyIncorrectly = 0;
    private int DoNothingCount = 0;
    private int PlaceTowerCount = 0;
    private int TotalEpisodes = 0;
    private int TotalRounds = 0;
    
    //[SerializeField] private float negativeReward = -0.1f;
    //[SerializeField] private float perDecisionReward = 0.001f;
    //[SerializeField] private float goodReward = 0.5f;
    //[SerializeField] private float greatReward = 1f;

    public void Start()
    {
        Debug.Log("AI starting");
    }

    public override void OnEpisodeBegin()
    {
        // Destroy all monkeys
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        waveManager.DestroyAllBloons();
        storeManagerScript.DestroyAllProjectiles();

        previousWave = 1;
        generateMapScript.GenerateMap();
        _map = generateMapScript.GetMap();
        _tiles = generateMapScript.GetTileMap();
        waveManager.ResetAll();
        gameManager.ResetAll();
        waveManager.StartWave();
        //Utils.Print2DArray(_map);
        
        DartMonkeysPlaced = 0;
        SniperMonkeysPlaced = 0;
        PlaceMonkeyCorrectly = 0;
        PlaceMonkeyIncorrectly = 0;
        DoNothingCount = 0;
        PlaceTowerCount = 0;
        TotalEpisodes++;
        TotalRounds++;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        float MaxMoney = 10000;
        sensor.AddObservation(((float) gameManager.Money) / MaxMoney);
        sensor.AddObservation(waveManager.CurrentWaveNumber);
        //Generate random int between 0 and 1
        for (int i = 0; i < _map.GetLength(0); i++)
        {
            for (int j = 0; j < _map.GetLength(1); j++)
            {
                sensor.AddObservation(_map[i, j]);
            }
        }
        //Utils.Print2DArray(_map);
    }
    
    // 1. Can do nothing
    // 2. Place tower
    // 3. Upgrade tower
    // 4. Sell tower
    
    // 0. Do not place tower
    // 1. Place Dart Monkey
    // 2. Place Tack Shooter
    // 3. Place Sniper Monkey
    // 4. Place Boomerang Monkey
    // 5. Place Ninja Monkey
    // 6. Place Bomb Shooter
    // 7. Spike Factory
    // 8. Super Monkey
    
    // Get X position (0, 14)
    
    // Get Y position (0, 9)
    
    private enum Decision
    {
        DoNothing,
        PlaceTower,
        //UpgradeTower
    }
    
    private enum TowerType
    {
        DoNothing,
        DartMonkey,
        //BoomerangMonkey,
        //TackShooter,
        SniperMonkey,
        //BoomerangMonkey,
        //NinjaMonkey,
        //BombShooter,
        //SpikeFactory,
        //SuperMonkey
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var statsRecorder = Academy.Instance.StatsRecorder;
        
        if (waveManager.isGameOver)
        {
            AddReward(1f);
            EndEpisode();
            return;
        }
        
        if (gameManager.GetLives() <= 0)
        {
            //AddReward(-1f);
            EndEpisode();
            return;
        }
        
        //AddReward(0.005f);
        
        if (waveManager.CurrentWaveNumber > previousWave)
        {
            TotalRounds++;
            previousWave = waveManager.CurrentWaveNumber;
            AddReward(0.5f);
        }
        

        Decision choice = (Decision) actions.DiscreteActions[0];
        TowerType towerType = (TowerType) actions.DiscreteActions[1];
        int xPos = actions.DiscreteActions[2];
        int yPos = actions.DiscreteActions[3];
        
        statsRecorder.Add("TowerType", (int) towerType);
        statsRecorder.Add("X Placement", xPos);
        statsRecorder.Add("Y Placement", yPos);
        
        Enums.TargetingMode targetingMode = (Enums.TargetingMode) actions.DiscreteActions[4];

        if (choice == Decision.PlaceTower)
        {
            PlaceTowerCount++;
            Tile tile = _tiles[yPos, xPos];
            
            if (tile == null || tile.ContainsTowers())
            {
                AddReward(-0.001f);
                Debug.Log("Failed to place tower");
                PlaceMonkeyIncorrectly++;
            }
            else
            {
                tile.SetContainsTower(true);
                Vector3 tilePos = tile.transform.position;
                tilePos.z = 0;
                if (towerType == TowerType.DartMonkey)
                {
                    if (CanBuyTower(dartMonkeyScript))
                    {
                        DartMonkeyScript script = Instantiate(dartMonkeyScript, tilePos, Quaternion.identity);
                        script.SetProjectileContainer(storeManagerScript.projectileContainer);
                        script.gameObject.transform.parent = gameObject.transform;
                        script.SetTile(tile);
                        script.SetTargetingMode(targetingMode);
                        script.transform.GetChild(0).gameObject.GetComponent<CircleCollider2D>().enabled = false;
                        script.transform.GetChild(0).gameObject.GetComponent<CircleCollider2D>().enabled = true;
                        statsRecorder.Add("TargetingMode", (int) targetingMode);
                        _map[yPos, xPos] = 2;
                        Debug.Log("Placed Dart Monkey");
                        gameManager.Money -= script.GetMonkeyCost();
                        DartMonkeysPlaced++;
                        PlaceMonkeyCorrectly++;
                    }
                    else
                    {
                        AddReward(-0.001f);
                    }
                }
                else if (towerType == TowerType.SniperMonkey)
                {
                    if (CanBuyTower(sniperMonkeyScript))
                    {
                        SniperMonkeyScript script = Instantiate(sniperMonkeyScript, tilePos, Quaternion.identity);
                        script.SetProjectileContainer(storeManagerScript.projectileContainer);
                        script.gameObject.transform.parent = gameObject.transform;
                        script.SetTile(tile);
                        script.SetTargetingMode(targetingMode);
                        script.transform.GetChild(0).gameObject.GetComponent<CircleCollider2D>().enabled = false;
                        script.transform.GetChild(0).gameObject.GetComponent<CircleCollider2D>().enabled = true;
                        statsRecorder.Add("TargetingMode", (int) targetingMode);
                        _map[yPos, xPos] = 3;
                        Debug.Log("Placed Sniper Monkey");
                        gameManager.Money -= script.GetMonkeyCost();
                        SniperMonkeysPlaced++;
                        PlaceMonkeyCorrectly++;
                    }
                    else
                    {
                        AddReward(-0.001f);
                    }
                }
                else if (towerType == TowerType.DoNothing)
                {
                    Debug.Log("Failed to place tower");
                    //AddReward(-0.005f);
                    //PlaceMonkeyIncorrectly++;
                }
                //Utils.Print2DArray(_map);
                //AddReward(0.01f);
            }
        }
        else
        {
            DoNothingCount++;
            if (towerType != TowerType.DoNothing)
            {
                //AddReward(-0.005f);
                Debug.Log("Failed to place tower");
                //PlaceMonkeyIncorrectly++;
            }
            else
            {
                Debug.Log("Do nothing");
            }
        }
        
        statsRecorder.Add("DartMonkeysPlaced", DartMonkeysPlaced);
        statsRecorder.Add("SniperMonkeysPlaced", SniperMonkeysPlaced);
        statsRecorder.Add("PlaceMonkeyCorrectly", PlaceMonkeyCorrectly);
        statsRecorder.Add("PlaceMonkeyIncorrectly", PlaceMonkeyIncorrectly);
        statsRecorder.Add("DoNothingCount", DoNothingCount);
        statsRecorder.Add("PlaceTowerCount", PlaceTowerCount);
        if (PlaceMonkeyIncorrectly != 0)
            statsRecorder.Add("PlacedTowerCorrectlyRatio", (float) PlaceMonkeyCorrectly / (float) PlaceMonkeyIncorrectly);
        statsRecorder.Add("Wave", previousWave);
        statsRecorder.Add("Total Rounds", TotalRounds);
        statsRecorder.Add("Total Episodes", TotalEpisodes);
    }
    
    private bool CanBuyTower(MonkeyScript monkeyScript)
    {
        return gameManager.Money >= monkeyScript.GetMonkeyCost();
    }
}
