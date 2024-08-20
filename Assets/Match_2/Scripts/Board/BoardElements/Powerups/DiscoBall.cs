using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Board.Elements.MatchElements;
using Helpers;
using UnityEngine;

public class DiscoBall : Powerup
{
    [Header("Disco Ball")]
    [SerializeField] private ParticleSystem TwoDiscoBallCombineParticle;
    [Range(0, 1), SerializeField] private float combineSpawnMultiplier = 0.4f;

    private List<BoardElement> elements = new List<BoardElement>();
    private List<DiscoBallLine> lines = new List<DiscoBallLine>();
    private List<BackgroundTile> tiles = new List<BackgroundTile>();
    private List<Powerup> spawnedPowerups = new List<Powerup>();

    private ElementType targetType;
    private Coroutine popActionCoroutine;
    private Coroutine powerupSpawnCoroutine;
    private DiscoBallLine line;
    private WaitForSeconds lineDelay;
    private Powerup combinedElement;
    private PoolType combineTargetType;
    private int combineTargetCount = 0;
    private int targetRow;
    private int targetColumn;
    private System.Random random;

    protected override void Awake()
    {
        base.Awake();
        lineDelay = new WaitForSeconds(boardParameters.DelayBetweenLines);
    }

    public override void OnGetFromPool(Vector3 _position, Transform _parent)
    {
        ClearCoroutines();
        base.OnGetFromPool(_position, _parent);
    }

    public override void OnReturnToPool()
    {
        ClearCoroutines();
        base.OnReturnToPool();
    }

    private void ClearCoroutines()
    {
        if (popActionCoroutine != null)
            StopCoroutine(popActionCoroutine);

        popActionCoroutine = null;

        if (powerupSpawnCoroutine != null)
            StopCoroutine(powerupSpawnCoroutine);

        powerupSpawnCoroutine = null;
    }

    public override void OnPop()
    {
        if (poweringUp)
            return;

        poweringUp = true;
        playerManager.CanPlay = false;
        boardManager.ClearCollapse();
        popActionCoroutine = StartCoroutine(PopAction());
    }

    private IEnumerator PopAction()
    {
        yield return WaitForBoard();

        targetType = UnityEngine.Random.Range(0, 4).IntToEnum<ElementType>();

        while (!BoardHelper.IsThereElement(targetType))
        {
            targetType = UnityEngine.Random.Range(0, 4).IntToEnum<ElementType>();
            yield return null;
        }

        GetElements();

        yield return SpawnLines(PoolType.None, elements, null);

        CallCollapse();
        boardManager.PlayerPlayControl();
        popActionCoroutine = null;
        ObjectPooling.ReturnPool(this);
    }

    private void GetElements()
    {
        elements.Clear();
        lines.Clear();

        for (int row = 0; row < boardManager.Height; row++)
        {
            for (int column = 0; column < boardManager.Width; column++)
            {
                if (Tile(row, column).Empty)
                    continue;

                if (Tile(row, column).Element.ElementType != targetType)
                    continue;

                if (elements.Contains(Tile(row, column).Element))
                    continue;

                elements.Add(Tile(row, column).Element);
            }
        }
    }

    private bool IsThereDiscoBallLinePlaying()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Playing)
                return true;
        }

        return false;
    }

    public override void OnCombine(Powerup _otherElement)
    {
        if (poweringUp)
            return;

        poweringUp = true;
        playerManager.CanPlay = false;
        spriteRenderer.enabled = false;
        combinedElement = _otherElement;
        if (_otherElement.ElementType == ElementType.DiscoBall)
        {
            PopAllElementsOnBoard();
            CallCollapse();
            playerManager.CanPlay = true;
            // ObjectPooling.ReturnPool(this);
        }
        else
            powerupSpawnCoroutine = StartCoroutine(SpawnPowerups());
    }

    private IEnumerator SpawnPowerups()
    {
        yield return WaitForBoard();
        spawnedPowerups.Clear();
        GetTargetTiles();
        yield return SpawnLines(combinedElement.PoolType, null, tiles);
        Tile(row, column).ClearTile();
        PopSpawnedPowerups();
        yield return lineDelay;
        boardManager.CollapseColumn(row, column);
        boardManager.CallNextCollapse(true);
        boardManager.PlayerPlayControl();
        spawnedPowerups = null;
        ObjectPooling.ReturnPool(this);
    }

    private void GetTargetTiles()
    {
        tiles.Clear();
        BoardHelper.DiscoBallCombineTargets(ref tiles);

        combineTargetCount = tiles.Count;
        combineTargetCount = Mathf.RoundToInt(combineTargetCount * combineSpawnMultiplier);

        //Guid is used for ther random class to have different seed every time so it can pick different random by every choose
        random = new System.Random(Guid.NewGuid().GetHashCode());
        tiles = tiles.OrderBy(x => random.Next()).Take(combineTargetCount).ToList();
    }

    private IEnumerator WaitForBoard()
    {
        while (boardManager.Collapsing)
            yield return null;

        while (BoardHelper.IsThereElementMoving())
            yield return null;
    }

    private IEnumerator SpawnLines(PoolType _type, List<BoardElement> _elements, List<BackgroundTile> _tiles)
    {
        lines.Clear();
        for (int index = 0; index < (_elements != null ? _elements.Count : _tiles.Count); index++)
        {
            targetRow = _elements != null ? elements[index].Row : _tiles[index].Row;
            targetColumn = _elements != null ? elements[index].Column : _tiles[index].Column;
            combineTargetType = BoardHelper.GetTargetForDiscoBallCombine(_type);
            AudioManager.Instance.PlaySound(SoundName.DiscoBall);
            line = ObjectPooling.SpawnObject<DiscoBallLine>(PoolType.DiscoBallLine, transform.position, boardManager.ElementsParent);
            line.Init(targetRow, targetColumn, boardManager, combineTargetType, this);
            lines.Add(line);
            yield return lineDelay;
        }

        while (IsThereDiscoBallLinePlaying())
            yield return null;

        ObjectPooling.ResetPool(PoolType.DiscoBallLine);
    }

    private void PopSpawnedPowerups()
    {
        boardManager.CollapseColumn(combinedElement.Row, combinedElement.Column);

        if (spawnedPowerups == null || spawnedPowerups.Count == 0)
            return;

        for (int i = 0; i < spawnedPowerups.Count; i++)
        {
            spawnedPowerups[i].OnPop();
        }
    }

    public void AddToSpawnedElements(Powerup _powerup)
    {
        if (!spawnedPowerups.Contains(_powerup))
            spawnedPowerups.Add(_powerup);
    }

    public void PopAllElementsOnBoard()
    {
        TwoDiscoBallCombineParticle.Play();
        AudioManager.Instance.PlaySound(SoundName.DiscoBallCombine);
        for (int row = 0; row < boardManager.Height; row++)
        {
            for (int column = 0; column < boardManager.Width; column++)
            {
                Tile(row, column).Element?.Pop(true, Category);
            }
        }
    }
}
