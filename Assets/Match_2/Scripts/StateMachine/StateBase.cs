using Helpers;

[System.Serializable]
public class StateBase
{
    protected string name;
    protected StateManager machine;

    public StateBase(string _stateName, StateManager _stateMachine)
    {
        name = _stateName;
        machine = _stateMachine;
    }

    public virtual void EnterState()
    {
        ConsoleHelper.PrintLogWithColor($"{name} -> State Enter", "orange");
    }
    public virtual void UpdateState() { }
    public virtual void ExitState()
    {
        ConsoleHelper.PrintLogWithColor($"{name} -> State Exit", "orange");
    }
}
