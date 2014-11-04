using System;

namespace Game
{
    public class Move
    {
        public bool IsCastleMove { get; private set; }
        public int FromSquare { get { return PieceToMove.Square; } }

        internal int ToSquare;
        internal Piece PromotedTo = new EmptyPiece();
        internal Piece PieceToMove;
        internal bool IsEnPassantCapture = false;
        internal bool IsPawnStartMove = false;
        internal Piece Captured = new EmptyPiece();

        // todo - test detect pawn start move and set PawnStartMove 
        public Move(Piece piece, int toSquare) : this(piece, toSquare, false)
        {
        }

        public Move(Piece piece, Piece pieceToCapture)
        {
            PieceToMove = piece;
            ToSquare = pieceToCapture.Square;
            Captured = pieceToCapture;
        }

        public Move(Piece piece, int toSquare, Piece promotedTo)
        {
            PieceToMove = piece;
            ToSquare = toSquare;
            PromotedTo = promotedTo;
            PromotedTo.Color = piece.Color;
            PromotedTo.Square = toSquare;
        }

        public Move(Piece piece, int toSquare, bool isCastleMove)
        {
            PieceToMove = piece;
            ToSquare = toSquare;
            if (piece.Type == PieceType.Pawn && Math.Abs(piece.Square - toSquare) == 20)
            {
                IsPawnStartMove = true;
            }
            IsCastleMove = isCastleMove;
        }

        public override string ToString()
        {
            var move = Lookups.MapSquareToRankFile(PieceToMove.Square) + Lookups.MapSquareToRankFile(ToSquare);
            if (PromotedTo.Type != PieceType.Empty)
            {
                move += PromotedTo.ToString();
            }

            return move;
        }
    }
}