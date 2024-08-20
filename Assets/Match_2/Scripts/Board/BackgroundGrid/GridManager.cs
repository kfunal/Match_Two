using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private RuleTile ruleTile;

    private BoardTile tempTile;

    public void CreateGrid(int _height, int _width, Level _currentLevel)
    {
        for (int row = 0; row < _height; row++)
        {
            for (int column = 0; column < _width; column++)
            {
                tempTile = _currentLevel.GetTile(row, column);

                if (tempTile.ElementType == PoolType.None)
                    tileMap.SetTile(new Vector3Int(column, row, 0), null);
                else
                    tileMap.SetTile(new Vector3Int(column, row, 0), ruleTile);
            }
        }
    }
}
