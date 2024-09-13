using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine:MonoBehaviour
{
    [SerializeField] private State currentState;

    public State GetCurrentState()
    {
        return currentState;
    }

    public void InitState(State state)
    {
        currentState = state;
        currentState.EnterState();
    }

    public void ChangeState(State state)
    {
        currentState?.ExitState();
        currentState = state;
        currentState.EnterState();
    }

    public void StateFrameUpdate()
    {
        currentState?.FrameUpdate();
    }

    public void StatePhysicsUpdate()
    {
        currentState.PhysicsUpdate();
    }
}
