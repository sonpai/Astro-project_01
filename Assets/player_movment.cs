using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Represents the player character in the game.
/// Manages player movement, jumping, interactions, and scene transitions.
/// </summary>
public class player_movment : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 1.5f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private GameObject interactionText;
    [SerializeField] private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private float horizontalMovement;

    [Header("Jumping")]
    [SerializeField] private float jumpPower = 3f;
    private int jumpCount = 0; // Tracks the number of jumps performed
    private bool isGrounded = false; // Determines if the player is on the ground
    [SerializeField] private Transform groundCheck; // Position used to check for ground collisions
    [SerializeField] private float groundCheckRadius = 0.2f; // Radius of the ground-check sphere
    [SerializeField] private LayerMask groundLayer; // Layer mask used to identify ground objects

    /// <summary>
    /// Gets or sets the player's movement speed.
    /// </summary>
    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = Mathf.Max(0, value); } // Ensures speed is non-negative
    }

    /// <summary>
    /// Gets or sets the player's jump power.
    /// </summary>
    public float JumpPower
    {
        get { return jumpPower; }
        set { jumpPower = Mathf.Max(0, value); } // Ensures jump power is non-negative
    }

    /// <summary>
    /// Gets whether the player is grounded.
    /// </summary>
    public bool IsGrounded
    {
        get { return isGrounded; }
    }

    /// <summary>
    /// Initializes necessary components and checks for required assignments.
    /// </summary>
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

    /// <summary>
    /// Updates player movement and checks if the player is grounded.
    /// </summary>
    void Update()
    {
        // Apply horizontal movement
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);

        // Check if the Player is grounded
        CheckIfGrounded();
    }

    /// <summary>
    /// Checks whether the player is currently on the ground.
    /// Resets the jump count when grounded.
    /// </summary>
    void CheckIfGrounded()
    {
        if (groundCheck == null) return; // Avoid errors if groundCheck is not assigned

        // Perform a raycast to detect ground
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius, groundLayer);
        isGrounded = hit.collider != null;

        // Reset jump count when grounded
        if (isGrounded)
        {
            jumpCount = 0;
        }
    }

    /// <summary>
    /// Handles player movement based on input.
    /// </summary>
    /// <param name="context">The input context containing movement data.</param>
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x; // Update horizontal movement value
    }

    /// <summary>
    /// Handles player jumping based on input.
    /// </summary>
    /// <param name="context">The input context containing jump data.</param>
    public void Jump(InputAction.CallbackContext context)
    {
        // Only jump when the input is performed (e.g., space key is pressed)
        if (context.performed)
        {
            // Apply jump force
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        }
    }

    /// <summary>
    /// Draws a visual representation of the ground check area in the editor.
    /// </summary>
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    /// <summary>
    /// Handles player interaction when entering a trigger collider.
    /// </summary>
    /// <param name="other">The collider of the object the player interacted with.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("blacksmith"))
        {
            interactionText.SetActive(true);
        }
    }

    /// <summary>
    /// Handles player interaction when exiting a trigger collider.
    /// </summary>
    /// <param name="other">The collider of the object the player exited.</param>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("blacksmith"))
        {
            interactionText.SetActive(false);
        }
    }

    /// <summary>
    /// Handles interaction input and checks for nearby interactable objects.
    /// Transitions to a new scene if interacting with a blacksmith.
    /// </summary>
    /// <param name="context">The input context containing interaction data.</param>
    public void Interact(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            // Check for nearby objects
            Collider2D nearbyObject = Physics2D.OverlapCircle(transform.position, 1f, LayerMask.GetMask("Interactable"));

            if (nearbyObject != null)
            {
                if (nearbyObject.CompareTag("blacksmith"))
                {
                    Debug.Log("Interacting with the blacksmith!");
                    SceneManager.LoadScene("shop");
                }
                else
                {
                    Debug.LogWarning($"Nearby object found, but it's not a blacksmith: {nearbyObject.name}");
                }
            }
            else
            {
                Debug.LogWarning("No interactable object found nearby.");
            }
        }
    }
}
