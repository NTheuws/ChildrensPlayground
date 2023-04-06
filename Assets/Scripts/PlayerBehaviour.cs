using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    private Rigidbody2D rb;
    private float upwardsVelocity = 8f;

    // Variable to prevent double jumps, jumping is only possible when the player is grounded.
    public bool isGrounded = true;
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // For test purposes, when pressing space the player jumps.
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            PlayerJump();
        }
    }
    // When entering the collision of an object.
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player hit the ground, if yes the player is grounded.
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }

    }
    // Called when a player has jumped.
    public void PlayerJump()
    {
        if (isGrounded)
        {
            isGrounded = false;
            rb.AddForce(Vector2.up * upwardsVelocity, ForceMode2D.Impulse);
        }
    }
    // Remove the gameobject.
    public void RemovePlayer()
    {
        Destroy(gameObject);
    }
}
