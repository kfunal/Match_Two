
using UnityEngine;

public class GameUIManagerEndState : StateBase
{
    private GameUIManager manager;
    public GameUIManagerEndState(GameUIManager _stateMachine) : base("GameUIManagerEndState", _stateMachine) => manager = _stateMachine;
}
