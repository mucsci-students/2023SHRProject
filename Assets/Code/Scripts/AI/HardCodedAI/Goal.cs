using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace HardCodedAI
{
    
    [Serializable]
    public class Goal
    {
        //things to store
        //store monkey script type
        //

        [SerializeField] private string description;
        [SerializeField] private GoalType goalType;
        [SerializeField] private MonkeyScript monkeyScript;
        private Func<MonkeyScript, bool> _canAchieveGoal;
        private Action<MonkeyScript, Tile> _executeGoal;
        [SerializeField] private Tile tile;

        private enum Decision
        {
            DoNothing,
            PlaceTower,
            //UpgradeTower
        }

        public Goal(string description, GoalType goalType, MonkeyScript monkeyScript, Func<MonkeyScript, bool> canAchieveGoal, Action<MonkeyScript, Tile> executeGoal)
        {
            this.description = description;
            this.goalType = goalType;
            this.monkeyScript = monkeyScript;
            _canAchieveGoal = canAchieveGoal;
            _executeGoal = executeGoal;
        }
        
        public void ExecuteGoal()
        {
            _executeGoal(monkeyScript, tile);
        }
        
        public bool CanAchieveGoal()
        {
            return _canAchieveGoal(monkeyScript);
        }

        public string GetDescription()
        {
            return description;
        }

        public MonkeyScript GetMonkeyScript()
        {
            return monkeyScript;
        }
        
        public GoalType GetGoalType()
        {
            return goalType;
        }
        
        public void SetTile(Tile tile)
        {
            this.tile = tile;
        }
        
        public void SetMonkeyScript(MonkeyScript monkeyScript)
        {
            this.monkeyScript = monkeyScript;
        }

    }
    
    public enum GoalType
    {
        PlaceTower,
        UpgradeTower
    }
}
