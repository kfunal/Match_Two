using UnityEngine;

[System.Serializable]
public class BoardTile
{
    [SerializeField] private string name;
    [SerializeField] private int row;
    [SerializeField] private int column;
    [SerializeField] private PoolType elementType;

    public int Row => row;
    public int Column => column;
    public PoolType ElementType { get { return elementType; } set { elementType = value; } }

    public BoardTile(int _row, int _column, PoolType _elementType)
    {
        row = _row;
        column = _column;
        elementType = _elementType;

        name = $"{row},{column}";
    }
}