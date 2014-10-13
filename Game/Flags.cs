using System;

namespace Game
{
    public enum Color
    {
        White,
        Black,
        Neither
    }

    [Flags]
    public enum CastlePermissions
    {
        WhiteKing = 1,
        WhiteQueen = 2,
        BlackKing = 4,
        BlackQueen = 8
    }

}