using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

    public class Board
    {
        //use Secure rand instead
        private Random random;
        public Board()
        {
            random = new Random();
            Squares = new Piece[120];
            Side = Color.White;
            Material = new int[2];
            CountOfEachPiece = new int[12];
            ActivePieces = new List<Piece>();
            KeyForSide = random.Next();
            EnPassantKeys = new int[16];
            for (int i = 0; i < EnPassantKeys.Length; i++)
            {
                EnPassantKeys[i] = random.Next();
            }
            ResetBoard();
        }

        public int[] EnPassantKeys { get; set; }

        //public Dictionary<string, int> Squares { get; set; }

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
        private int KeyForSide;

        protected int GeneratePositionKey()
        {
            int finalKey = 0;
            for (int ndxSquares = 0; ndxSquares < Squares.Length; ndxSquares++)
            {
                finalKey |= Squares[ndxSquares].PositionKeys[ndxSquares];
            }
            if (Side == Color.White)
            {
                finalKey |= KeyForSide;
            }
            finalKey |= EnPassantKeys[EnPassant];
            finalKey |= (int)CastlePermission;

            return finalKey;
        }

        public static int FileRankToSquare(int file, int rank)
        {
            return ((21 + file) + (rank * 10));
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
            string[] sections = fen.Split('/', ' ');
            const int sideToMoveSection = 8;
            const int castleSection = 9;
            const int enPassantSection = 10;

            if (sections.Length != 13)
            {
                throw new ArgumentException("fen");
            }

            ParseRankAndFile(sections);

            Side = sections[sideToMoveSection] == "w" ? Color.White : Color.Black;

            ParseCastleSection(sections[castleSection]);
            ParseEnPassantSection(sections[enPassantSection]);
        }

        private void ParseEnPassantSection(string section)
        {
            if (section == "-") return;

            if(section.Length != 2) throw new ArgumentException(section);

            // todo: char to in
            int file = (section[0] -'a');
            int rank = section[1] - '1';

            EnPassant = FileRankToSquare(file, rank);
        }

        private void ParseCastleSection(string section)
        {
            foreach (var flag in section)
            {
                switch (flag)
                {
                    case 'K': CastlePermission |= CastlePermissions.WhiteKing; break;
                    case 'k': CastlePermission |= CastlePermissions.BlackKing; break;
                    case 'Q': CastlePermission |= CastlePermissions.WhiteQueen; break;
                    case 'q': CastlePermission |= CastlePermissions.BlackQueen; break;
                }
            }
        }

        private void ParseRankAndFile(string[] sections)
        {
            Piece piece;
            int ranks = 8;
            int file;
            for (int rank = 0; rank < ranks; rank++)
            {
                file = 0;
                var section = sections[rank];
                for (int ndx = 0; ndx < section.Length; ndx++)
                {
                    var countOfPieces = 1;
                    switch (section[ndx])
                    {
                        case 'p':
                            piece = new Pawn { Color = Color.Black };
                            break;
                        case 'r':
                            piece = new Rook { Color = Color.Black };
                            break;
                        case 'n':
                            piece = new Knight { Color = Color.Black };
                            break;
                        case 'b':
                            piece = new Bishop { Color = Color.Black };
                            break;
                        case 'q':
                            piece = new Queen { Color = Color.Black };
                            break;
                        case 'k':
                            piece = new King { Color = Color.Black };
                            break;
                        case 'P':
                            piece = new Pawn { Color = Color.White };
                            break;
                        case 'R':
                            piece = new Rook { Color = Color.White };
                            break;
                        case 'N':
                            piece = new Knight { Color = Color.White };
                            break;
                        case 'B':
                            piece = new Bishop { Color = Color.White };
                            break;
                        case 'Q':
                            piece = new Queen { Color = Color.White };
                            break;
                        case 'K':
                            piece = new King { Color = Color.White };
                            break;
                        default:
                            piece = new EmptyPiece();
                            countOfPieces = int.Parse(section[ndx].ToString());
                            break;
                    }

                    for (int i = 0; i < countOfPieces; i++)
                    {
                        int boardNdx = FileRankToSquare(file, rank);
                        Squares[boardNdx] = piece;
                        file++;
                    }
                }
            }
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

        internal static int GameIndexToFullIndex(int gameBoardIndex)
        {
            int offset = (gameBoardIndex / 8) * 2;
            return gameBoardIndex + 21 + offset;
        }
    }

    public class Piece
    {
        private Random random;
        public Piece()
        {
            //use Secure rand instead
            random = new Random();
            Color = Color.White;
            Slides = true;
            PositionKeys = new int[120];
            for (var i = 0; i < PositionKeys.Length; i++)
            {
                PositionKeys[i] = random.Next();
            }
        }
        public bool Big { get; protected set; }
        public bool Major { get; protected set; }
        public virtual bool Minor { get { return !Major; } }
        public Color Color { get; set; }
        public bool Slides { get; protected set; }
        public int Value { get; protected set; }
        public int[] PositionKeys { get; set; }
    }

    public class EmptyPiece : Piece
    {
        public EmptyPiece()
        {
            for (var i = 0; i < PositionKeys.Length; i++)
            {
                PositionKeys[i] = 0;
            }
        }

        public override string ToString()
        {
            return " ";
        }
    }

    public class OffBoardPiece : Piece
    {
        public OffBoardPiece()
        {
            for (var i = 0; i < PositionKeys.Length; i++)
            {
                PositionKeys[i] = 0;
            }
        }
        public override string ToString()
        {
            return "X";
        }
    }

    public class King : Piece
    {
        public King()
        {
            Big = true;
            Major = true;
            Value = 50000;
            Slides = false;
        }
        public override string ToString()
        {
            return Color == Color.White ? "K" : "k";
        }
    }

    public class Queen : Piece
    {
        public Queen()
        {
            Big = true;
            Major = true;
            Value = 1000;
        }
        public override string ToString()
        {
            return Color == Color.White ? "Q" : "q";
        }
    }

    public class Rook : Piece
    {
        public Rook()
        {
            Big = true;
            Major = true;
            Value = 550;
        }
        public override string ToString()
        {
            return Color == Color.White ? "R" : "r";
        }
    }

    public class Bishop : Piece
    {
        public Bishop()
        {
            Big = true;
            Major = false;
            Value = 325;
        }
        public override string ToString()
        {
            return Color == Color.White ? "B" : "b";
        }
    }

    public class Knight : Piece
    {
        public Knight()
        {
            Big = true;
            Major = false;
            Value = 325;
            Slides = false;
        }
        public override string ToString()
        {
            return Color == Color.White ? "N" : "n";
        }
    }

    public class Pawn : Piece
    {
        public Pawn()
        {
            Big = false;
            Major = false;
            Value = 100;
            Slides = false;
        }

        public override bool Minor { get { return false; } }
    }
}

