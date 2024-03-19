using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HardCodedAI
{
    public class AgentScript : MonoBehaviour
    {
        private int[,] _map;
        private Tile[,] _tiles;
        
        [Header("Managers Scripts")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private MonkeySpawner storeManagerScript;
        [SerializeField] private GenerateMapScript generateMapScript;
        [SerializeField] private WaveManager waveManager;
        
        [Header("Tower References")]
        [SerializeField] private DartMonkeyScript dartMonkeyScript;
        [SerializeField] private SniperMonkeyScript sniperMonkeyScript;
        
        
        [Header("Settings")]
        
        [SerializeField, Range(1, 1000)] private int framesBetweenDecisions = 100;

        [Header("Debugging")]
        private readonly List<Goal> goals = new();
        
        private readonly Metrics _metrics = new();
        private Goal _currentGoal = null;

        // Start is called before the first frame update
        private IEnumerator Start()
        {
            Debug.Log("Loading");
            yield return new WaitForSeconds(5f);
            Debug.Log("Agent Started");
            
            // Add a new goal to the list
            _map = generateMapScript.GetMap();
            _tiles = generateMapScript.GetTileMap();
            
            goals.Add(new Goal("Buy Monkey Tower", GoalType.PlaceTower, dartMonkeyScript, CanBuyTower, BuyAndPlaceTower));
            goals.Add(new Goal("Buy Sniper Tower", GoalType.PlaceTower, sniperMonkeyScript, CanBuyTower, BuyAndPlaceTower));
        }

        // Update is called once per frame
        private void Update()
        {
            if (Time.frameCount % framesBetweenDecisions != 0 || _map == null) return;

            if (_currentGoal == null)
            {
                //TODO come up with a better way to choose a goal
                _currentGoal = GetNewGoal();
                
                if (_currentGoal.GetGoalType() == GoalType.PlaceTower)
                {
                    var tile = _currentGoal.GetMonkeyScript() is SniperMonkeyScript 
                        ? MapUtils.GetFirstAvailableTile(ref _map, ref _tiles) 
                        : MapUtils.GetRandomAvailableTile(ref _map, ref _tiles);
                 
                    if (tile == null) {
                        _currentGoal = null;
                        return;
                    }
                    
                    _currentGoal.SetTile(tile);
                }
                
            } else if (_currentGoal.CanAchieveGoal())
            {
                _currentGoal.ExecuteGoal();
                _currentGoal = null;
                Debug.Log("Executed Goal");
            }
        }
        
        private Goal GetNewGoal() {
            Goal newGoal;
            
            if (waveManager.CurrentWaveNumber < 3) {
                newGoal = GoalUtils.GetARandomBuyGoal(goals);
            } else {
                newGoal = GoalUtils.GetRandomGoal(goals);
            }

            return newGoal;
        }

        private bool CanBuyTower(MonkeyScript monkeyScript)
        {
            return gameManager.money >= monkeyScript.GetMonkeyCost();
        }
        
        private bool CanBuyUpgradePath1(MonkeyScript monkeyScript)
        {
            if (monkeyScript.GetUpgradePath1().Count == 0) {
                goals.Remove(_currentGoal);
                _currentGoal = null;
                return false;
            }
            
            return gameManager.money >= monkeyScript.GetUpgradePath1()[0].GetCost();
        }
    
        private bool CanBuyUpgradePath2(MonkeyScript monkeyScript)
        {
            if (monkeyScript.GetUpgradePath2().Count == 0) {
                goals.Remove(_currentGoal);
                _currentGoal = null;
                return false;
            }
            
            return gameManager.money >= monkeyScript.GetUpgradePath2()[0].GetCost();
        }
        
        private void PurchaseUpgradePath1(MonkeyScript monkeyScript, Tile tile)
        {
            if (monkeyScript.GetUpgradePath1().Count == 0) {
                goals.Remove(_currentGoal);
                _currentGoal = null;
                return;
            }

            Upgrade currentUpgrade = monkeyScript.GetUpgradePath1()[0];

            gameManager.money -= currentUpgrade.GetCost();
            monkeyScript.SetMonkeySellPrice(monkeyScript.GetMonkeySellPrice() + currentUpgrade.GetCost());
            currentUpgrade.UpgradeTower();
            
            _metrics.UpgradesBought++;
        }
        
        private void PurchaseUpgradePath2(MonkeyScript monkeyScript, Tile tile)
        {
            if (monkeyScript.GetUpgradePath2().Count == 0) {
                goals.Remove(_currentGoal);
                _currentGoal = null;
                return;
            }

            Upgrade currentUpgrade = monkeyScript.GetUpgradePath2()[0];

            gameManager.money -= currentUpgrade.GetCost();
            monkeyScript.SetMonkeySellPrice(monkeyScript.GetMonkeySellPrice() + currentUpgrade.GetCost());
            currentUpgrade.UpgradeTower();
            
            _metrics.UpgradesBought++;
        }

        private MonkeyScript PlaceTower(MonkeyScript monkeyScript, Tile tile) {
            var instantiatedScript = Instantiate(monkeyScript, tile.transform.position, Quaternion.identity);
            instantiatedScript.SetProjectileContainer(storeManagerScript.projectileContainer);
            instantiatedScript.gameObject.transform.parent = gameObject.transform;
            instantiatedScript.transform.Translate(0, 0, -1);
            instantiatedScript.SetTile(tile);
            instantiatedScript.SetTargetingMode(Enums.TargetingMode.First);
            
            return instantiatedScript;
        }
        
        private void BuyAndPlaceTower(MonkeyScript monkeyScript, Tile tile) {
            var instantiatedScript = PlaceTower(monkeyScript, tile);
            
            gameManager.money -= monkeyScript.GetMonkeyCost();
            
            _metrics.TowerCount++;
            
            // Add goals to upgrade tower it just placed for future decisions
            goals.Add(new Goal("Upgrade path 1 on a tower", GoalType.UpgradeTower, instantiatedScript, CanBuyUpgradePath1, PurchaseUpgradePath1));
            goals.Add(new Goal("Upgrade path 2 on a tower", GoalType.UpgradeTower, instantiatedScript, CanBuyUpgradePath2, PurchaseUpgradePath2));
            
        }
    }   
}
