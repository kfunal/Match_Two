using System.Collections.Generic;
using Board.Elements.MatchElements;
using Board.Manager;
using Player.Manager;
using UnityEngine;

public class Obstacle : BoardElement
{
    [SerializeField] private int health;
    [SerializeField] private bool breakableByOnlyPowerup;

    private int currentHealth;
    protected bool callCollapseOnPop = true;

    public override void InitElement(int _row, int _column, BoardManager _boardManager, PlayerManager _playerManager, bool _setPosition)
    {
        base.InitElement(_row, _column, _boardManager, _playerManager, _setPosition);
        currentHealth = health;
    }

    public override void Damage(BoardElementCategory _otherElementType)
    {
        if (breakableByOnlyPowerup && _otherElementType != BoardElementCategory.Powerup)
            return;

        currentHealth--;

        if (currentHealth > 0)
            OnObstacleHealthDecrease();
        else
            OnObstacleDestroy();
    }

    public override void Pop(bool _callCollapse, BoardElementCategory _elementCategory)
    {
        Damage(_elementCategory);
        callCollapseOnPop = _callCollapse;
    }

    public override bool IsTntTarget(List<BoardElement> _elements) => !poweringUp && !_elements.Contains(this);

    public virtual void OnObstacleHealthDecrease() { }
    public virtual void OnObstacleDestroy() { }
}
