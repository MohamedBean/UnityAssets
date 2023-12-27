using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------------------------
//----- THE STATE MANAGER; FEEL FREE TO ADD COMMONLY USED FUNCTIONS HERE -----
//----------------------------------------------------------------------------
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerManager : MonoBehaviour
{
    public State currentState;
    public DefaultState defaultState = new DefaultState();
    public DashState dashState = new DashState();
    public ParryEffectState parryEffectState = new ParryEffectState();
    public WalkingState walkingState = new WalkingState();
    // INVENTORY UI
    [HideInInspector] public InventorySystem inventory;
    [HideInInspector] public InventoryUI inventoryUI;
    // PROPERTIES
    [HideInInspector] public Rigidbody2D rb;
    public float movementSpeed = 5f;
    public float jumpPower = 15f;
    [HideInInspector] public float horizontal;
    [HideInInspector] public float timeAfterLastDash = 0.0f;
    public float dashCoolDown = 0.1f;
    public float dashSpeed = 15f;

    public int maxNumberOfJumps = 2;
    [HideInInspector] public int currentNumberOfJumps = 0;
    public int maxNumberOfDashes = 2;
    [HideInInspector] public int currentNumberOfDashes = 0;

    [HideInInspector] public bool isFacingRight = true;
    [HideInInspector] public bool grounded = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inventory = GetComponent<InventorySystem>();
        inventoryUI = GameObject.Find("InventoryPanel").GetComponent<InventoryUI>();
        currentState = defaultState;
        currentState.EnterState(this);
    }

    void Update()
    {
        currentState.UpdateState(this); 
    }

    public void SwitchState(State state)
    {
        currentState.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
        if (state == parryEffectState) 
        {
            StartCoroutine(ParryEffect());
        }
    }

    public IEnumerator ParryEffect()
    {
        Time.timeScale = 0f;
        GameObject.Find("PlayerCamera").GetComponent<PlayerCamera>().shakeAmount = 0.02f;
        GameObject.Find("PlayerCamera").GetComponent<PlayerCamera>().shake = true;
        yield return new WaitForSecondsRealtime(0.2f);
        GameObject.Find("PlayerCamera").GetComponent<PlayerCamera>().shake = false;
        Time.timeScale = 1f;
        SwitchState(defaultState);
    }

    public void Dash()
    {
        if (isFacingRight)
        {
            rb.velocity = new Vector2(dashSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(-dashSpeed, rb.velocity.y);
        }
        if (!grounded)
        {
            currentNumberOfDashes++;
        }
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        grounded = false;
        currentNumberOfJumps++;
    }

    public void StopJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
    }

    public void CheckForDirectionFacing()
    {
        if (rb.velocity.x > 0)
        {
            isFacingRight = true;
        }
        if (rb.velocity.x < 0)
        {
            isFacingRight = false;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        float playerLowestY = gameObject.transform.position.y - 0.9f;
        float floorHighestY = collision.gameObject.transform.localScale.y * 0.5f + collision.gameObject.transform.position.y;
        if (collision.gameObject.CompareTag("Floor") & playerLowestY > floorHighestY)
        {
            grounded = true;
            currentNumberOfDashes = 0;
            currentNumberOfJumps = 0;
        }
    }

    private void OnCollisionExit2D(UnityEngine.Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor") && currentNumberOfJumps == 0)
        {
            grounded = false;
            currentNumberOfJumps += 1;
        }
    }
}
