using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static RoomFirstDungeonGenerator;

public class Level : MonoBehaviour
{
    // Level move zoned enter, if collider is a player
    // Move game to another scene
    private void OnTriggerEnter2D(Collider2D other)
    {
        print("Trigger Entered");

        // Could use other.GetComponent<Player>() to see if the game object has a Player component
        // Tags work too. Maybe some players have different script components?
        if (other.tag == "Player")
        {
            // Player entered, so move level
            print("New level ");
            newLevel = true;
            // RoomFirstDungeonGenerator.GenerateNewDungeon();
            // GenerateNewDungeon();
        }
    }

    // void GenerateNewDungeon()
    // {
    //     // Check if the Level GameObject has a RoomFirstDungeonGenerator component
    //     RoomFirstDungeonGenerator dungeonGenerator = GetComponent<RoomFirstDungeonGenerator>();

    //     // If the RoomFirstDungeonGenerator component is null, add a new one
    //     if (dungeonGenerator == null)
    //     {
    //         gameObject.AddComponent<RoomFirstDungeonGenerator>();
    //     }

    //     // Generate the new dungeon
    //     dungeonGenerator.GenerateNewDungeon();
    // }
}
