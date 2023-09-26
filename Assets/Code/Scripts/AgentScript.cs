using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentScript : Agent
{
    private int[,] _map;

    public float timer = 0;

    public int guess = 0;

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

    public override void OnActionReceived(ActionBuffers actions)
    {
        int choice = actions.DiscreteActions[0];

        if (choice == guess)
        {
            AddReward(1f); // Strong negative reward for choosing 1
            Debug.Log("Correct");
        }
        else
        {
            AddReward(-1f);  // Positive reward for choosing 0
            Debug.Log("Incorrect");
        } 
        EndEpisode();
    }
}
