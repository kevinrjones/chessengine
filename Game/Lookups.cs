using System;
using System;
using System.Collections.Generic;

namespace Game
{
    public static class Lookups
    {
        public const int A1 = 21;
        public const int A2 = 31;
        public const int A3 = 41;
        public const int A4 = 51;
        public const int A5 = 61;
        public const int A6 = 71;
        public const int A7 = 81;
        public const int A8 = 91;

        public const int B1 = 22;
        public const int B2 = 32;
        public const int B3 = 42;
        public const int B4 = 52;
        public const int B5 = 62;
        public const int B6 = 72;
        public const int B7 = 82;
        public const int B8 = 92;

        public const int C1 = 23;
        public const int C2 = 33;
        public const int C3 = 43;
        public const int C4 = 53;
        public const int C5 = 63;
        public const int C6 = 73;
        public const int C7 = 83;
        public const int C8 = 93;

        public const int D1 = 24;
        public const int D2 = 34;
        public const int D3 = 44;
        public const int D4 = 54;
        public const int D5 = 64;
        public const int D6 = 74;
        public const int D7 = 84;
        public const int D8 = 94;

        public const int E1 = 25;
        public const int E2 = 35;
        public const int E3 = 45;
        public const int E4 = 55;
        public const int E5 = 65;
        public const int E6 = 75;
        public const int E7 = 85;
        public const int E8 = 95;

        public const int F1 = 26;
        public const int F2 = 36;
        public const int F3 = 46;
        public const int F4 = 56;
        public const int F5 = 66;
        public const int F6 = 76;
        public const int F7 = 86;
        public const int F8 = 96;

        public const int G1 = 27;
        public const int G2 = 37;
        public const int G3 = 47;
        public const int G4 = 57;
        public const int G5 = 67;
        public const int G6 = 77;
        public const int G7 = 87;
        public const int G8 = 97;

        public const int H1 = 28;
        public const int H2 = 38;
        public const int H3 = 48;
        public const int H4 = 58;
        public const int H5 = 68;
        public const int H6 = 78;
        public const int H7 = 88;
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

        public static readonly Dictionary<char, PieceType> FenPieceLookup = new Dictionary<char, PieceType>
        {
                {'p', PieceType.BlackPawn},
                {'P', PieceType.WhitePawn},
                {'r', PieceType.BlackRook},
                {'R', PieceType.WhiteRook},
                {'n', PieceType.BlackKnight},
                {'N', PieceType.WhiteKnight},
                {'b', PieceType.BlackBishop},
                {'B', PieceType.WhiteBishop},
                {'q', PieceType.BlackQueen},
                {'Q', PieceType.WhiteQueen},
                {'k', PieceType.BlackKing},
                {'K',PieceType.WhiteKing},
            };


        public static Dictionary<PieceType, Values> PieceValues = new Dictionary<PieceType, Values>
        {
            { PieceType.WhitePawn, new Values { Big = false, Major = false, Value = 100, Slides = false } },
            { PieceType.WhiteRook, new Values { Big = true, Major = false, Value = 550, Slides = true } },
            { PieceType.WhiteKnight, new Values { Big = true, Major = false, Value = 325, Slides = false } },
            { PieceType.WhiteBishop, new Values { Big = false, Major = false, Value = 325, Slides = true } },
            { PieceType.WhiteQueen, new Values { Big = true, Major = true, Value = 1000, Slides = true } },
            { PieceType.WhiteKing, new Values { Big = true, Major = true, Value = 50000, Slides = false } },
            { PieceType.BlackPawn, new Values { Big = false, Major = false, Value = 100, Slides = false } },
            { PieceType.BlackRook, new Values { Big = true, Major = false, Value = 550, Slides = true } },
            { PieceType.BlackKnight, new Values { Big = true, Major = false, Value = 325, Slides = false } },
            { PieceType.BlackBishop, new Values { Big = false, Major = false, Value = 325, Slides = true } },
            { PieceType.BlackQueen, new Values { Big = true, Major = true, Value = 1000, Slides = true } },
            { PieceType.BlackKing, new Values { Big = true, Major = true, Value = 50000, Slides = false } },
        };

        public static Dictionary<PieceType, int[]> MoveDirections = new Dictionary<PieceType, int[]>
        {
            {PieceType.WhiteRook, new[] { -1, -10, 1, 10 }},
            {PieceType.WhiteKnight, new[] { -8, -19, -21, -12, 8, 19, 21, 12 }},
            {PieceType.WhiteBishop, new[] { -9, -11, 11, 9 }},
            {PieceType.WhiteQueen, new[] { -1, -10, 1, 10, -9, -11, 11, 9  }},
            {PieceType.WhiteKing, new[] { -1, -10, 1, 10, -9, -11, 11, 9  }},
            {PieceType.BlackRook, new[] { -1, -10, 1, 10 }},
            {PieceType.BlackKnight, new[] { -8, -19, -21, -12, 8, 19, 21, 12 }},
            {PieceType.BlackBishop, new[] { -9, -11, 11, 9 }},
            {PieceType.BlackQueen, new[] { -1, -10, 1, 10, -9, -11, 11, 9  }},
            {PieceType.BlackKing, new[] { -1, -10, 1, 10, -9, -11, 11, 9  }},
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
            15, 13, 15, 15, 15, 12, 15, 15, 14, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15,  7, 15, 15, 15,  3, 15, 15, 11, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 
        };

        public static int GetCastlePerms(int square)
        {
            return CastlePerms[square];
        }

        public const int Maxmoves = 2048;
        public const int MaxDepth = 20;

        public enum Pieces
        {
            Empty = 0,
            wP = 1,
            wR = 2,
            wN = 3,
            wB = 4,
            wQ = 5,
            wK = 6,
            bP = 7,
            bR = 8,
            bN = 9,
            bB = 10,
            bQ = 11,
            bK = 12

        }

        public static int[] PieceCounts = new int[13];

    }
}