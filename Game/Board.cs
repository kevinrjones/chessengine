using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace Game
{
 
    public class Board
    {
        //use Secure rand instead
        private Random random;
        private readonly int _keyForSide;

        public Board()
        {
            random = new Random();
            Squares = new Piece[120];
            Side = Color.White;
            Material = new int[2];
            CountOfEachPiece = new int[12];
            ActivePieces = new List<Piece>();
            _keyForSide = random.Next();
            EnPassantKeys = new int[16];
            for (int i = 0; i < EnPassantKeys.Length; i++)
            {
                EnPassantKeys[i] = random.Next();
            }
            ResetBoard();
        }

        public int[] EnPassantKeys { get; set; }
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
        public int PositionKey { get; set; }

        protected int GeneratePositionKey()
        {
            int finalKey = 0;
            for (int ndxSquares = 0; ndxSquares < Squares.Length; ndxSquares++)
            {
                finalKey |= Squares[ndxSquares].PositionKeys[ndxSquares];
            }
            if (Side == Color.White)
            {
                finalKey |= _keyForSide;
            }
            finalKey |= EnPassantKeys[EnPassant];
            finalKey |= (int)CastlePermission;

            return finalKey;
        }


        /*
         long LongRandom(long min, long max, Random rand) {
    byte[] buf = new byte[4];
    rand.NextBytes(buf);
    long longRand = BitConverter.ToInt32(buf, 0);

    return (Math.Abs(longRand % (max - min)) + min);
}
         */

        // rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
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
}

