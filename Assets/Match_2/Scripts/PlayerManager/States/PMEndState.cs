
using Player.Manager;

public class PMEndState : StateBase
{
    private PlayerManager manager;
    public PMEndState(PlayerManager _stateMachine) : base("PMEndState", _stateMachine) => manager = _stateMachine;
}
