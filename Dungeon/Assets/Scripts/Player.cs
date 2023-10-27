using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Adjust the speed as needed
    private Rigidbody2D rb;
    public GameObject exitPrefab;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Get input from the player
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector2 movement = new Vector2(moveX, moveY).normalized;

        // Apply movement
        rb.velocity = movement * moveSpeed;

        // // Check if the player is overlapping with the exit prefab
        // Collider2D exitCollider = Physics2D.OverlapCircle(transform.position, 0.5f, exitPrefab.layer);

        // // Log the exit collider to the console
        // Debug.Log(exitCollider);

        // if (exitCollider != null)
        // {
        //     // Generate a new dungeon
        //     GenerateNewDungeon();
        // }
    }
}
