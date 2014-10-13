using System;

namespace Game
{
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
        protected int[] MoveDirection = new int[1];
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

            MoveDirection = new[] { -1, -10, 1, 10, 9, -11, 11, 9 };
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
            MoveDirection = new[] { -1, -10, 1, 10, 9, -11, 11, 9 };
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
            MoveDirection = new[] { -1, -10, 1, 10 };
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
            MoveDirection = new[] { 9, -11, 11, 9 };
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
            MoveDirection = new[] { -8, -19, -21, -12, 8, 19, 21, 12 };
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