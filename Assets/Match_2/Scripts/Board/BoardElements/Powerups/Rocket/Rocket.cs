using DG.Tweening;
using UnityEngine;

public class Rocket : Powerup
{
    [SerializeField] private SpriteRenderer firstPiece;
    [SerializeField] private SpriteRenderer secondPiece;

    protected bool firstPieceMoving = false;
    protected bool secondPieceMoving = false;

    private Vector3 firstPieceTargetPosition;
    private Vector3 secondPieceTargetPosition;

    private Powerup firstRocket;
    private Powerup secondRocket;

    public override void OnReturnToPool()
    {
        PieceSpriteRenderers(false);
        PieceDoKills();
        base.OnReturnToPool();
    }

    public override void OnCombine(Powerup _otherElement)
    {
        if (poweringUp)
            return;

        poweringUp = true;

        AudioManager.Instance.PlaySound(SoundName.Rocket);
        firstRocket = ObjectPooling.SpawnObject<Powerup>(PoolType.HorizontalRocket, transform.position, boardManager.ElementsParent);
        firstRocket.InitElement(row, column, boardManager, boardManager.PlayerManager, true);
        Tile(row, column).SetElement(firstRocket);

        secondRocket = ObjectPooling.SpawnObject<Powerup>(PoolType.VerticalRocket, transform.position, boardManager.ElementsParent);
        secondRocket.InitElement(row, column, boardManager, boardManager.PlayerManager, true);

        boardManager.CollapseColumn(_otherElement.Row, _otherElement.Column);
        firstRocket.OnPop();
        secondRocket.OnPop();

        ObjectPooling.ReturnPool(this);
    }

    public override void OnPop()
    {
        base.OnPop();
        AudioManager.Instance.PlaySound(SoundName.Rocket);
    }

    protected void ResetPieceTransforms()
    {
        firstPiece.transform.position = transform.position;
        secondPiece.transform.position = transform.position;
    }

    protected void ControlFirstPiece(bool _isHorizontal)
    {
        firstPieceMoving = true;
        firstPieceTargetPosition = _isHorizontal ? new Vector3(-1, firstPiece.transform.position.y) : new Vector3(firstPiece.transform.position.x, -1);
        firstPiece.transform.DOMove(firstPieceTargetPosition, boardParameters.PieceDuration).SetEase(boardParameters.PieceEase).OnUpdate(() =>
        {
            FirstPiecePopControl(_isHorizontal);
        }).OnComplete(() =>
        {
            firstPieceMoving = false;
            firstPiece.enabled = false;
            AfterPiecesMove();
        });
    }

    protected void ControlSecondPiece(bool _isHorizontal)
    {
        secondPieceMoving = true;
        secondPieceTargetPosition = _isHorizontal ? new Vector3(boardManager.Width, secondPiece.transform.position.y) : new Vector3(secondPiece.transform.position.x, boardManager.Height);
        secondPiece.transform.DOMove(secondPieceTargetPosition, boardParameters.PieceDuration).SetEase(boardParameters.PieceEase).OnUpdate(() =>
        {
            SecondPiecePopControl(_isHorizontal);
        }).OnComplete(() =>
        {
            secondPieceMoving = false;
            secondPiece.enabled = false;
            AfterPiecesMove();
        });
    }

    private void FirstPiecePopControl(bool _isHorizontal)
    {
        if (_isHorizontal && firstPiece.transform.position.x >= 0)
            Tile((int)firstPiece.transform.position.y, (int)firstPiece.transform.position.x).Element?.Pop(true, Category);

        if (!_isHorizontal && firstPiece.transform.position.y >= 0)
            Tile((int)firstPiece.transform.position.y, (int)firstPiece.transform.position.x).Element?.Pop(true, Category);
    }

    private void SecondPiecePopControl(bool _isHorizontal)
    {
        if (_isHorizontal && secondPiece.transform.position.x < boardManager.Width)
            Tile((int)secondPiece.transform.position.y, (int)secondPiece.transform.position.x).Element?.Pop(true, Category);

        if (!_isHorizontal && secondPiece.transform.position.y < boardManager.Height)
            Tile((int)secondPiece.transform.position.y, (int)secondPiece.transform.position.x).Element?.Pop(true, Category);
    }


    protected void PieceDoKills()
    {
        firstPiece.transform.DOKill();
        secondPiece.transform.DOKill();
    }

    protected void PieceSpriteRenderers(bool _status)
    {
        firstPiece.enabled = _status;
        secondPiece.enabled = _status;
    }

    private void AfterPiecesMove()
    {
        if (firstPieceMoving || secondPieceMoving)
            return;

        if (!Tile(row, column).Empty && Tile(row, column).Element == this)
            CallCollapse();

        ObjectPooling.ReturnPool(this);
    }
}
