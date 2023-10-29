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

    public int previousWave = 1;
    
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
        Utils.Print2DArray(_map);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
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
        //DoNothing,
        DartMonkey,
        TackShooter,
        SniperMonkey,
        BoomerangMonkey,
        NinjaMonkey,
        BombShooter,
        SpikeFactory,
        SuperMonkey
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (waveManager.isGameOver)
        {
            AddReward(1f);
            EndEpisode();
            return;
        }
        
        if (gameManager.GetLives() <= 0)
        {
            AddReward(-1f);
            EndEpisode();
            return;
        }
        
        if (waveManager.CurrentWaveNumber > previousWave)
        {
            previousWave = waveManager.CurrentWaveNumber;
            AddReward(0.5f);
        }
        

        Decision choice = (Decision) actions.DiscreteActions[0];
        int xPos = actions.DiscreteActions[1] % 8;
        int yPos = actions.DiscreteActions[1] / 8;

        if (choice == Decision.PlaceTower)
        {
            Tile tile = _tiles[yPos, xPos];
            
            if (tile == null || tile.ContainsTowers())
            {
                AddReward(-0.0125f);
                Debug.Log("Failed to place tower");
            }
            else
            {
                tile.SetContainsTower(true);
                Vector3 tilePos = tile.transform.position;
                tilePos.z = 0;
                DartMonkeyScript script = Instantiate(dartMonkeyScript, tilePos, Quaternion.identity);
                script.SetProjectileContainer(storeManagerScript.projectileContainer);
                script.gameObject.transform.parent = gameObject.transform;
                script.SetTile(tile);
                // Set script pos z to 0
                _map[yPos, xPos] = 2;
                Utils.Print2DArray(_map);
                Debug.Log("Placed tower");
            }
        }
        else
        {
            Debug.Log("Do nothing");
        }

    }
}
