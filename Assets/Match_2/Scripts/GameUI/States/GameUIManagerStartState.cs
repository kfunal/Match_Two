using UnityEngine;

public class GameUIManagerStartState : StateBase
{
    private GameUIManager manager;
    private EndCondition endCondition;

    private Level currentLevel => manager.GameManager.CurrentLevel;

    public GameUIManagerStartState(GameUIManager _stateMachine) : base("GameUIManagerStartState", _stateMachine) => manager = _stateMachine;

    public override void EnterState()
    {
        for (int i = 0; i < currentLevel.EndConditions.Count; i++)
            manager.EndConditionElements[i].Init(currentLevel.EndConditions[i]);

        manager.MoveCountText.SetText(currentLevel.MoveCount.ToString());

        manager.ChangeState(manager.PlayState);
    }

}
