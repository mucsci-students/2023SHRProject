
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HardCodedAI
{
    public class AgentScript : MonoBehaviour
    {
        private int[,] _map;
        private Tile[,] _tiles;
        
        [SerializeField] private GameManager gameManager;
        [SerializeField] private MonkeySpawner storeManagerScript;
        [SerializeField] private GenerateMapScript generateMapScript;
        
        [SerializeField] private DartMonkeyScript dartMonkeyScript;
        [SerializeField] private SniperMonkeyScript sniperMonkeyScript;
        
        [SerializeField] private List<MonkeyScript> currentMonkeys = new();
        
        private readonly List<Goal> _goals = new();
        private Goal _currentGoal;
        
        [SerializeField] private string currentGoalName;

        // Start is called before the first frame update
        private IEnumerator Start()
        {
            Debug.Log("Loading");
            yield return new WaitForSeconds(2f);
            Debug.Log("Agent Started");
            // Add a new goal to the list
            _map = generateMapScript.GetMap();
            _tiles = generateMapScript.GetTileMap();
            _goals.Add(new Goal("Buy monkey tower", GoalType.PlaceTower, dartMonkeyScript, CanBuyTower, BuyAndPlaceTower));
            _goals.Add(new Goal("Buy sniper tower", GoalType.PlaceTower, sniperMonkeyScript, CanBuyTower, BuyAndPlaceTower));
        }

        // Update is called once per frame
        private void Update()
        {
            if (Time.frameCount % 100 != 0 && _map == null) return;
            
            if (_currentGoal == null)
            {
                // Get Goal
                Debug.Log(_map);
                
                //TODO Add a check to see if the map is full and decide what tile to choose smartly
                var tile = GetRandomTile();
                
                //TODO come up with a better way to choose a goal
                _currentGoal = _goals[Random.Range(0, _goals.Count)];
                
                _currentGoal.SetTile(tile);
                currentGoalName = _currentGoal.GetDescription();

                Debug.Log(currentGoalName);
            } else if (_currentGoal.CanAchieveGoal())
            {
                _currentGoal.ExecuteGoal();
                _currentGoal = null;
                currentGoalName = "No Goal";
                Debug.Log("Executed Goal");
            }
        }

        private Tile GetRandomTile()
        {
            for (int i = 0; i < _map.GetLength(0); i++)
            {
                for (int j = 0; j < _map.GetLength(1); j++)
                {
                    if (_map[i, j] == 0)
                    {
                        _map[i, j] = 1;
                        return _tiles[i, j];
                    }
                }
            }

            return null;
        }

        private bool CanBuyTower(MonkeyScript monkeyScript)
        {
            return gameManager.money >= monkeyScript.GetMonkeyCost();
        }
        
        private bool CanBuyUpgradePath1(MonkeyScript monkeyScript)
        {
            return gameManager.money >= monkeyScript.GetUpgradePath1()[0].GetCost();
        }
    
        private bool CanBuyUpgradePath2(MonkeyScript monkeyScript)
        {
            return gameManager.money >= monkeyScript.GetUpgradePath2()[0].GetCost();
        }
        
        private void PurchaseUpgradePath1(MonkeyScript monkeyScript, Tile tile)
        {
            //TODO: Remove goal associated with this upgrade path on tower somehow ?
            if (monkeyScript.GetUpgradePath1().Count == 0)
                return;

            Upgrade currentUpgrade = monkeyScript.GetUpgradePath1()[0];

            gameManager.money -= currentUpgrade.GetCost();
            monkeyScript.SetMonkeySellPrice(monkeyScript.GetMonkeySellPrice() + currentUpgrade.GetCost());
            currentUpgrade.UpgradeTower();
        }
        
        private void PurchaseUpgradePath2(MonkeyScript monkeyScript, Tile tile)
        {
            //TODO: Remove goal associated with this upgrade path on tower somehow ?
            if (monkeyScript.GetUpgradePath2().Count == 0)
                return;

            Upgrade currentUpgrade = monkeyScript.GetUpgradePath2()[0];

            gameManager.money -= currentUpgrade.GetCost();
            monkeyScript.SetMonkeySellPrice(monkeyScript.GetMonkeySellPrice() + currentUpgrade.GetCost());
            currentUpgrade.UpgradeTower();
        }
        
        private void BuyAndPlaceTower(MonkeyScript monkeyScript, Tile tile)
        {
            MonkeyScript script = Instantiate(monkeyScript, tile.transform.position, Quaternion.identity);
            script.SetProjectileContainer(storeManagerScript.projectileContainer);
            script.gameObject.transform.parent = gameObject.transform;
            script.SetTile(tile);
            script.SetTargetingMode(Enums.TargetingMode.First);
            gameManager.money -= monkeyScript.GetMonkeyCost();
            currentMonkeys.Add(script);
            
            // Add goals to upgrade tower it just placed for future decisions
            _goals.Add(new Goal("Upgrade path 1 on a tower", GoalType.UpgradeTower, script, CanBuyUpgradePath1, PurchaseUpgradePath1));
            _goals.Add(new Goal("Upgrade path 2 on a tower", GoalType.UpgradeTower, script, CanBuyUpgradePath2, PurchaseUpgradePath2));
            
        }
    }   
}
