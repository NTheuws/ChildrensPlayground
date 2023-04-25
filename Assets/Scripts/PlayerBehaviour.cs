using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    private Rigidbody2D rb;
    public GameObject floor1;
    private float upwardsVelocity = 8f;
    private float horizontalSpeed = 2f;

    private Vector3 currentDirection;
    private Vector3 previousPosition;

    // Variable to prevent double jumps, jumping is only possible when the player is grounded.
    public bool isGrounded = true;
    // After crouching
    private bool isDropping = false;
    // Variables for Player movement
    public bool isMovingRight = false;
    public bool isMovingLeft = false;
    void Start()
    {
        floor1 = GameObject.FindGameObjectWithTag("1st Layer Ground");
        rb = this.GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        previousPosition = transform.position;
    }

    void Update()
    {
        // Find current direction.
        currentDirection = (transform.position - previousPosition).normalized;
        previousPosition = transform.position;

        // Turn the colliders on after you reached the top of your jump.
        if (currentDirection.y < 0 && !isDropping)
        {
            FloorCollidersToggle(false);
        }

        if (isMovingLeft)
        {
            transform.position += transform.right * -horizontalSpeed * Time.deltaTime;
        }
        else if (isMovingRight)
        {
            transform.position += transform.right * horizontalSpeed * Time.deltaTime;
        }
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
            isDropping = false;
        }
    }
    // Player has jumped.
    public void PlayerJump()
    {
        if (isGrounded)
        {
            isGrounded = false;
            // Make sure you're able to jump through the platforms.
            FloorCollidersToggle(true);
            rb.AddForce(Vector2.up * upwardsVelocity, ForceMode2D.Impulse);
        }
    }
    // Player is crouching.
    public void PlayerCrouch()
    {
        FloorCollidersToggle(true);
        isDropping = true;
    }
    // Remove the gameobject.
    public void RemovePlayer()
    {
        Destroy(gameObject);
    }

    // Turn the colliders of the floors on and off.
    // True: colliders are off.
    // False: colliders are on.
    public void FloorCollidersToggle(bool collide)
    {
        Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), floor1.GetComponent<Collider2D>(), collide);
    }

    public void MoveLeft()
    {
        StartCoroutine(ExampleCoroutine(true, false));
    }

    public void MoveRight()
    {
        StartCoroutine(ExampleCoroutine(false, true));
    }

    IEnumerator ExampleCoroutine(bool left, bool right)
    {
        isMovingLeft = left;
        isMovingRight = right;
        yield return new WaitForSeconds(1);
        isMovingLeft = false;
        isMovingRight = false;
    }
}
