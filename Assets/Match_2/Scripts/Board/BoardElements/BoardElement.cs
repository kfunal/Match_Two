using System.Collections.Generic;
using Board.Parameters;
using Board.Manager;
using DG.Tweening;
using ObjectPool;
using Player.Manager;
using UnityEngine;

namespace Board.Elements.MatchElements
{
    public class BoardElement : PoolObject
    {
        [Header("Data")]
        [SerializeField] protected BoardElementCategory elementCategory;
        [SerializeField] protected ElementType elementType;
        [SerializeField] protected int row;
        [SerializeField] protected int column;
        [SerializeField] protected bool matchable;
        [SerializeField] protected bool collapsable;

        [Header("Components")]
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected ParticleController destroyParticle;
        [SerializeField] protected BoxCollider2D boxCollider;

        [Header("Scriptable Objects")]
        [SerializeField] protected BoardParameters boardParameters;

        protected bool matching = false;
        protected bool waitingToCreatePowerup = false;
        protected bool poweringUp = false;
        protected bool moving = false;
        protected bool destroying = false;

        protected BoardManager boardManager;
        protected PlayerManager playerManager;
        protected Powerup powerup;

        public int Row => row;
        public int Column => column;
        public bool WaitingToCreatePowerup => waitingToCreatePowerup;
        public bool Moving => moving;
        public bool Collapsable => collapsable;
        public bool Destroying => destroying;

        public ElementType ElementType => elementType;
        public BoardElementCategory Category => elementCategory;
        public BoxCollider2D BoxCollider => boxCollider;
        public Powerup Powerup => powerup;
        public bool Matching { get { return matching; } set { matching = value; } }
        public bool PoweringUp { get { return poweringUp; } set { poweringUp = value; } }

        public override void OnGetFromPool(Vector3 _position, Transform _parent)
        {
            base.OnGetFromPool(_position, _parent);
            spriteRenderer.enabled = true;
            matching = false;
            waitingToCreatePowerup = false;
            poweringUp = false;
            destroying = false;

            if (boxCollider != null)
                boxCollider.enabled = true;
        }

        public virtual void InitElement(int _row, int _column, BoardManager _boardManager, PlayerManager _playerManager, bool _setPosition)
        {
            row = _row;
            column = _column;
            boardManager = _boardManager;
            playerManager = _playerManager;

            if (_setPosition)
                transform.position = new Vector3(column, row);
        }

        public virtual void IsThereMatch(ElementType _clickedElementType, BoardElementCategory _category, ref List<BoardElement> _matchElements) { }
        public virtual void Match(MatchType _matchType, BoardElement _targetElement, bool _callCreatePowerUp, ref List<BoardElement> _matchElements) { }
        public virtual bool IsTntTarget(List<BoardElement> _elements) { return false; }
        public virtual void Pop(bool _callCollapse, BoardElementCategory _elementCategory) { }
        public void Collapse(int _row, int _column)
        {
            row = _row;
            column = _column;
        }

        public void GoToWhereBelong()
        {
            moving = true;
            transform.DOMove(new Vector3(column, row), boardParameters.MoveDuration).SetEase(boardParameters.MoveEase, boardParameters.MoveEaseAmplitude).OnComplete(() => moving = false);
        }
        protected BackgroundTile Tile(int _row, int _column) => boardManager.BackgroundTiles[_row, _column];
        public virtual void AfterDestroyParticle() => ObjectPooling.ReturnPool(this);

        protected void CallCollapse()
        {
            Tile(row, column).ClearTile();
            boardManager.CollapseColumn(row, column);
            boardManager.CallNextCollapse(true);
        }

        public void PowerupCombine(BoardElement _otherElement)
        {
            powerup.OnCombine(_otherElement.Powerup);
        }

        public virtual void Damage(BoardElementCategory _otherElementType) { }
    }
}
