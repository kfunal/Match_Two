using System;
using System.Collections;
using System.Collections.Generic;
using Board.Parameters;
using Board.Elements.MatchElements;
using Helpers;
using ObjectPool;
using Player.Manager;
using UnityEngine;
using System.Linq;

namespace Board.Manager
{
    public class BoardManager : MonoBehaviour
    {
        #region Inspector

        [Header("Scriptable Objects")]
        [SerializeField] private BoardParameters boardData;

        [Header("Board")]
        [SerializeField] private Transform elementsParent;

        [Header("Grid")]
        [SerializeField] private GridManager gridManager;

        [Header("Scripts")]
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private GameManager gameManager;
        [Header("Board Camera Borders")]
        [SerializeField] private Transform LeftBottomBorder;
        [SerializeField] private Transform LeftTopBorder;
        [SerializeField] private Transform RightBottomBorder;
        [SerializeField] private Transform RightTopBorder;

        [Header("Board Mask")]
        [SerializeField] private Transform boardMask;

        #endregion

        #region Variables
        private bool collapsing = false;
        private bool refilling = false;
        private int refillIndex = 0;
        private PoolType randomElement;
        private PoolType powerupType;
        private BoardElement tempElement;
        private BoardElement boardElement;
        private Powerup tempPowerup;
        private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        private MatchType tempMatchType;
        private BoardTile tempTile;
        private List<BoardElement> tempElementList;
        private List<BoardElement> powerupElements = new List<BoardElement>();
        private BackgroundTile[,] backgroundTiles;

        private Queue<Action> collapseQueue = new Queue<Action>();
        private Queue<Action> refillQueue = new Queue<Action>();

        #endregion

        #region Props

        public int Width => currentLevel.Width;
        public int Height => currentLevel.Height;
        public bool Collapsing => collapsing;
        public PlayerManager PlayerManager => playerManager;
        public Transform ElementsParent => elementsParent;
        public BackgroundTile[,] BackgroundTiles => backgroundTiles;
        public BoardParameters BoardData => boardData;
        private ObjectPooling objectPooling => ObjectPooling.Instance;
        private Level currentLevel => gameManager.CurrentLevel;

        #endregion

        #region Board Events
        public Action<List<BoardElement>> OnNormalElementMatch;
        public Action<Powerup> OnPowerupPop;
        public Action<BoardElement, BoardElement> OnPowerupCombine;
        public Action<Obstacle> OnObstacleDestroy;
        #endregion

        private void Awake()
        {
            BoardHelper.BoardManager = this;
        }

        public void ControlMatches(ref List<BoardElement> _elements)
        {
            tempMatchType = BoardHelper.Match(_elements.Count);

            if (tempMatchType == MatchType.None)
                return;

            tempElementList = _elements;
            OnNormalElementMatch?.Invoke(_elements);

            for (int i = 0; i < tempElementList.Count; i++)
                tempElementList[i].Match(tempMatchType, tempElementList[0], i == tempElementList.Count - 1, ref _elements);

            if (tempMatchType == MatchType.NormalMatch)
            {
                for (int i = 0; i < tempElementList.Count; i++)
                    CollapseColumn(tempElementList[i].Row, tempElementList[i].Column);
            }

            CallNextCollapse(false);
        }

        public void ControlPowerupMatch(ref List<BoardElement> _elements)
        {
            _elements = _elements.OrderBy(x => x.ElementType).ToList();

            for (int i = 0; i < _elements.Count; i++)
                _elements[i].Powerup.PowerupMatch(_elements[0], _elements[1], true);

            AudioManager.Instance.PlaySound(SoundName.PowerupCombine);
            OnPowerupCombine?.Invoke(_elements[0], _elements[1]);
        }

        public void CollapseColumn(int _row, int _column)
        {
            if (!collapseQueue.Contains(() => ControlCollapse(_row, _column)))
                collapseQueue.Enqueue(() => ControlCollapse(_row, _column));
        }

        private void RefillColumn(int _row, int _column)
        {
            if (!refillQueue.Contains(() => ControlRefill(_row, _column)))
                refillQueue.Enqueue(() => ControlRefill(_row, _column));
        }

        private void ControlCollapse(int _row, int _column)
        {
            if (!CanCollapse(_row, _column))
            {
                CollapseColumn(_row, _column);
                return;
            }

            collapsing = true;

            for (int row = 0; row < Height; row++)
            {
                if (!Tile(row, _column).Empty || !Tile(row, _column).CanContainElement || BoardHelper.IsThereNotCollapsableElementAbove(row, _column))
                    continue;

                tempElement = BoardHelper.FindFirstElementBelow(row, _column);

                if (tempElement != null)
                {
                    Tile(tempElement.Row, tempElement.Column).ClearTile();
                    tempElement.Collapse(row, _column);
                    Tile(row, _column).SetElement(tempElement);
                }
            }

            collapsing = false;
            RefillColumn(_row, _column);
            CallNextCollapse(true);
        }

        public void CallNextCollapse(bool _refill)
        {
            if (collapsing)
                return;

            if (collapseQueue.Count > 0)
                collapseQueue.Dequeue()();
            else if (_refill)
                CallNextRefill();
        }

        private void CallNextRefill()
        {
            if (refilling)
                return;

            if (refillQueue.Count > 0)
                refillQueue.Dequeue()();
        }

        public void ClearCollapse() => collapseQueue.Clear();

        private void ControlRefill(int _row, int _column)
        {
            if (refilling || BoardHelper.IsThereElementWaitingToCreatePowerupOnColumn(_column))
            {
                RefillColumn(_row, _column);
                return;
            }

            refilling = true;
            refillIndex = 0;

            BoardHelper.IterateColumn(_column, (_tile) =>
            {
                if (CanRefill(_tile))
                {
                    randomElement = UnityEngine.Random.Range(0, 4).IntToEnum<PoolType>();
                    tempElement = objectPooling.SpawnObject<BoardElement>(randomElement, new Vector3(_tile.Column, Height + refillIndex), elementsParent);
                    _tile.SetElement(tempElement);
                    tempElement.InitElement(_tile.Row, _tile.Column, this, playerManager, false);
                    refillIndex++;
                }
            });

            BoardHelper.IterateColumn(_column, (_tile) => _tile.Element?.GoToWhereBelong());

            refilling = false;
            CallNextRefill();
        }

        public void CreatePowerup(MatchType _matchType, BoardElement _targetElement, List<BoardElement> _matchElements)
        {
            powerupType = BoardHelper.PowerupTypeByMatchType(_matchType);

            if (powerupType == PoolType.None)
                return;

            powerupElements.Clear();

            for (int i = 0; i < _matchElements.Count; i++)
                powerupElements.Add(_matchElements[i]);

            tempPowerup = objectPooling.SpawnObject<Powerup>(powerupType, _targetElement.transform.position, elementsParent);
            tempPowerup.InitElement(_targetElement.Row, _targetElement.Column, this, playerManager, true);
            Tile(_targetElement.Row, _targetElement.Column).SetElement(tempPowerup);
            tempPowerup.OnCreate();
            objectPooling.ReturnPool(_targetElement);

            if (powerupElements == null || powerupElements.Count == 0)
                return;

            for (int i = 0; i < powerupElements.Count; i++)
                CollapseColumn(powerupElements[i].Row, powerupElements[i].Column);

            CallNextCollapse(true);
        }

        public void CreateBoard()
        {
            gridManager.CreateGrid(Height, Width, currentLevel);
            AdjustCameraBorders();
            AdjustBoardMask();
            StartCoroutine(CreateTiles());
            collapsing = false;
        }

        private void AdjustBoardMask()
        {
            boardMask.position = new Vector3((Width / 2f) - .5f, (Height / 2f) - .5f);
            boardMask.localScale = new Vector3(Width, Height);
        }

        private void AdjustCameraBorders()
        {
            LeftBottomBorder.position = Vector3.zero;
            LeftTopBorder.position = new Vector3(0, Height - 1);
            RightBottomBorder.position = new Vector3(Width - 1, 0);
            RightTopBorder.position = new Vector3(Width - 1, Height - 1);
        }

        private IEnumerator CreateTiles()
        {
            backgroundTiles = new BackgroundTile[Height, Width];

            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    tempTile = currentLevel.GetTile(row, column);

                    if (tempTile == null)
                        continue;

                    boardElement = tempTile.ElementType == PoolType.None ? null : CreateElement(row, column, tempTile.ElementType, true);

                    // boardElement = CreateElement(row, column, tempTile.ElementType, true);
                    backgroundTiles[row, column] = new BackgroundTile(row, column, boardElement, this, tempTile.ElementType != PoolType.None);

                    yield return waitForEndOfFrame;
                }
            }

            yield return waitForEndOfFrame;
            playerManager.ChangeState(playerManager.PlayState);
        }

        private BoardElement CreateElement(int _row, int _column, PoolType _elementType, bool _setPosition)
        {
            tempElement = objectPooling.SpawnObject<BoardElement>(_elementType, new Vector3(_column, _row), elementsParent);
            tempElement.InitElement(_row, _column, this, playerManager, _setPosition);

            return tempElement;
        }

        private bool CanCollapse(int _row, int _column)
        {
            return !collapsing && !BoardHelper.IsThereElementWaitingToCreatePowerupOnColumn(_column)
            && !BoardHelper.IsThereVerticalRocketOnColumn(_column) && !BoardHelper.IsThereHorizontalRocketOnBoard() && !BoardHelper.IsTherePowerupPoweringUp() && !BoardHelper.IsThereObstacleDestroying(_column);
        }

        private bool CanRefill(BackgroundTile _tile)
        {
            return _tile.Empty && _tile.CanContainElement && !BoardHelper.IsThereNotCollapsableElementAbove(_tile.Row, _tile.Column);
        }

        private BackgroundTile Tile(int _row, int _column) => backgroundTiles[_row, _column];

        public void PlayerPlayControl()
        {
            if (gameManager.GameFinished)
                return;

            playerManager.CanPlay = true;
        }
    }
}
