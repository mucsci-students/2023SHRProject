using System;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

public class GenerateMapScript : MonoBehaviour
{
    [Header("Object Links")]
    
    [SerializeField] private Camera currentSceneCamera;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private WaveManager waveManager;
    
    [Header("Map Settings")]
    
    [SerializeField] [Range(0, 1)] 
    private float moveRightProbability = 0.5f;
    
    private float leftCameraPosition = 0f;
    private float rightCameraPosition = 0f;
    private float topCameraPosition = 0f;
    private float bottomCameraPosition = 0f;
    private float blockSize = 0f;
    private float yBlocks = 0f;
    private float xBlocks = 0f;
    private int[,] _map;
    private bool[,] _visited;
    private int cornerCount = 0;  // Add this line at the top of your class with other member variables



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
        
        CalculateMapSize();
        _visited = new bool[(int)Math.Ceiling(yBlocks), (int)Math.Ceiling(xBlocks)];
        _map = new int[(int)Math.Ceiling(yBlocks), (int)Math.Ceiling(xBlocks)];
        GenerateRandomMap();
        DisplayMap(_map);
        DetectCorners();
    }
    
    private void CalculateMapSize()
    {
        Vector3 bottomLeftCameraPosition = currentSceneCamera.ViewportToWorldPoint(new Vector3(0f, 0f));
        leftCameraPosition = bottomLeftCameraPosition.x;
        rightCameraPosition = -leftCameraPosition;
        topCameraPosition = bottomLeftCameraPosition.y;
        bottomCameraPosition = -topCameraPosition;

        blockSize = Math.Abs(topCameraPosition - bottomCameraPosition) / 10;

        yBlocks = Math.Abs(topCameraPosition - bottomCameraPosition) / blockSize;
        xBlocks = Math.Abs(leftCameraPosition - rightCameraPosition) / blockSize - 2;
    }

    [ContextMenu("Regenerate Map")]
    private void RegenerateMap()
    {
        _visited = new bool[(int)Math.Ceiling(yBlocks), (int)Math.Ceiling(xBlocks)];
        _map = new int[(int)Math.Ceiling(yBlocks), (int)Math.Ceiling(xBlocks)];
        DeleteMap();
        GenerateRandomMap();
        DisplayMap(_map);
        DetectCorners();
    }

    [ContextMenu("Delete Map")]
    private void DeleteMap()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void GenerateRandomMap()
    {
        var startY = _randomNumberGenerator.Next(1, (int)yBlocks - 1);
        var endY = _randomNumberGenerator.Next(1, (int)yBlocks - 1);

        var currPos = new Vector2(0, startY);
        SetMapValue(currPos, 1);
        currPos.x += 1;
        SetMapValue(currPos, 1);
        var lastMove = MoveDirection.Right;

        while (currPos.x < _map.GetLength(1) - 2)
        {
            var nextMove = GetRandomMove(lastMove);

            lastMove = nextMove switch
            {
                MoveDirection.Right => MoveRightOnMap(ref currPos, 2),
                MoveDirection.Up => MoveUpOnMap(ref currPos, 2),
                MoveDirection.Down => MoveDownOnMap(ref currPos, 2),
                _ => lastMove
            };
        }
    }

    private MoveDirection MoveRightOnMap(ref Vector2 currentPosition, uint distance)
    {
        for (var i = 0; i < distance; i++)
        {
            if ((int)currentPosition.x == _map.GetLength(1) - 1) return MoveDirection.Right;
            currentPosition.x += 1;
            SetMapValue(currentPosition, 1);
        }
        return MoveDirection.Right;
    }
    
    private MoveDirection MoveUpOnMap(ref Vector2 currentPosition, uint distance)
    {
        for (var i = 0; i < distance; i++)
        {
            if ((int)currentPosition.y <= 1)
            {
                MoveRightOnMap(ref currentPosition, distance);
                MoveDownOnMap(ref currentPosition, distance);
                return MoveDirection.Down;
            }
            currentPosition.y -= 1;
            SetMapValue(currentPosition, 1);
        }
        return MoveDirection.Up;
    }

    private MoveDirection MoveDownOnMap(ref Vector2 currentPosition, uint distance)
    {
        for (var i = 0; i < distance; i++)
        {
            if ((int)currentPosition.y >= _map.GetLength(0) - 2)
            {
                MoveRightOnMap(ref currentPosition, distance);
                MoveUpOnMap(ref currentPosition, distance);
                return MoveDirection.Up;
            }
            currentPosition.y += 1;
            SetMapValue(currentPosition, 1);
        }
        return MoveDirection.Down;
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
    
    private void DisplayMap(int[,] array)
    {
        for (int y = 0; y < array.GetLength(0); y++)
        {
            for (int x = 0; x < array.GetLength(1); x++)
            {
                if (array[y, x] == 0)
                {
                    var spawnedTile = Instantiate(tilePrefab, new Vector3(x + leftCameraPosition + blockSize / 2, y + topCameraPosition + blockSize / 2, 10), quaternion.identity);
                    spawnedTile.name = $"Tile {y} {x}";

                    var isOffsetColor = (y % 2 == 0 && x % 2 != 0) || (y % 2 != 0 && x % 2 == 0);
                    spawnedTile.Init(isOffsetColor);
                    spawnedTile.transform.parent = transform;
                }
                else if (array[y, x] == 1)
                {
                    var spawnedRoad = Instantiate(roadPrefab, new Vector3(x + leftCameraPosition + blockSize / 2, y + topCameraPosition + blockSize / 2, 10), quaternion.identity);
                    spawnedRoad.name = $"Road {y} {x}";
                    spawnedRoad.transform.parent = transform;
                }
            }
        }

        //camera.transform.position = new Vector3((float)width/2-0.5f,(float)height/2-0.5f,-10);
    }
    
    private void DetectCorners()
    {
        // Start from each cell on the left border
        for (int y = 0; y < _map.GetLength(0); y++)
        {
            if (_map[y, 0] == 1)
            {
                CornerFunction(0, y); // Consider the far-left '1' as a corner
                FindCorners(0, y, -1);
            }
        }
    }

    private void FindCorners(int x, int y, int prevDir)
    {
        if (x < 0 || x >= _map.GetLength(1) || y < 0 || y >= _map.GetLength(0) || _visited[y, x] || _map[y, x] == 0)
        {
            return;
        }

        _visited[y, x] = true;

        // Consider the far-right '1' as a corner
        if (x == _map.GetLength(1) - 1)
        {
            CornerFunction(x, y);
        }

        int[] dx = { 1, 0, 0 }; // right, up, down
        int[] dy = { 0, -1, 1 }; // right, up, down

        for (int dir = 0; dir < 3; dir++)
        {
            int newX = x + dx[dir];
            int newY = y + dy[dir];

            if (newX >= 0 && newX < _map.GetLength(1) && newY >= 0 && newY < _map.GetLength(0) && !_visited[newY, newX] && _map[newY, newX] == 1)
            {
                if (dir != prevDir && prevDir != -1)
                {
                    CornerFunction(x, y);
                }

                FindCorners(newX, newY, dir);
            }
        }
    }

    private void CornerFunction(int x, int y)
    {
        // Create an empty GameObject at the corner
        GameObject cornerObj = new GameObject($"Corner {cornerCount}")
        {
            transform =
            {
                position = new Vector3(x + leftCameraPosition + blockSize / 2, y + topCameraPosition + blockSize / 2, 10),
                parent = transform
            }
        };

        if (cornerCount == 0)
        {
            waveManager.spawn = cornerObj.transform;
            waveManager.spawn.transform.position =
                new Vector3(cornerObj.transform.position.x, cornerObj.transform.position.y, 3);
        } 
        else
        {
            waveManager.path.Add(cornerObj.transform);
        }
        
        cornerCount++;
    }
    

}
