using System.Collections.Generic;
using System.Linq;
using UnityRandom = UnityEngine.Random;
using Random = System.Random;

namespace HardCodedAI {
    public static class GoalUtils {
        
        public static Goal GetRandomGoal(List<Goal> goals) {
            return goals[UnityRandom.Range(0, goals.Count)];
        }
        
        public static Goal GetARandomBuyGoal(List<Goal> goals) {
            return Shuffle(goals).FirstOrDefault(goal => goal.GetGoalType() == GoalType.PlaceTower);
        }
        
        public static Goal GetARandomUpgradeGoal(List<Goal> goals) {
            return Shuffle(goals).FirstOrDefault(goal => goal.GetGoalType() == GoalType.UpgradeTower);
        }

        private static IList<T> Shuffle<T>(this IList<T> list)  
        {
            var rng = new Random();

            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);  
                T value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }

            return list;
        }
        
    }
}
