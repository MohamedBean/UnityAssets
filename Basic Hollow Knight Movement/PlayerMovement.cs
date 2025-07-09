using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    // Components required for the script to work
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Collider2D collider;
    // Inspector variables that control the different player movements
    public float movementSpeed = 5f;
    public float jumpPower = 15f;
    public float dashSpeed = 15f;
    public float dashCoolDown = 0.1f;
    public int maxNumberOfJumps = 2;
    // Variables that are not meant to be assigned but are used in the logic of the game
    [HideInInspector] public bool isDashing = false;
    [HideInInspector] public bool isFacingRight = true;
    [HideInInspector] public int currentNumberOfJumps = 0;
    [HideInInspector] public bool grounded = true;
    [HideInInspector] public float timeAfterLastDash = 0.0f;

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        // All movement logic should be disabled while dashing.
        if (!isDashing)
        {
            timeAfterLastDash += Time.deltaTime;
            AllowHorizontalMovement();
            if (Input.GetKeyDown(KeyCode.Space) && currentNumberOfJumps < maxNumberOfJumps) Jump();
            if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocityY >= 0) StopJump();
            if (Input.GetKeyDown(KeyCode.LeftShift) && timeAfterLastDash > dashCoolDown) StartCoroutine(Dash());
        }
    }

    // "HorizontalMovement" moves the character with a constant speed either left or right in order 
    // to simulate a precise horizontal movement.
    public void AllowHorizontalMovement()
    {
        rb.linearVelocityX = Input.GetAxisRaw("Horizontal") * movementSpeed;
        UpdateDirectionFacing();
    }


    // The "JumpImpulse" method makes the jump look like someone punched the player from below.
    public void JumpImpulse()
    {
        rb.AddForceY(jumpPower * rb.mass);
    }

    // The "Jump" and "StopJump" methods below are used to implement something like Hollow Knight's jump
    // mechanics, where jumping is a velocity that is stopped immediately.
    // This allows for more precise control over the jump, but doesn't look very aesthetic.
    public void Jump()
    {
        rb.linearVelocityY = jumpPower;
        currentNumberOfJumps++;
        grounded = false;
    }

    public void StopJump()
    {
        rb.linearVelocityY = 0;
    }

    public IEnumerator Dash()
    {
        float originalGravityScale = rb.gravityScale;
        isDashing = true;
        rb.linearVelocityX = isFacingRight ? dashSpeed : -dashSpeed;
        rb.linearVelocityY = 0;
        rb.gravityScale = 0;
        yield return new WaitForSecondsRealtime(0.15f);
        rb.gravityScale = originalGravityScale;
        rb.linearVelocityX = 0;
        timeAfterLastDash = 0;
        isDashing = false;
    }

    public void UpdateDirectionFacing()
    {
        if (rb.linearVelocityX > 0) isFacingRight = true;
        if (rb.linearVelocityX < 0) isFacingRight = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player is below a groundable floor or not
        float playerLowestY = collider.bounds.min.y;
        float floorHighestY = collision.collider.bounds.max.y;
        if (collision.gameObject.CompareTag("Floor") & playerLowestY >= floorHighestY)
        {
            grounded = true;
            currentNumberOfJumps = 0;
        }
    }

    private void OnCollisionExit2D(UnityEngine.Collision2D collision)
    {
        // Check if the player leaves the collision with the floor so that we can set the grounded state to false
        if (collision.gameObject.CompareTag("Floor")) grounded = false;
    }
}


