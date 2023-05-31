using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    // JumpEvent.
    public delegate void DisableHalfBlock();
    public static event DisableHalfBlock disableHalfBlock;

    private Rigidbody2D rb;
    private GameObject floor1;
    private float upwardsVelocity = 8f;
    private float horizontalSpeed = 1.5f;
    private float idleSpeed = 2f;

    private Vector3 currentDirection;
    private Vector3 previousPosition;

    // Variable to prevent double jumps, jumping is only possible when the player is grounded.
    public bool isGrounded = true;
    // After crouching
    public bool isDropping = false;
    // Variables for Player movement.
    public bool isMovingRight = false;
    public bool isMovingLeft = false;

    // Ignore tutorial collisions.
    public bool tutorialCollisions;

    public bool falling = false;
    public bool jumped = false;

    void Start()
    {
        if (tutorialCollisions)
        {
            floor1 = GameObject.FindGameObjectWithTag("1st Layer Ground");
        }
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
            if (tutorialCollisions)
            {
                FloorCollidersToggle(false);
            }
        }

        if (isMovingLeft)
        {
            if (tutorialCollisions)
            {
                transform.position += transform.right * -horizontalSpeed * Time.deltaTime;
            }
            else
            {
                transform.position += transform.right * -(horizontalSpeed / 2) * Time.deltaTime;
            }
        }
        else if (isMovingRight)
        {
            if (tutorialCollisions)
            {
                transform.position += transform.right * horizontalSpeed * Time.deltaTime;
            }
            else
            {
                transform.position += transform.right * (horizontalSpeed + idleSpeed) * Time.deltaTime;
            }
        }
        // If not in tutorial make sure the character stays neutral when the player is not moving.
        else if (!tutorialCollisions)
        {
            transform.position += transform.right * idleSpeed * Time.deltaTime;
        }
        // For test purposes, when pressing space the player jumps.
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            PlayerJump();
        }

        // Check player movement.
        Vector2 movement = rb.velocity;
        if (movement.y <0 && !falling) 
        {
            jumped = false;
            falling = true;        
        }
        else if(movement.y > 0)
        {
            falling = false;
        }

        // the player is grounded when the y-axis isnt moving.
        if (movement.y == 0 && !jumped)
        {
            isGrounded = true;
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
            if (tutorialCollisions)
            {
                FloorCollidersToggle(true);
            }
            jumped = true;

            // Invoke event.
            if (disableHalfBlock != null)
            {
                disableHalfBlock();
            }
            // Move upwards
            // In the tutorial everything is larger so the jump will have to be as well.
            if (tutorialCollisions)
            {
                this.rb.AddForce(Vector2.up * upwardsVelocity, ForceMode2D.Impulse);
            }
            // The actual game is small and the player wont just as high.
            else
            {
                this.rb.AddForce(Vector2.up * upwardsVelocity/4f, ForceMode2D.Impulse);
            }
        }
    }
    // Player is crouching.
    public void PlayerCrouch()
    {
        if (tutorialCollisions)
        {
            FloorCollidersToggle(true);
        }
        // Invoke event.
        if (disableHalfBlock != null)
        {
            disableHalfBlock();
        }

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
        StartCoroutine(WaitForMovement(true, false));
    }

    public void MoveRight()
    {
        StartCoroutine(WaitForMovement(false, true));
    }

    IEnumerator WaitForMovement(bool left, bool right)
    {
        isMovingLeft = left;
        isMovingRight = right;
        yield return new WaitForSeconds(1);
        isMovingLeft = false;
        isMovingRight = false;
    }
}
