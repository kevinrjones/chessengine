using System;
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
                    int sq = FileRankToSquare(file, rank);
                    FilesBoard[sq] = file;
                    RanksBoard[sq] = rank;
                }
            }
        }

        public static readonly Dictionary<char, Func<Piece>> FenPieceLookup = new Dictionary<char, Func<Piece>>
            {
                {'p', () => new Pawn{Color = Color.Black}},
                {'P', () => new Pawn{Color = Color.White}},
                {'r', () => new Rook{Color = Color.Black}},
                {'R', () => new Rook{Color = Color.White}},
                {'n', () => new Knight{Color = Color.Black}},
                {'N', () => new Knight{Color = Color.White}},
                {'b', () => new Bishop{Color = Color.Black}},
                {'B', () => new Bishop{Color = Color.White}},
                {'q', () => new Queen{Color = Color.Black}},
                {'Q', () => new Queen{Color = Color.White}},
                {'k', () => new King{Color = Color.Black}},
                {'K', () => new King{Color = Color.White}},
            };
        public static int FileRankToSquare(int file, int rank)
        {
            return ((21 + file) + (rank * 10));
        }

        public static int Map64To120(int gameBoardIndex)
        {
            return Array64To120[gameBoardIndex];
        }

        private static readonly int[] Array64To120 = 
        {
            21, 22, 23, 24, 25, 26, 27, 28,
            31, 32, 33, 34, 35, 36, 37, 38,
            41, 42, 43, 44, 45, 46, 47, 48,
            51, 52, 53, 54, 55, 56, 57, 58,
            61, 62, 63, 64, 65, 66, 67, 68,
            71, 72, 73, 74, 75, 76, 77, 78,
            81, 82, 83, 84, 85, 86, 87, 88,
            91, 92, 93, 94, 95, 96, 97, 98,
        };

        private static readonly int[] Array120To64 = 
        {
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1,  0,  1,  2,  3,  4,  5,  6,  7, -1,
            -1,  8,  9, 10, 11, 12, 13, 14, 15, -1,
            -1, 16, 17, 18, 19, 20, 21, 22, 23, -1,
            -1, 24, 25, 26, 27, 28, 29, 30, 31, -1,
            -1, 32, 33, 34, 35, 36, 37, 38, 39, -1,
            -1, 40, 41, 42, 43, 44, 45, 46, 47, -1,
            -1, 48, 49, 50, 51, 52, 53, 54, 55, -1,
            -1, 56, 57, 58, 59, 60, 61, 62, 63, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        };

        public static int Map120To64(int fullIndex)
        {
            return Array120To64[fullIndex];
        }

    }
}