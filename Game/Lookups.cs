using System;
using System.Collections.Generic;

namespace Game
{
    public static class Lookups
    {
        public const int A1 = 21;
        public const int A2 = 22;
        public const int A3 = 23;
        public const int A4 = 24;
        public const int A5 = 25;
        public const int A6 = 26;
        public const int A7 = 27;
        public const int A8 = 28;
        public const int H1 = 91;
        public const int H3 = 93;
        public const int H4 = 94;
        public const int H5 = 95;
        public const int H6 = 96;
        public const int H2 = 92;
        public const int H7 = 97;
        public const int H8 = 98;

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

        public static int Map64To120(int gameBoardIndex)
        {
            return Array64To120[gameBoardIndex];
        }

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

        private static readonly string[] SquareToRankFile = 
        {
            "  ", "  ", "  ", "  ", "  ", "  ", "  ", "  ", "  ",  "  ", 
            "  ", "  ", "  ", "  ", "  ", "  ", "  ", "  ", "  ",  "  ", 
            "  ", "a1", "b1", "c1", "d1", "e1", "f1", "g1", "h1",  "  ", 
            "  ", "a2", "b2", "c2", "d2", "e2", "f2", "g2", "h2",  "  ", 
            "  ", "a3", "b3", "c3", "d3", "e3", "f3", "g3", "h3",  "  ", 
            "  ", "a4", "b4", "c4", "d4", "e4", "f4", "g4", "h4",  "  ", 
            "  ", "a5", "b5", "c5", "d5", "e5", "f5", "g5", "h5",  "  ", 
            "  ", "a6", "b6", "c6", "d6", "e6", "f6", "g6", "h6",  "  ", 
            "  ", "a7", "b7", "c7", "d7", "e7", "f7", "g7", "h7",  "  ", 
            "  ", "a8", "b8", "c8", "d8", "e8", "f8", "g8", "h8",  "  ", 
            "  ", "  ", "  ", "  ", "  ", "  ", "  ", "  ", "  ",  "  ", 
            "  ", "  ", "  ", "  ", "  ", "  ", "  ", "  ", "  ",  "  ", 
        };

        public static string MapSquareToRankFile(int fullIndex)
        {
            return SquareToRankFile[fullIndex];
        }

        public static readonly int[] CastlePerms = 
        {
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 
        };

        public const int Maxmoves = 2048;
    }
}