using System;
using System.Collections.Generic;
using Board.Parameters;
using Board.Elements.MatchElements;
using Board.Manager;

namespace Helpers
{
    public static class BoardHelper
    {
        public static BoardManager BoardManager;
        private static BoardParameters boardData => BoardManager.BoardData;

        public static void CheckMatchAround(int _row, int _column, ElementType _clickedElementType, BoardElementCategory _category, ref List<BoardElement> _matchElements)
        {
            if (_row - 1 >= 0 && Tile(_row - 1, _column).Element != null)
                Tile(_row - 1, _column).Element.IsThereMatch(_clickedElementType, _category, ref _matchElements);

            if (_row + 1 < BoardManager.Height && Tile(_row + 1, _column).Element != null)
                Tile(_row + 1, _column).Element.IsThereMatch(_clickedElementType, _category, ref _matchElements);

            if (_column - 1 >= 0 && Tile(_row, _column - 1).Element != null)
                Tile(_row, _column - 1).Element.IsThereMatch(_clickedElementType, _category, ref _matchElements);

            if (_column + 1 < BoardManager.Width && Tile(_row, _column + 1).Element != null)
                Tile(_row, _column + 1).Element.IsThereMatch(_clickedElementType, _category, ref _matchElements);
        }

        public static void CheckObstacleAround(int _row, int _column, BoardElementCategory _otherElementType)
        {
            if (_row - 1 >= 0 && Tile(_row - 1, _column).Element != null)
                Tile(_row - 1, _column).Element.Damage(_otherElementType);

            if (_row + 1 < BoardManager.Height && Tile(_row + 1, _column).Element != null)
                Tile(_row + 1, _column).Element.Damage(_otherElementType);

            if (_column - 1 >= 0 && Tile(_row, _column - 1).Element != null)
                Tile(_row, _column - 1).Element.Damage(_otherElementType);

            if (_column + 1 < BoardManager.Width && Tile(_row, _column + 1).Element != null)
                Tile(_row, _column + 1).Element.Damage(_otherElementType);
        }

        public static void IterateColumn(int _column, Action<BackgroundTile> _tileAction)
        {
            for (int row = 0; row < BoardManager.Height; row++)
                _tileAction?.Invoke(Tile(row, _column));
        }

        public static BoardElement FindFirstElementBelow(int _row, int _column)
        {
            if (_row + 1 >= BoardManager.Height)
                return null;

            for (int row = _row; row < BoardManager.Height; row++)
            {
                if (IsThereNotCollapsableElementAbove(row, _column))
                    return null;

                if (!Tile(row, _column).Empty)
                    return Tile(row, _column).Element;
            }

            return null;
        }

        public static MatchType Match(int _count)
        {
            if (_count < boardData.MinimumElementCountToMatch)
                return MatchType.None;

            if (_count >= boardData.MinimumElementCountToMatch && _count < boardData.RocketMatchElementCount)
                return MatchType.NormalMatch;

            if (_count >= boardData.RocketMatchElementCount && _count < boardData.TNTMatchElementCount)
                return MatchType.RocketMatch;

            if (_count >= boardData.TNTMatchElementCount && _count < boardData.DiscoBallMatchElementCount)
                return MatchType.TNTMatch;

            return MatchType.DiscoBallMatch;
        }

        public static PoolType PowerupTypeByMatchType(MatchType _matchType)
        {
            return _matchType switch
            {
                MatchType.RocketMatch => UnityEngine.Random.Range(0, 2) == 0 ? PoolType.HorizontalRocket : PoolType.VerticalRocket,
                MatchType.TNTMatch => PoolType.TNT,
                MatchType.DiscoBallMatch => PoolType.DiscoBall,
                _ => PoolType.None
            };
        }

        public static bool IsPowerupMatch(MatchType _matchType)
        {
            return _matchType == MatchType.RocketMatch || _matchType == MatchType.TNTMatch || _matchType == MatchType.DiscoBallMatch;
        }

        public static bool IsThereElementMoving()
        {
            for (int row = 0; row < BoardManager.Height; row++)
            {
                for (int column = 0; column < BoardManager.Width; column++)
                {
                    if (Tile(row, column).Empty)
                        continue;

                    if (Tile(row, column).Element.Moving)
                        return true;
                }
            }

            return false;
        }

        public static bool IsThereHorizontalRocketOnBoard()
        {
            for (int row = 0; row < BoardManager.Height; row++)
            {
                for (int column = 0; column < BoardManager.Width; column++)
                {
                    if (Tile(row, column).Element == null)
                        continue;

                    if (Tile(row, column).Element.ElementType != ElementType.HorizontalRocket)
                        continue;

                    if (Tile(row, column).Element.PoweringUp)
                        return true;
                }
            }

            return false;
        }

        public static bool IsThereVerticalRocketOnColumn(int _column)
        {
            for (int row = 0; row < BoardManager.Height; row++)
            {
                if (Tile(row, _column).Element == null)
                    continue;

                if (Tile(row, _column).Element.ElementType != ElementType.VerticalRocket)
                    continue;

                if (Tile(row, _column).Element.PoweringUp)
                    return true;
            }

            return false;
        }


        public static bool IsThereElementWaitingToCreatePowerupOnColumn(int _column)
        {
            for (int row = 0; row < BoardManager.Height; row++)
            {
                if (!Tile(row, _column).Empty && Tile(row, _column).Element.WaitingToCreatePowerup)
                    return true;
            }

            return false;
        }

        public static bool IsThereElement(ElementType _type)
        {
            for (int row = 0; row < BoardManager.Height; row++)
            {
                for (int column = 0; column < BoardManager.Width; column++)
                {
                    if (Tile(row, column).Empty)
                        continue;

                    if (Tile(row, column).Element.ElementType == _type)
                        return true;
                }
            }
            return false;
        }

        public static bool IsTherePowerupPoweringUp()
        {
            for (int row = 0; row < BoardManager.Height; row++)
            {
                for (int column = 0; column < BoardManager.Width; column++)
                {
                    if (Tile(row, column).Empty)
                        continue;

                    if (Tile(row, column).Element.PoweringUp)
                        return true;
                }
            }
            return false;
        }

        public static void DiscoBallCombineTargets(ref List<BackgroundTile> _tiles)
        {

            for (int row = 0; row < BoardManager.Height; row++)
            {
                for (int column = 0; column < BoardManager.Width; column++)
                {
                    if (Tile(row, column).DiscoBallTarget)
                        _tiles.Add(Tile(row, column));
                }
            }
        }

        public static PoolType GetTargetForDiscoBallCombine(PoolType _otherElementType)
        {
            if (_otherElementType == PoolType.HorizontalRocket || _otherElementType == PoolType.VerticalRocket)
                return UnityEngine.Random.Range(0, 2) == 0 ? PoolType.HorizontalRocket : PoolType.VerticalRocket;

            return _otherElementType;
        }

        public static bool IsThereNotCollapsableElementAbove(int _row, int _column)
        {
            for (int row = _row; row < BoardManager.Height; row++)
            {
                if (Tile(row, _column).Element == null)
                    continue;

                if (!Tile(row, _column).Element.Collapsable)
                    return true;
            }

            return false;
        }

        public static bool IsThereObstacleDestroying(int _column)
        {
            for (int row = 0; row < BoardManager.Height; row++)
            {
                if (Tile(row, _column).Element == null)
                    continue;

                if (Tile(row, _column).Element.Category != BoardElementCategory.Obstacle)
                    continue;

                if (Tile(row, _column).Element.Destroying)
                    return true;
            }

            return false;
        }

        private static BackgroundTile Tile(int _row, int _column) => BoardManager.BackgroundTiles[_row, _column];
    }
}
