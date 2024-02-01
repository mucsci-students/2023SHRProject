
using System;

namespace HardCodedAI
{
    public class Goal
    {
        //things to store
        //store monkey script type
        //

        private string _description;
        private GoalType _goalType;
        private MonkeyScript _monkeyScript;
        private Func<MonkeyScript, bool> _canAchieveGoal;
        private Action<MonkeyScript, Tile> _executeGoal;
        private Tile _tile;

        private enum Decision
        {
            DoNothing,
            PlaceTower,
            //UpgradeTower
        }

        public Goal(string description, GoalType goalType, MonkeyScript monkeyScript, Func<MonkeyScript, bool> canAchieveGoal, Action<MonkeyScript, Tile> executeGoal)
        {
            _description = description;
            _goalType = goalType;
            _monkeyScript = monkeyScript;
            _canAchieveGoal = canAchieveGoal;
            _executeGoal = executeGoal;
        }
        
        public void ExecuteGoal()
        {
            _executeGoal(_monkeyScript, _tile);
        }
        
        public bool CanAchieveGoal()
        {
            return _canAchieveGoal(_monkeyScript);
        }

        public string GetDescription()
        {
            return _description;
        }

        public MonkeyScript GetMonkeyScript()
        {
            return _monkeyScript;
        }
        
        public GoalType GetGoalType()
        {
            return _goalType;
        }
        
        public void SetTile(Tile tile)
        {
            _tile = tile;
        }
        
        public void SetMonkeyScript(MonkeyScript monkeyScript)
        {
            _monkeyScript = monkeyScript;
        }

    }
    
    public enum GoalType
    {
        PlaceTower,
        UpgradeTower
    }
}
