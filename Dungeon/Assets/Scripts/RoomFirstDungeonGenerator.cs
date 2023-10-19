using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField]
    [Range(0, 10)]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;

    // added this method so that the dungeon would be generated when the game starts
    private void Awake()
    {
        RunProceduralGeneration();
    }

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    public GameObject playerPrefab;
    public GameObject exitDoorPrefab;
    public GameObject enemyPrefab;
    public Vector2Int exitPosition;


    private void CreateRooms()
    {
        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomly(roomsList);
        }
        else
        {
            floor = CreateSimpleRooms(roomsList);
        }

        Debug.Log("Start: " + startPosition);
        Debug.Log("End: " + exitPosition);

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        List<Vector2Int> enemyRooms = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
            enemyRooms.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
            Debug.Log("Center: " + room.center);
        }

        Debug.Log("List Size: " + roomCenters.Count);

        Vector2Int mostLowerLeft = exitPosition;
        Vector2Int mostUpperRight = startPosition;

        // Determine start and end rooms
        foreach (var center in roomCenters)
        {
            Debug.Log("List Element: " + center);
            if (center.x < mostLowerLeft.x || center.y < mostLowerLeft.y || (center.x <= mostLowerLeft.x && center.y < mostLowerLeft.y))
            {
                mostLowerLeft = center;
                Debug.Log("Lower: " + mostLowerLeft);
            }
            else if (center.x > mostUpperRight.x || center.y > mostLowerLeft.y || (center.x >= mostUpperRight.x && center.y > mostUpperRight.y))
            {
                mostUpperRight = center;
                Debug.Log("Upper: " + mostUpperRight);
            }
        }
        enemyRooms.Remove(mostLowerLeft);
        enemyRooms.Remove(mostUpperRight);

        Debug.Log("List Size: " + enemyRooms.Count);
        foreach (var center in enemyRooms)
        {
            // Determine the random number of enemies to generate within the range of 1 to 5
            int numberOfEnemies = Random.Range(1, 6);

            // Generate the specified number of enemies around the center
            for (int i = 0; i < numberOfEnemies; i++)
            {
                // Variables to store the spawn position
                Vector3 enemySpawnPosition = Vector3.zero;
                bool positionFound = false;

                // Attempt to find a valid spawn position for the enemy
                while (!positionFound)
                {
                    // Calculate a random offset within a certain radius from the center
                    float xOffset = Random.Range(-4f, 4f); // Adjust the X offset range as needed
                    float yOffset = Random.Range(-4f, 4f); // Adjust the Y offset range as needed

                    // Calculate the potential enemy's spawn position based on the center and random offsets
                    enemySpawnPosition = new Vector3(center.x + xOffset, center.y + yOffset, 0);

                    // Check if the new spawn position overlaps with existing enemy positions
                    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(enemySpawnPosition, 0.5f); // Adjust the radius as needed

                    // If no overlap is found, set positionFound to true to exit the loop
                    if (hitColliders.Length == 0)
                    {
                        positionFound = true;
                    }
                }

                // Instantiate the enemy prefab at the calculated position
                Instantiate(enemyPrefab, enemySpawnPosition, Quaternion.identity);
            }
        }



        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        // Instantiate player prefab in the start room
        Vector3 playerSpawnPosition = new Vector3(mostLowerLeft.x, mostLowerLeft.y, 0); // Convert Vector2Int to Vector3 with Z set to 0
        // Debug.Log("Player: " + playerSpawnPosition);
        Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);


        // Find the end room and instantiate exit door prefab
        // Vector2Int endRoomCenter = FindClosestPointTo(exitPosition, roomCenters);
        Vector3 exitDungeonPosition = new Vector3(mostUpperRight.x, mostUpperRight.y, 0); // Convert Vector2Int to Vector3Int
        // Debug.Log("End: " + exitDungeonPosition);
        Instantiate(exitDoorPrefab, exitDungeonPosition, Quaternion.identity);


        tilemapVisualizer.Clear(); // this line was added so the rooms won't show up on top of base/previous map
        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
        }
        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
}
