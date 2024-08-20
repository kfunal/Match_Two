using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager : MonoBehaviour
{
    private StateBase currentState;

    public StateBase CurrentState => currentState;

    public virtual void Start()
    {
        currentState = InitState();

        if (currentState != null)
            currentState.EnterState();
    }

    public virtual void Update()
    {
        if (currentState != null)
            currentState.UpdateState();
    }

    public void ChangeState(StateBase _nextState)
    {
        currentState.ExitState();
        currentState = _nextState;
        currentState.EnterState();
    }

    public abstract StateBase InitState();
}