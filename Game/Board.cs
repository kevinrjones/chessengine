using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace Game
{
 
    public class Board
    {
        public Board()
        {
            Squares = new Piece[120];
            Side = Color.White;
            Material = new int[2];
            CountOfEachPiece = new int[12];
            ActivePieces = new List<Piece>();
            ResetBoard();
        }

        // todo - do I need this function - can I use positionKey.GeneratePositionKey directly
        protected int GeneratePositionKey()
        {
            var positionKey = new PositionKey();
            return positionKey.GeneratePositionKey(Squares, Side, EnPassant, CastlePermission);
        }

        public Piece[] Squares { get; private set; }
        public Color Side { get; set; }
        public int FiftyMove { get; set; }
        public int HistoryPly { get; set; }
        public int Ply { get; set; }
        public int EnPassant { get; set; }
        public CastlePermissions CastlePermission { get; set; }
        public int[] Material { get; set; }
        public int[] CountOfEachPiece { get; set; }
        public List<Piece> ActivePieces;


        public void ParseFen(string fen)
        {
            ResetBoard();

            var parseFen = new ParseFen(fen);

            Squares = parseFen.ParseRankAndFile();
            Side = parseFen.SideToMove(); 
            CastlePermission = parseFen.ParseCastleSection();
            EnPassant = parseFen.ParseEnPassantSection();

            // todo: update material?
        }

        internal void ResetBoard()
        {
            for (int i = 0; i < Squares.Length; i++)
            {
                Squares[i] = new OffBoardPiece();
            }

            for (int i = 0; i < 64; i++)
            {
                Squares[GameIndexToFullIndex(i)] = new EmptyPiece();
            }

            for (int i = 0; i < Material.Length; i++)
            {
                Material[i] = 0;
            }

            for (int i = 0; i < CountOfEachPiece.Length; i++)
            {
                CountOfEachPiece[i] = 0;
            }

            Side = Color.Neither;

            FiftyMove = 0;
            HistoryPly = 0;
            Ply = 0;
            EnPassant = 0;
            CastlePermission = 0;
            // MoveListStart[Ply] = 0
        }

        public static int FileRankToSquare(int file, int rank)
        {
            return ((21 + file) + (rank * 10));
        }

        public static int GameIndexToFullIndex(int gameBoardIndex)
        {
            int offset = (gameBoardIndex / 8) * 2;
            return gameBoardIndex + 21 + offset;
        }

        public bool IsSquareAttacked(int square, Color side)
        {
            if (IsAttackedByPawn(square, side)) return true;
            if (IsAttackedByAKnight(square, side)) return true;
            if (IsAttackedByAKing(square, side)) return true;            
            if (IsSquareAttackedByRook(square, side)) return true;

            return false;
        }

        // todo: generate moves

        private bool IsSquareAttackedByRook(int square, Color side)
        {
            foreach (var direction in Rook.MoveDirection)
            {
                var testSquare = direction;
                var piece = Squares[square + testSquare];
                while (piece.GetType() != typeof (OffBoardPiece))
                {
                    if (piece.Color == side && piece.GetType() == typeof (Rook) ||
                        piece.Color == side && piece.GetType() == typeof (Queen)) return true;

                    testSquare += direction;
                }
            }
            return false;
        }

        private bool IsAttackedByAKing(int square, Color side)
        {
            foreach (var direction in King.MoveDirection)
            {
                var piece = Squares[square + direction];
                if (piece.Color == side && piece.GetType() == typeof(King)) return true;
            }
            return false;
        }

        private bool IsAttackedByAKnight(int square, Color side)
        {
            foreach (var direction in Knight.MoveDirection)
            {
                var piece = Squares[square + direction];
                if (piece.Color == side && piece.GetType() == typeof (Knight)) return true;
            }
            return false;
        }

        private bool IsAttackedByPawn(int square, Color side)
        {
            if (side == Color.White)
            {
                var piece = Squares[square - 11];
                if (piece.Color == side && piece.GetType() == typeof (Pawn)) return true;
                piece = Squares[square - 9];
                if (piece.Color == side && piece.GetType() == typeof (Pawn)) return true;
            }
            else
            {
                var piece = Squares[square + 11];
                if (piece.Color == side && piece.GetType() == typeof (Pawn)) return true;
                piece = Squares[square + 9];
                if (piece.Color == side && piece.GetType() == typeof (Pawn)) return true;
            }
            return false;
        }
    }

    public class PositionKey
    {
        private readonly int _keyForSide;
        readonly Random _random = new Random();

        public PositionKey()
        {
            _keyForSide = _random.Next();
        }

        static int[] EnPassantKeys { get; set; }
        static PositionKey() 
        {
             EnPassantKeys = new int[16];            
        }

        public int GeneratePositionKey(Piece[] squares, Color side, int enPassant, CastlePermissions castlePermission)
        {
            int finalKey = 0;
            for (int ndxSquares = 0; ndxSquares < squares.Length; ndxSquares++)
            {
                finalKey |= squares[ndxSquares].PositionKeys[ndxSquares];
            }
            if (side == Color.White)
            {
                finalKey |= _keyForSide;
            }
            finalKey |= EnPassantKeys[enPassant];
            finalKey |= (int)castlePermission;

            return finalKey;
        }
    }
}

