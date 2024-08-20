using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Board/Level")]
public class Level : ScriptableObject
{
    [SerializeField] private int levelNo;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int moveCount = 25;
    [SerializeField] private List<EndCondition> endConditions;
    [SerializeField] private List<BoardTile> elements;

    public int LevelNo => levelNo;
    public int Width => width;
    public int Height => height;
    public int MoveCount => moveCount;
    public List<BoardTile> Elements => elements;
    public List<EndCondition> EndConditions => endConditions;

    public BoardTile GetTile(int _row, int _column)
    {
        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Row == _row && elements[i].Column == _column)
                return elements[i];
        }

        return null;
    }

    public void ControlEndConditions(int _count, ElementType _element)
    {
        for (int i = 0; i < endConditions.Count; i++)
        {
            if (endConditions[i].ElementType == _element)
                endConditions[i].DecreaseAmount(_count);
        }
    }

    public bool EndConditionsCompleted()
    {
        for (int i = 0; i < endConditions.Count; i++)
        {
            if (!endConditions[i].Completed)
                return false;
        }

        return true;
    }

    public void SaveChanges(int _levelNo, int _width, int _height, int _moveCount, List<BoardTile> _elements)
    {
        levelNo = _levelNo;
        width = _width;
        height = _height;
        moveCount = _moveCount;

        if (elements == null)
            elements = new List<BoardTile>();

        elements.Clear();

        for (int i = 0; i < _elements.Count; i++)
        {
            BoardTile tile = _elements[i];
            elements.Add(new BoardTile(tile.Row, tile.Column, tile.ElementType));
        }
    }
}
