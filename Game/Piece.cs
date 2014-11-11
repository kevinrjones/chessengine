using System;
using System.Data.SqlClient;

namespace Game
{
    [Flags]
    public enum PieceType
    {
        OffBoard = -1,
        Empty = 0,
        Pawn = 1,
        Rook = 2,
        Knight = 4,
        Bishop = 8,
        Queen = 16,
        King = 32,
    };

    public abstract class Piece
    {
        public PieceType Type = PieceType.Empty;

        public Piece()
        {
            Color = Color.Neither;
            Slides = true;
        }
        public bool Big { get; protected set; }
        public bool Major { get; protected set; }
        public virtual bool Minor { get { return !Major; } }
        public Color Color { get; set; }
        public bool Slides { get; protected set; }
        public int Value { get; protected set; }
        public int Square;
        public abstract Piece Clone();
    }

    public class EmptyPiece : Piece
    {
        public override string ToString()
        {
            return ".";
        }

        public override Piece Clone()
        {
            return new EmptyPiece{Color = Color, Square = Square};
        }
    }

    public class OffBoardPiece : Piece
    {
        public OffBoardPiece()
        {
            Type = PieceType.OffBoard;
        }
        public override string ToString()
        {
            return "X";
        }
        public override Piece Clone()
        {
            return new OffBoardPiece { Color = Color, Square = Square };
        }
    }

    public class King : Piece
    {
        public static int[] MoveDirection = { -1, -10, 1, 10, -9, -11, 11, 9 };

        public King()
        {
            Type = PieceType.King;
            Big = true;
            Major = true;
            Value = 50000;
            Slides = false;
        }
        public override string ToString()
        {
            return Color == Color.White ? "K" : "k";
        }
        public override Piece Clone()
        {
            return new King { Color = Color, Square = Square };
        }
    }

    public class Queen : Piece
    {
        public static int[] MoveDirection = { -1, -10, 1, 10, -9, -11, 11, 9 };

        public Queen()
        {
            Type = PieceType.Queen;
            Big = true;
            Major = true;
            Value = 1000;
        }
        public override string ToString()
        {
            return Color == Color.White ? "Q" : "q";
        }
        public override Piece Clone()
        {
            return new Queen { Color = Color, Square = Square };
        }
    }

    public class Rook : Piece
    {
        public static int[] MoveDirection = { -1, -10, 1, 10 };

        public Rook()
        {
            Type = PieceType.Rook;
            Big = true;
            Major = true;
            Value = 550;
        }
        public override string ToString()
        {
            return Color == Color.White ? "R" : "r";
        }
        public override Piece Clone()
        {
            return new Rook { Color = Color, Square = Square };
        }
    }

    public class Bishop : Piece
    {
        public static int[] MoveDirection = { -9, -11, 11, 9 };

        public Bishop()
        {
            Type = PieceType.Bishop;
            Big = true;
            Major = false;
            Value = 325;
        }
        public override string ToString()
        {
            return Color == Color.White ? "B" : "b";
        }
        public override Piece Clone()
        {
            return new Bishop { Color = Color, Square = Square };
        }

    }

    public class Knight : Piece
    {
        public static int[] MoveDirection = { -8, -19, -21, -12, 8, 19, 21, 12 };

        public Knight()
        {
            Type = PieceType.Knight;
            Big = true;
            Major = false;
            Value = 325;
            Slides = false;
        }
        public override string ToString()
        {
            return Color == Color.White ? "N" : "n";
        }
        public override Piece Clone()
        {
            return new Knight { Color = Color, Square = Square };
        }
    }

    public class Pawn : Piece
    {
        public Pawn()
        {
            Type = PieceType.Pawn;
            Big = false;
            Major = false;
            Value = 100;
            Slides = false;
        }

        public override bool Minor { get { return false; } }

        public override string ToString()
        {
            return Color == Color.White ? "P" : "p";
        }
        public override Piece Clone()
        {
            return new Pawn { Color = Color, Square = Square };
        }
    }
}