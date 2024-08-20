using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUIManager : StateManager
{
    #region States
    private GameUIManagerStartState startState;
    private GameUIManagerPlayState playState;
    private GameUIManagerEndState endState;
    #endregion

    #region Inspector

    #region Props

    public GameUIManagerPlayState PlayState => playState;
    public GameUIManagerEndState EndState => endState;

    #endregion

    [field: Header("End Condition")]
    [field: SerializeField] public List<EndConditionUIElement> EndConditionElements { get; private set; }

    [field: Header("Move Count")]
    [field: SerializeField] public TextMeshProUGUI MoveCountText { get; private set; }

    [field: Header("Script References")]
    [field: SerializeField] public GameManager GameManager { get; private set; }

    #endregion

    private void Awake()
    {
        InitStates();
    }

    private void InitStates()
    {
        startState = new GameUIManagerStartState(this);
        playState = new GameUIManagerPlayState(this);
        endState = new GameUIManagerEndState(this);
    }

    public void UpdateMoveCountText(int _newAmount)
    {
        if (_newAmount < 0)
            return;

        MoveCountText.SetText(_newAmount.ToString());
    }

    public void ControlEndConditions(Level _currentLevel)
    {
        for (int i = 0; i < _currentLevel.EndConditions.Count; i++)
            EndConditionElements[i].UpdateAmount(_currentLevel.EndConditions[i].CurrentAmount);
    }

    public override StateBase InitState()
    {
        return startState;
    }
}
