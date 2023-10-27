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

    public float timer = 0;

    public int guess = 0;
    
    [SerializeField] private float negativeReward = -0.1f;
    [SerializeField] private float perDecisionReward = 0.001f;
    [SerializeField] private float goodReward = 0.5f;
    [SerializeField] private float greatReward = 1f;

    public void SetMap(int[,] map)
    {
        //_map = map;
    }

    public override void OnEpisodeBegin()
    {
        guess = UnityEngine.Random.Range(0, 2);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Generate random int between 0 and 1
        sensor.AddObservation(guess);
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
        UpgradeTower
    }
    
    private enum TowerType
    {
        DoNothing,
        DartMonkey,
        TackShooter,
        SniperMonkey,
        BoomerangMonkey,
        NinjaMonkey,
        BombShooter,
        SpikeFactory,
        SuperMonkey
    }

    private bool ContainsTower()
    {
        //TODO: implement
        return true;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Each time makes a decision give it very small reward.
        AddReward(perDecisionReward);
        
        Decision choice = (Decision) actions.DiscreteActions[0];
        TowerType towerType = (TowerType)actions.DiscreteActions[1];
        
        switch (choice)
        {
            case Decision.DoNothing:
                if (towerType != TowerType.DoNothing)
                {
                    AddReward(negativeReward);
                }
                break;
            case Decision.PlaceTower:
                if (towerType != TowerType.DoNothing || ContainsTower())
                {
                    AddReward(negativeReward);
                }
                else
                {
                    AddReward(goodReward);
                }
                break;
        }
        
    }
}
