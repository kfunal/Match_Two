using System.Collections.Generic;
using Board.Elements.MatchElements;
using UnityEngine;

public class TNT : Powerup
{
    [Header("Pop Borders")]
    [SerializeField] private int horizontalPopBorder;
    [SerializeField] private int verticalPopBorder;
    [SerializeField] private int horizontalCombinePopBorder = 3;
    [SerializeField] private int verticalCombinePopBorder = 3;

    [Header("Pop Element Sizes")]
    [SerializeField] private float popParticleSize = 5;
    [SerializeField] private float combineParticleSize = 7;

    private List<BoardElement> popElements = new List<BoardElement>();
    private List<Powerup> spawnedElements = new List<Powerup>();
    private Powerup otherElement;
    private Powerup spawnElement;

    public override void OnPop()
    {
        if (poweringUp)
            return;

        poweringUp = true;
        PopTNT(horizontalPopBorder, verticalPopBorder, popParticleSize);
    }

    public override void OnCombine(Powerup _otherElement)
    {
        if (poweringUp)
            return;

        poweringUp = true;
        otherElement = _otherElement;

        if (_otherElement.PoolType == PoolType.TNT)
            PopTNT(horizontalCombinePopBorder, verticalCombinePopBorder, combineParticleSize);
        else
        {
            spriteRenderer.enabled = false;
            SpawnElements();
            PopSpawnedElements();
        }
    }

    private void PopTNT(int _horizontalBorder, int _verticalBorder, float _particleSize)
    {
        popElements.Clear();
        for (int row = Row - _verticalBorder; row <= Row + _verticalBorder; row++)
        {
            if (row >= boardManager.Height)
                break;

            for (int column = Column - _horizontalBorder; column <= Column + _horizontalBorder; column++)
            {
                if (row < 0 || column < 0)
                    continue;

                if (column >= boardManager.Width)
                    break;

                if (Tile(row, column).Empty || Tile(row, column).Element == this)
                    continue;

                if (!Tile(row, column).Element.IsTntTarget(popElements))
                    continue;

                if (!popElements.Contains(Tile(row, column).Element))
                    popElements.Add(Tile(row, column).Element);
            }
        }

        spriteRenderer.enabled = false;
        destroyParticle.ChangeStartSize(_particleSize);
        destroyParticle.PlayParticle();

        AudioManager.Instance.PlaySound(SoundName.TNT);
        for (int i = 0; i < popElements.Count; i++)
            popElements[i].Pop(true, Category);

    }

    public override void AfterDestroyParticle()
    {
        CallCollapse();
        ObjectPooling.ReturnPool(this);
    }

    private void SpawnElements()
    {
        spawnedElements.Clear();

        Spawn(row, column, PoolType.HorizontalRocket);
        ClearAndSpawn(column - 1 >= 0 && SpawnCondition(row, column - 1), row, column - 1, PoolType.VerticalRocket);
        ClearAndSpawn(column + 1 < boardManager.Width && SpawnCondition(row, column + 1), row, column + 1, PoolType.VerticalRocket);
        ClearAndSpawn(row - 1 >= 0 && SpawnCondition(row - 1, column), row - 1, column, PoolType.HorizontalRocket);
        ClearAndSpawn(row + 1 < boardManager.Height && SpawnCondition(row + 1, column), row + 1, column, PoolType.HorizontalRocket);
        Spawn(row, column, PoolType.VerticalRocket);
    }

    private bool SpawnCondition(int _row, int _column)
    {
        return Tile(_row, _column).CanContainElement && ((!Tile(_row, _column).Empty && Tile(_row, _column).Element.Category != BoardElementCategory.Obstacle) || Tile(_row, _column).Empty);
    }

    private void ClearAndSpawn(bool _condition, int _row, int _column, PoolType _type)
    {
        if (_condition)
        {
            ClearSpawnTarget(_row, _column);
            Spawn(_row, _column, _type);
        }
    }

    private void ClearSpawnTarget(int _row, int _column)
    {
        if (Tile(_row, _column).Empty)
            return;

        ObjectPooling.ReturnPool(Tile(_row, _column).Element);
        Tile(_row, _column).ClearTile();
    }

    private void Spawn(int _row, int _column, PoolType _type)
    {
        spawnElement = ObjectPooling.SpawnObject<Powerup>(_type, new Vector3(_column, _row), boardManager.ElementsParent);
        spawnElement.InitElement(_row, _column, boardManager, boardManager.PlayerManager, true);
        Tile(_row, column).SetElement(spawnElement);
        spawnedElements.Add(spawnElement);
    }

    private void PopSpawnedElements()
    {
        if (spawnedElements.Count == 0)
            return;

        AudioManager.Instance.PlaySound(SoundName.TNT);
        for (int i = 0; i < spawnedElements.Count; i++)
            spawnedElements[i].OnPop();
    }
}
