using Board.Elements.MatchElements;
using Board.Manager;

[System.Serializable]
public class BackgroundTile
{
    private int row;
    private int column;
    private bool canContainElement;
    private BoardManager boardManager;
    private BoardElement boardElement;

    public bool Empty => boardElement == null;
    public bool CanContainElement => canContainElement;
    public int Row => row;
    public int Column => column;
    public BoardElement Element => boardElement;

    public bool DiscoBallTarget
    {
        get
        {
            if (!canContainElement)
                return false;

            if (boardElement != null && boardElement.Category == BoardElementCategory.Powerup)
                return false;

            if (boardElement != null && boardElement.Category == BoardElementCategory.Obstacle)
                return false;

            if (boardElement != null && boardElement.Category == BoardElementCategory.Property)
                return false;

            return true;
        }
    }

    public BackgroundTile(int _row, int _column, BoardElement _element, BoardManager _boardManager, bool _canContainElement)
    {
        row = _row;
        column = _column;
        boardElement = _element;
        boardManager = _boardManager;
        canContainElement = _canContainElement;
    }

    public void SetElement(BoardElement _element)
    {
        boardElement = _element;
    }

    public void ClearTile() => boardElement = null;
}
