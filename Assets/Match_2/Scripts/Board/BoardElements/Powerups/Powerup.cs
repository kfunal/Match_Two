using System.Collections.Generic;
using Board.Elements.MatchElements;
using Board.Manager;
using DG.Tweening;
using Helpers;
using Player.Manager;

public class Powerup : BoardElement, IClickable
{
    #region IClickable Implementation

    public ElementType ClickedElementType => elementType;
    public BoardElement Element => this;
    public BoardElementCategory ClickedElementCategory => elementCategory;
    public bool Clickable => !matching && !waitingToCreatePowerup && !poweringUp;
    public void OnClick(ElementType _clickedElementType, BoardElementCategory _category, ref List<BoardElement> _matchElements) => IsThereMatch(_clickedElementType, _category, ref _matchElements);

    public void CheckMatch(ref List<BoardElement> _matchElements)
    {
        if (_matchElements == null || _matchElements.Count == 0)
            return;

        if (_matchElements.Count == 1)
        {
            boardManager.OnPowerupPop(this);
            OnPop();
            return;
        }

        boardManager.ControlPowerupMatch(ref _matchElements);
    }

    #endregion

    public override void InitElement(int _row, int _column, BoardManager _boardManager, PlayerManager _playerManager, bool _setPosition)
    {
        base.InitElement(_row, _column, _boardManager, _playerManager, _setPosition);
        powerup = this;
        poweringUp = false;
    }

    public override void IsThereMatch(ElementType _clickedElementType, BoardElementCategory _category, ref List<BoardElement> _matchElements)
    {
        if (!matchable || matching || waitingToCreatePowerup || _category != Category || _matchElements.Contains(this))
            return;

        _matchElements.Add(this);
        BoardHelper.CheckMatchAround(row, column, _clickedElementType, Category, ref _matchElements);
    }

    public virtual void OnCreate()
    {
        ConsoleHelper.PrintLogWithColor($"On Create {row},{column} -> {elementType}", "aqua");
    }

    public virtual void OnPop()
    {
        ConsoleHelper.PrintLogWithColor($"On Pop {row},{column} -> {elementType}", "aqua");
    }

    public virtual void OnCombine(Powerup _otherElement) { }
    public override bool IsTntTarget(List<BoardElement> _elements) => matchable && !matching && !poweringUp && !waitingToCreatePowerup && !_elements.Contains(this);

    public override void Pop(bool _callCollapse, BoardElementCategory _elementCategory)
    {
        OnPop();
    }

    public void PowerupMatch(BoardElement _targetElement, BoardElement _otherElement, bool _callPowerupCombine)
    {
        matching = true;
        boxCollider.enabled = false;
        if (_targetElement == this)
        {
            waitingToCreatePowerup = true;
            return;
        }

        waitingToCreatePowerup = false;
        Tile(row, column).ClearTile();
        moving = true;
        transform.DOMove(_targetElement.transform.position, boardParameters.PowerupCreateMoveSpeed).SetEase(boardParameters.PowerupCreateMoveEase).OnComplete(() =>
        {
            if (_callPowerupCombine)
                _targetElement.PowerupCombine(_otherElement);

            ObjectPooling.ReturnPool(this);
        });
    }
}
