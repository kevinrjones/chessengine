using System.Collections.Generic;

namespace Game
{
    public static class Lookups
    {
        public const int BoardSize = 120;
        public const int OffBoard = 100;


        public static int[] FilesBoard = new int[BoardSize];
        public static int[] RanksBoard = new int[BoardSize];

        static Lookups()
        {
            ResetLookups();
        }

        public static void ResetLookups()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                FilesBoard[i] = OffBoard;
                RanksBoard[i] = OffBoard;
            }

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    int sq = Board.FileRankToSquare(file, rank);
                    FilesBoard[sq] = file;
                    RanksBoard[sq] = rank;
                }
            }
        }

        public static readonly Dictionary<char, Piece> FenPieceLookup = new Dictionary<char, Piece>
            {
                {'p', new Pawn{Color = Color.Black}},
                {'P', new Pawn{Color = Color.White}},
                {'r', new Rook{Color = Color.Black}},
                {'R', new Rook{Color = Color.White}},
                {'n', new Knight{Color = Color.Black}},
                {'N', new Knight{Color = Color.White}},
                {'b', new Bishop{Color = Color.Black}},
                {'B', new Bishop{Color = Color.White}},
                {'q', new Queen{Color = Color.Black}},
                {'Q', new Queen{Color = Color.White}},
                {'k', new King{Color = Color.Black}},
                {'K', new King{Color = Color.White}},
            };

    }
}