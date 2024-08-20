using System.Collections.Generic;
using Board.Elements.MatchElements;
using DG.Tweening;
using Helpers;

public class MatchElement : BoardElement, IClickable
{

    #region IClickable Implementation

    public ElementType ClickedElementType => elementType;
    public BoardElement Element => this;
    public BoardElementCategory ClickedElementCategory => elementCategory;
    public bool Clickable => !matching && !waitingToCreatePowerup;
    public void OnClick(ElementType _clickedElementType, BoardElementCategory _category, ref List<BoardElement> _matchElements) => IsThereMatch(_clickedElementType, _category, ref _matchElements);
    public void CheckMatch(ref List<BoardElement> _matchElements) => boardManager.ControlMatches(ref _matchElements);

    #endregion

    public override void IsThereMatch(ElementType _clickedElementType, BoardElementCategory _category, ref List<BoardElement> _matchElements)
    {
        if (!matchable || matching || waitingToCreatePowerup || _clickedElementType != elementType || _matchElements.Contains(this))
            return;

        _matchElements.Add(this);

        BoardHelper.CheckMatchAround(row, column, _clickedElementType, Category, ref _matchElements);
    }

    public override void Match(MatchType _matchType, BoardElement _targetElement, bool _callCreatePowerUp, ref List<BoardElement> _matchElements)
    {
        matching = true;
        boxCollider.enabled = false;
        BoardHelper.CheckObstacleAround(row, column, Category);

        if (_matchType == MatchType.NormalMatch)
        {
            OnNormalMatch();
            return;
        }

        if (BoardHelper.IsPowerupMatch(_matchType))
        {
            OnPowerupMatch(_matchType, _targetElement, _callCreatePowerUp, _matchElements);
            return;
        }
    }

    public override bool IsTntTarget(List<BoardElement> _elements) => matchable && !matching && !waitingToCreatePowerup && !_elements.Contains(this);

    private void OnNormalMatch()
    {
        spriteRenderer.enabled = false;
        destroyParticle.PlayParticle();
        Tile(row, column).ClearTile();
        AudioManager.Instance.PlaySound(SoundName.ElementPop);
    }

    private void OnPowerupMatch(MatchType _matchType, BoardElement _targetElement, bool _callCreatePowerUp, List<BoardElement> _matchElements)
    {
        if (_targetElement == this)
        {
            waitingToCreatePowerup = true;
            return;
        }

        AudioManager.Instance.PlaySound(SoundName.PowerupCombine);

        waitingToCreatePowerup = false;
        Tile(row, column).ClearTile();
        moving = true;
        transform.DOMove(_targetElement.transform.position, boardParameters.PowerupCreateMoveSpeed).SetEase(boardParameters.PowerupCreateMoveEase).OnComplete(() =>
        {
            if (_callCreatePowerUp)
                boardManager.CreatePowerup(_matchType, _targetElement, _matchElements);

            ObjectPooling.ReturnPool(this);
        });
    }

    public override void Pop(bool _callCollapse, BoardElementCategory _elementCategory)
    {
        matching = true;
        OnNormalMatch();

        if (_callCollapse)
            boardManager.CollapseColumn(row, column);
    }
}
