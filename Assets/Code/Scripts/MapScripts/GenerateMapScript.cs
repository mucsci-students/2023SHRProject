using System;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

public class GenerateMapScript : MonoBehaviour
{

    #region Variables

    [Header("Object Links")]
    
    [SerializeField] private Camera currentSceneCamera;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private WaveManager waveManager;
    
    [Header("Map Settings")]
    
    [SerializeField] private bool useCameraBounds = true;
    [SerializeField] private bool useRandomMap;
    [SerializeField] private int seed = 524;
    
    [SerializeField] [Range(0, 1)] 
    private float moveRightProbability = 0.5f;
    
    [Header("Debug")]
    [SerializeField] private float leftCameraPosition;
    [SerializeField] private float rightCameraPosition;
    [SerializeField] private float topCameraPosition;
    [SerializeField] private float bottomCameraPosition;
    [SerializeField] private float blockSize;
    [SerializeField] private float yBlocks;
    [SerializeField] private float xBlocks;
    private int[,] _map;
    private Tile[,] _tileMap;
    
    private bool[,] _visited;
    private int _cornerCount;  // Add this line at the top of your class with other member variables
    
    private Random _randomNumberGenerator = new();
    
    #endregion
    
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
            Debug.Log("Camera is null on GenerateMapScript on object " + name);
        }
        
        GenerateMap();
    }

    private void CalculateMapSize()
    {
        var bottomLeftCameraPosition = useCameraBounds ? currentSceneCamera.ViewportToWorldPoint(new Vector3(0f, 0f)) : transform.parent.position;
        
        
        if (leftCameraPosition == 0f)
        {
            leftCameraPosition = bottomLeftCameraPosition.x;
        }
        if (rightCameraPosition == 0f)
        {
            rightCameraPosition = -leftCameraPosition;
        }
        if (bottomCameraPosition == 0f)
        {
            bottomCameraPosition = bottomLeftCameraPosition.y;
        }
        if (topCameraPosition == 0f)
        {
            topCameraPosition = -bottomCameraPosition;
        }

        blockSize = 1;

        if (yBlocks == 0)
        {
            yBlocks = (float)Math.Ceiling(Math.Abs(topCameraPosition - bottomCameraPosition) / blockSize);
        }

        if (xBlocks == 0)
        {
            xBlocks = (float)Math.Ceiling(Math.Abs(leftCameraPosition - rightCameraPosition) / blockSize - 2);
        }
    }
    
    public void GenerateMap()
    {
        if (!useRandomMap)
        {
            _randomNumberGenerator = new Random(seed);
        }
        DeleteMap();
        waveManager.path.Clear();
        
        CalculateMapSize();
        
        _visited = new bool[(int)yBlocks, (int)xBlocks];
        _map = new int[(int)yBlocks, (int)xBlocks];
        _tileMap = new Tile[(int)yBlocks, (int)xBlocks];
        _cornerCount = 0;
        
        GenerateRandomMapArray();
        CreateMap(_map);
        DetectCorners();
    }
    
    public void DeleteMap()
    {
        // Destroy all children of this transform using a while loop
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; --i)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    private void GenerateRandomMapArray()
    {
        var startY = _randomNumberGenerator.Next(1, (int)yBlocks - 1);

        var currPos = new Vector2(0, startY);
        SetMapValue(currPos, 1);
        currPos.x += 1;
        SetMapValue(currPos, 1);
        var lastMove = MoveDirection.Right;

        while (currPos.x < _map.GetLength(1) - 3)
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

        MoveRightOnMap(ref currPos, 2);
    }

    private MoveDirection MoveRightOnMap(ref Vector2 currentPosition, uint distance)
    {
        for (var i = 0; i < distance; i++)
        {
            if ((int)currentPosition.x == _map.GetLength(1) - 1) return MoveDirection.Right;
            currentPosition.x += 1;
            SetMapValue(currentPosition, (int)Enums.TileMode.Path);
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
            SetMapValue(currentPosition, (int)Enums.TileMode.Path);
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
            SetMapValue(currentPosition, (int)Enums.TileMode.Path);
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
    
    private void CreateMap(int[,] array)
    {
        for (int y = 0; y < array.GetLength(0); y++)
        {
            for (int x = 0; x < array.GetLength(1); x++)
            {
                if (array[y, x] == (int)Enums.TileMode.Open)
                {
                    var spawnedTile = Instantiate(tilePrefab, new Vector3(x + leftCameraPosition + blockSize / 2, (topCameraPosition - y + blockSize / 2) - blockSize, 10), quaternion.identity);
                    spawnedTile.name = $"Tile {y} {x}";

                    var isOffsetColor = (y % 2 == 0 && x % 2 != 0) || (y % 2 != 0 && x % 2 == 0);
                    spawnedTile.SetColor(isOffsetColor);
                    spawnedTile.transform.parent = transform;
                    
                    _tileMap[y, x] = spawnedTile;
                }
                else if (array[y, x] == (int)Enums.TileMode.Path)
                {
                    var spawnedRoad = Instantiate(roadPrefab, new Vector3(x + leftCameraPosition + blockSize / 2, (topCameraPosition - y + blockSize / 2) - blockSize, 10), quaternion.identity);
                    spawnedRoad.name = $"Road {y} {x}";
                    spawnedRoad.transform.parent = transform;
                }
            }
        }

        //camera.transform.position = new Vector3((float)width/2-0.5f,(float)height/2-0.5f,-10);
    }

    #region CornerCalculationForBloonPath

    private void DetectCorners()
    {
        // Start from each cell on the left border
        for (int y = 0; y < _map.GetLength(0); y++)
        {
            if (_map[y, 0] == (int)Enums.TileMode.Path)
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
        GameObject cornerObj = new GameObject($"Corner {_cornerCount}")
        {
            transform =
            {
                position = new Vector3(x + leftCameraPosition + blockSize / 2, (topCameraPosition - y + blockSize / 2) - blockSize, 10),
                parent = transform
            }
        };

        if (_cornerCount == 0)
        {
            waveManager.spawn = cornerObj.transform;
            var position = cornerObj.transform.position;
            waveManager.spawn.transform.position =
                new Vector3(position.x, position.y, 3);
        } 
        else
        {
            waveManager.path.Add(cornerObj.transform);
        }
        
        _cornerCount++;
    }
    
    #endregion

    public int[,] GetMap()
    {
        return _map;
    }
    
    public Tile[,] GetTileMap()
    {
        return _tileMap;
    }
}

