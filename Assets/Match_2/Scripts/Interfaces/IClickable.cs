using System.Collections.Generic;
using Board.Elements.MatchElements;

public interface IClickable
{
    ElementType ClickedElementType { get; }
    BoardElementCategory ClickedElementCategory { get; }
    BoardElement Element { get; }
    bool Clickable { get; }

    void OnClick(ElementType _clickedElementType, BoardElementCategory _category, ref List<BoardElement> _matchElements);
    void CheckMatch(ref List<BoardElement> _matchElements);
}
