using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//---------------------------------------------------------------------------------------
//-----THE STATE ABSTRACT CLASS; IMPLEMENT THIS WHENEVER YOU WANT TO ADD A NEW STATE-----
//---------------------------------------------------------------------------------------
public abstract class State
{
    public abstract void EnterState(PlayerManager manager);

    public abstract void UpdateState(PlayerManager manager);

    public abstract void ExitState(PlayerManager manager);

}

//-----------------------
//-----DEFAULT STATE-----
//-----------------------
public class DefaultState : State
{
    public override void EnterState(PlayerManager manager)
    {
        
    }

    public override void UpdateState(PlayerManager manager)
    {
        // HAS PLAYER STARTED MOVING
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1)
        {
            manager.SwitchState(manager.walkingState);
        }
        manager.timeAfterLastDash += Time.deltaTime;
        // OPENING/CLOSING THE INVENTORY
        if (Input.GetKeyDown(KeyCode.B))
        {
            manager.inventoryUI.ToggleInventoryDisplay();
        }
        // CONTROLS
        if (Input.GetKeyDown(KeyCode.Space) && manager.maxNumberOfJumps > manager.currentNumberOfJumps)
        {
            manager.Jump();
        }
        if (Input.GetKeyUp(KeyCode.Space) && manager.rb.velocity.y >= 0)
        {
            manager.StopJump();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && manager.timeAfterLastDash >= manager.dashCoolDown && manager.maxNumberOfDashes > manager.currentNumberOfDashes)
        {
            manager.SwitchState(manager.dashState);
        }
    }

    public override void ExitState(PlayerManager manager)
    {
        
    }
}

//-----------------------
//-----WALKING STATE-----
//-----------------------
public class WalkingState : State
{
    public override void EnterState(PlayerManager manager)
    {
        if (manager.inventoryUI.isInventoryOpen)
        {
            manager.inventoryUI.ToggleInventoryDisplay();
        }
    }

    public override void UpdateState(PlayerManager manager)
    {
        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            manager.SwitchState(manager.defaultState);
        }
        manager.timeAfterLastDash += Time.deltaTime;
        manager.rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * manager.movementSpeed, manager.rb.velocity.y);
        // FACING WHICH DIRECTION LOGIC
        manager.CheckForDirectionFacing();
        // CONTROLS
        if (Input.GetKeyDown(KeyCode.Space) && manager.maxNumberOfJumps > manager.currentNumberOfJumps)
        {
            manager.Jump();
        }
        if (Input.GetKeyUp(KeyCode.Space) && manager.rb.velocity.y >= 0)
        {
            manager.StopJump();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && manager.timeAfterLastDash >= manager.dashCoolDown && manager.maxNumberOfDashes > manager.currentNumberOfDashes)
        {
            manager.SwitchState(manager.dashState);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            manager.SwitchState(manager.parryEffectState);
        }
    }

    public override void ExitState(PlayerManager manager)
    {

    }
}

//----------------------
//------DASH STATE------
//----------------------
public class DashState : State
{
    float dashTime = 0f;
    public override void EnterState(PlayerManager manager)
    {
        dashTime = 0f;
        manager.Dash();
    }


    public override void UpdateState(PlayerManager manager)
    {
        dashTime += Time.deltaTime;
        if (dashTime >= 0.15f)
        {
            manager.SwitchState(manager.defaultState);
        }
    }

    public override void ExitState(PlayerManager manager)
    {
        manager.timeAfterLastDash = 0.0f;
        manager.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        manager.rb.velocity = new Vector2(0, manager.rb.velocity.y);
    }

}

//------------------------------
//------PARRY EFFECT STATE------
//------------------------------

public class ParryEffectState : State
{
    bool counterAttacked = false;
    public override void EnterState(PlayerManager manager)
    {
        counterAttacked = false;
    }

    public override void UpdateState(PlayerManager manager)
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            counterAttacked = true;
        }
    }

    public override void ExitState(PlayerManager manager)
    {
        if (counterAttacked)
        {
            Debug.Log("GET FUICKED");
        }
    }

}
