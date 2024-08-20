using Board.Manager;
using DG.Tweening;
using ObjectPool;
using UnityEngine;

public class DiscoBallLine : PoolObject
{
    private BoardManager boardManager;
    private int row;
    private int column;
    private Vector3 targetPosition;
    private bool playing;
    private PoolType powerupType;
    public bool Playing => playing;
    private Powerup powerup;
    private DiscoBall discoBall;

    protected override void Awake()
    {
        base.Awake();
        playing = false;
    }

    public void Init(int _row, int _column, BoardManager _boardManager, PoolType _powerupType, DiscoBall _discoBall)
    {
        row = _row;
        column = _column;
        boardManager = _boardManager;
        targetPosition = new Vector3(column, row);
        powerupType = _powerupType;
        discoBall = _discoBall;
        StartMove();
    }

    private void StartMove()
    {
        playing = true;

        transform.DOMove(targetPosition, boardManager.BoardData.LineDuration).SetEase(boardManager.BoardData.LineEase).OnComplete(() =>
        {
            if (powerupType == PoolType.None)
            {
                playing = false;
                PopElement();
                return;
            }

            SpawnPowerup();
        });
    }

    private void PopElement() => Tile(row, column).Element?.Pop(true, Tile(row, column).Element.Category);

    private void SpawnPowerup()
    {
        if (Tile(row, column).Element != null)
        {
            ObjectPooling.ReturnPool(Tile(row, column).Element);
            Tile(row, column).ClearTile();
        }

        powerup = ObjectPooling.SpawnObject<Powerup>(powerupType, transform.position, boardManager.ElementsParent);
        powerup.InitElement(row, column, boardManager, boardManager.PlayerManager, true);
        Tile(row, column).SetElement(powerup);
        discoBall.AddToSpawnedElements(powerup);
        playing = false;
    }

    private BackgroundTile Tile(int _row, int _column) => boardManager.BackgroundTiles[_row, _column];
}
