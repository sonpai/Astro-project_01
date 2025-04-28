using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class player_movment : MonoBehaviour
{
    public Rigidbody2D rb;
    [Header("Movment")]
    public float moveSpeed = 5f;
    float horizontalMovement;

    [Header("Jumping")]
    public float jumpPower = 3f;
    private int jumpCount = 0; // Track the number of jumps
    private bool isGrounded = false; // Check if the player is on the ground
    public Transform groundCheck; // A position to check for ground (empty GameObject under the Player)
    public float groundCheckRadius = 0.2f; // Radius of the ground check circle
    public LayerMask groundLayer; // Layer for the Platforms


    void Start()
    {
        // Ensure the Rigidbody2D is assigned
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Ensure groundCheck is assigned
        if (groundCheck == null)
        {
            Debug.LogError("Ground Check Transform not assigned for Player! Please assign the GroundCheck GameObject in the Inspector.");
        }
    }

    void Update()
    {
        // Apply horizontal movement
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);

        // Check if the Player is grounded
        CheckIfGrounded();
    }

    void CheckIfGrounded()
    {
        if (groundCheck == null) return; // Avoid errors if groundCheck is not assigned

        // Use Physics2D.OverlapCircle to check if the Player is touching the Platforms
        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius, groundLayer);
        isGrounded = hit.collider != null;

        // Reset jump count when grounded
        if (isGrounded)
        {
            jumpCount = 0;
        }
    }


    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x; // Now this matches the declared variable
    }

    public void Jump(InputAction.CallbackContext context)
    {
        // Only jump when the input is performed (e.g., space key is pressed)
        if (context.performed)
        {
            // Allow a jump if the Player is grounded or has only jumped once (double jump)
          //  if (isGrounded || jumpCount < 2)
          //  {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower); // Apply jump force
             //   jumpCount++; // Increment jump count
             
          //  }
        }
    }
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}