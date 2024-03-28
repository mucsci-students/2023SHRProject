using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

namespace AI
{

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

        private int _dartMonkeysPlaced;
        private int _sniperMonkeysPlaced;
        private int _doNothingCount;
        private int _placeTowerCount;
        private int _totalEpisodes;
        private int _totalRounds;
        
        private bool _dartMonkeyRequested = false;

        public void Start()
        {
            Debug.Log("AI starting");

            RequestDecision();
        }

        public void FixedUpdate()
        {
            // Ask AI if it wants to place a sniper monkey when it has enough money
            if (gameManager.money >= sniperMonkeyScript.GetMonkeyCost()) {
                RequestDecision();
            }
            
            // Ask AI if it wants to place a dart monkey once and only once when it has enough money
            if (gameManager.money >= dartMonkeyScript.GetMonkeyCost() && !_dartMonkeyRequested) {
                _dartMonkeyRequested = true;
                RequestDecision();
            }
            
            // Force AI to use all of its money on the first wave
            if (waveManager.CurrentWaveNumber == 1 && gameManager.money >= dartMonkeyScript.GetMonkeyCost())
            {
                RequestDecision();
            }
            
            if (gameManager.GetLives() <= 0)
            {
                Academy.Instance.StatsRecorder.Add("Wave", previousWave, StatAggregationMethod.Histogram);
                EndEpisode();
            }
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

            _dartMonkeysPlaced = 0;
            _sniperMonkeysPlaced = 0;
            _doNothingCount = 0;
            _placeTowerCount = 0;
            _totalEpisodes++;
            _totalRounds++;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // Normalize all values between [-1, 1] for better learning

            const float maxMoney = 10000;
            const float maxWaveNumber = 50;
            const float maxBoardValue = 10;

            sensor.AddObservation(gameManager.money / maxMoney);
            sensor.AddObservation(waveManager.CurrentWaveNumber / maxWaveNumber);

            for (int i = 0; i < _map.GetLength(0); ++i)
            {
                for (int j = 0; j < _map.GetLength(1); ++j)
                {
                    sensor.AddObservation(_map[i, j] / maxBoardValue);
                }
            }

            sensor.AddObservation(0);
        }

        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
            const int mainDecisionBranch = 0;
            const int monkeyBranch = 1;
            const int mapBranch = 2;

            // If AI can not buy cheapest tower (dart monkey) prevent them from placing any tower
            actionMask.SetActionEnabled(mainDecisionBranch, (int)Decision.PlaceTower, CanBuyTower(dartMonkeyScript));

            actionMask.SetActionEnabled(monkeyBranch, (int)TowerType.DartMonkey, CanBuyTower(dartMonkeyScript));
            actionMask.SetActionEnabled(monkeyBranch, (int)TowerType.SniperMonkey, CanBuyTower(dartMonkeyScript));

            
            // Prevent AI from placing tower anywhere that is not an open tile
            for (int i = 0; i < _map.GetLength(0); ++i)
            {
                for (int j = 0; j < _map.GetLength(1); ++j)
                {
                    if (_map[i, j] != (int)Enums.TileData.Open)
                    {
                        actionMask.SetActionEnabled(mapBranch, (int)generateMapScript.xBlocks * i + j, false);
                    }
                }
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            var statsRecorder = Academy.Instance.StatsRecorder;
            
            AddReward(0.001f);

            if (waveManager.isGameOver)
            {
                AddReward(1f);
                statsRecorder.Add("Wave", previousWave, StatAggregationMethod.Histogram);
                EndEpisode();
                return;
            }

            if (gameManager.GetLives() <= 0)
            {
                statsRecorder.Add("Wave", previousWave, StatAggregationMethod.Histogram);
                EndEpisode();
                return;
            }

            if (waveManager.CurrentWaveNumber > previousWave)
            {
                _totalRounds++;
                previousWave = waveManager.CurrentWaveNumber;
                AddReward(0.5f);
                RequestDecision();
            }

            Decision choice = (Decision)actions.DiscreteActions[0];
            TowerType towerType = (TowerType)actions.DiscreteActions[1];
            int xPos = actions.DiscreteActions[2] % (int)generateMapScript.xBlocks;
            int yPos = actions.DiscreteActions[2] / (int)generateMapScript.xBlocks;

            if (yPos == (int)generateMapScript.yBlocks)
            {
                Debug.Log("Choose a dummy tile");
                return;
            }

            statsRecorder.Add("TowerType", (int)towerType, StatAggregationMethod.Histogram);
            statsRecorder.Add("/Map Placement/X Placement", xPos, StatAggregationMethod.Histogram);
            statsRecorder.Add("/Map Placement/Y Placement", yPos, StatAggregationMethod.Histogram);

            var targetingMode = (Enums.TargetingMode)actions.DiscreteActions[3];

            if (choice == Decision.PlaceTower)
            {
                _placeTowerCount++;
                Tile tile = _tiles[yPos, xPos];

                if (tile == null || tile.ContainsTowers())
                {
                    AddReward(-0.001f);
                    Debug.Log("Failed to place tower: chose to place tower but tile was null or contained tower");
                    UpdateStats(statsRecorder);
                    
                    return;
                }
                
                switch (towerType)
                {
                    case TowerType.DartMonkey when CanBuyTower(dartMonkeyScript):
                    {
                        Vector3 tilePos = tile.transform.position;
                        tilePos.z = 0;

                        Debug.Log("Placed Dart Monkey");
                        DartMonkeyScript script = Instantiate(dartMonkeyScript, tilePos, Quaternion.identity);
                        PlaceTower(script, tile, targetingMode);

                        // Update map for AI to see
                        _map[yPos, xPos] = (int)Enums.TileData.DartMonkey;

                        // Update stats for tensorboard
                        statsRecorder.Add("TargetingMode", (int)targetingMode, StatAggregationMethod.Histogram);
                        _dartMonkeysPlaced++;
                        break;
                    }
                    case TowerType.DartMonkey:
                        AddReward(-0.001f);
                        break;
                    case TowerType.SniperMonkey when CanBuyTower(sniperMonkeyScript):
                    {
                        Vector3 tilePos = tile.transform.position;
                        tilePos.z = 0;
                        
                        Debug.Log("Placed Sniper Monkey");
                        SniperMonkeyScript script = Instantiate(sniperMonkeyScript, tilePos, Quaternion.identity);
                        PlaceTower(script, tile, targetingMode);

                        // Update map for AI to see
                        _map[yPos, xPos] = (int)Enums.TileData.SniperMonkey;

                        // Update stats for tensorboard
                        statsRecorder.Add("TargetingMode", (int)targetingMode, StatAggregationMethod.Histogram);
                        _sniperMonkeysPlaced++;
                        break;
                    }
                    case TowerType.SniperMonkey:
                        AddReward(-0.001f);
                        break;
                    case TowerType.DoNothing:
                        Debug.Log("Failed to place tower: chose to place tower but tower type was do nothing");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                _doNothingCount++;
                if (towerType != TowerType.DoNothing)
                {
                    Debug.Log("Failed to place tower: chose to do nothing but tower type was not do nothing");
                }
                else
                {
                    Debug.Log("Do nothing");
                }
            }

            UpdateStats(statsRecorder);
        }

        private void UpdateStats(StatsRecorder statsRecorder)
        {
            // Update stats for tensorboard
            statsRecorder.Add("DartMonkeysPlaced", _dartMonkeysPlaced);
            statsRecorder.Add("SniperMonkeysPlaced", _sniperMonkeysPlaced);
            statsRecorder.Add("DoNothingCount", _doNothingCount);
            statsRecorder.Add("PlaceTowerCount", _placeTowerCount);
            statsRecorder.Add("Total Rounds", _totalRounds);
            statsRecorder.Add("Total Episodes", _totalEpisodes);
        }

        private void PlaceTower(MonkeyScript monkeyScript, Tile tile, Enums.TargetingMode targetingMode)
        {
            _dartMonkeyRequested = false;
            
            tile.SetContainsTower(true);
            
            monkeyScript.SetProjectileContainer(storeManagerScript.projectileContainer);
            monkeyScript.gameObject.transform.parent = gameObject.transform;
            monkeyScript.SetTile(tile);
            monkeyScript.SetTargetingMode(targetingMode);
            
            gameManager.money -= monkeyScript.GetMonkeyCost();
        }

        private bool CanBuyTower(MonkeyScript monkeyScript)
        {
            return gameManager.money >= monkeyScript.GetMonkeyCost();
        }
    }
}
