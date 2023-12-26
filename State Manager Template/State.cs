using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//---------------------------------------------------------------------------------------
//-----THE STATE ABSTRACT CLASS; IMPLEMENT THIS WHENEVER YOU WANT TO ADD A NEW STATE-----
//---------------------------------------------------------------------------------------
public abstract class State
{
    public abstract void EnterState(StateManager manager);

    public abstract void UpdateState(StateManager manager);

    public abstract void ExitState(StateManager manager);

}
//---------------------------------------
//-----THE IMPLEMENTED STATE CLASSES-----
//---------------------------------------
public class ExampleState1 : State
{
    public override void EnterState(StateManager manager)
    {
        Debug.Log("Entered stationary!");
    }

    public override void UpdateState(StateManager manager)
    {
        Debug.Log("Updating stationary");
    }

    public override void ExitState(StateManager manager)
    {
        Debug.Log("Stopped stationary!");
    }
}