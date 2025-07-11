using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------------------------
//----- THE STATE MANAGER; FEEL FREE TO ADD COMMONLY USED FUNCTIONS HERE -----
//----------------------------------------------------------------------------
public class StateManager : MonoBehaviour
{
    private State currentState;
    private ExampleState1 exampleState = new ExampleState1();

    void Start()
    {
        currentState = exampleState;
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
    }

    public void GetState()
    {
        return currentState;
    }
}
