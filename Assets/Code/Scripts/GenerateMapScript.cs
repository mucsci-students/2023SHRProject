using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class GenerateMapScript : MonoBehaviour
{
    [SerializeField] private Camera currentSceneCamera;
    public float leftCameraPosition = 0f;
    public float rightCameraPosition = 0f;
    public float topCameraPosition = 0f;
    public float bottomCameraPosition = 0f;
    public float blockSize = 0f;
    public float yBlocks = 0f;
    public float xBlocks = 0f;
    private int[,] _map;

    [SerializeField] [Range(0, 1)] 
    private float moveRightProbability = 0.5f;

    private readonly Random _randomNumberGenerator = new Random();
    
    private enum MoveDirection
    {
        Up,
        Down,
        Right
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (currentSceneCamera == null)
        {
            Debug.LogError("Camera is null on GenerateMapScript on object " + name);
            Destroy(gameObject);
        }
        leftCameraPosition = currentSceneCamera.ViewportToWorldPoint(new Vector3(0f, 0f)).x;
        rightCameraPosition = -leftCameraPosition;
        topCameraPosition = currentSceneCamera.ViewportToWorldPoint(new Vector3(0f, 0f)).y;
        bottomCameraPosition = -topCameraPosition;

        blockSize = Math.Abs(topCameraPosition - bottomCameraPosition) / 10;

        yBlocks = Math.Abs(topCameraPosition - bottomCameraPosition) / blockSize;
        xBlocks = Math.Abs(leftCameraPosition - rightCameraPosition) / blockSize;
        
        _map = new int[(int)Math.Ceiling(yBlocks), (int)Math.Ceiling(xBlocks)];
        
        FillMapArray();
        Print2DArray(_map);
    }

    private void GenerateMapAndDisplay()
    {
        
    }

    private void FillMapArray()
    {
        var startY = _randomNumberGenerator.Next(1, (int)yBlocks - 1);
        var endY = _randomNumberGenerator.Next(1, (int)yBlocks - 1);

        var currPos = new Vector2(0, startY);
        SetMapValue(currPos, 1);
        currPos.x += 1;
        SetMapValue(currPos, 1);
        var lastMove = MoveDirection.Right;

        while (currPos.x < _map.GetLength(1) - 1)
        {
            var nextMove = GetRandomMove(lastMove);

            if (nextMove == MoveDirection.Right)
            {
                MoveRightOnMap(ref currPos, 2);
            } else if (nextMove == MoveDirection.Up)
            {
                MoveUpOnMap(ref currPos, 2);
            } else if (nextMove == MoveDirection.Down)
            {
                MoveDownOnMap(ref currPos, 2);
            }

            lastMove = nextMove;
            
        }
    }

    private void MoveRightOnMap(ref Vector2 currentPosition, uint distance)
    {
        for (var i = 0; i < distance; i++)
        {
            if ((int)currentPosition.x == _map.GetLength(1) - 1) return;
            currentPosition.x += 1;
            SetMapValue(currentPosition, 1);
        }
    }
    
    private void MoveUpOnMap(ref Vector2 currentPosition, uint distance)
    {
        for (var i = 0; i < distance; i++)
        {
            if ((int)currentPosition.y <= 1)
            {
                MoveRightOnMap(ref currentPosition, distance);
                return;
            }
            currentPosition.y -= 1;
            SetMapValue(currentPosition, 1);
        }
    }
    
    private void MoveDownOnMap(ref Vector2 currentPosition, uint distance)
    {
        for (var i = 0; i < distance; i++)
        {
            if ((int)currentPosition.y >= _map.GetLength(0) - 2)
            {
                MoveRightOnMap(ref currentPosition, distance);
                return;
            }
            currentPosition.y += 1;
            SetMapValue(currentPosition, 1);
        }
    }

    private void SetMapValue(Vector2 pos, int value)
    {
        _map[(int)pos.y, (int)pos.x] = value;
    }

    private MoveDirection GetRandomMove(MoveDirection lastMove)
    {
        var randomNumber = _randomNumberGenerator.NextDouble();
        var turnProbability = 1 - moveRightProbability;
        MoveDirection nextMove;
        
        if (randomNumber < turnProbability / 2)
            nextMove = MoveDirection.Up;
        else if (randomNumber < turnProbability)
            nextMove = MoveDirection.Down;
        else
            nextMove = MoveDirection.Right;

        if (lastMove == MoveDirection.Down && nextMove == MoveDirection.Up)
            nextMove = MoveDirection.Down;
        else if (lastMove == MoveDirection.Up && nextMove == MoveDirection.Down)
            nextMove = MoveDirection.Up;

        return nextMove;
    }

    private static void Print2DArray(int[,] array)
    {
        for (var i = 0; i < array.GetLength(0); i++)
        {
            var message = "Row " + i + ": ";
            for (var j = 0; j < array.GetLength(1); j++) {
                message += array[i, j] + " ";
            }
            Debug.Log(message);
        }
    }
    
    
}
