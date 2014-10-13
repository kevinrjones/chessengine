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

        // todo - do I need this function
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

