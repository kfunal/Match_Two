using UnityEngine;

public class GameUIManagerPlayState : StateBase
{
    private GameUIManager manager;
    public GameUIManagerPlayState(GameUIManager _stateMachine) : base("GameUIManagerPlayState", _stateMachine) => manager = _stateMachine;
}
