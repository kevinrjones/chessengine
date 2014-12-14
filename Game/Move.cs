using System;

namespace Game
{
    public class Move
    {
        public bool IsCastleMove { get; private set; }
        public int FromSquare { get; set; }

        internal int ToSquare;
        internal PieceType PromotedTo = PieceType.Empty;
        internal PieceType FromPiece;
        internal bool IsEnPassantCapture = false;
        internal bool IsPawnStartMove = false;
        internal PieceType Captured = PieceType.Empty;
        public int PromotedToSquare { get; set; }

        public Color PromotedToColor { get; set; }

        // todo - test detect pawn start move and set PawnStartMove 
        public Move(PieceType fromPiece, int fromSquare, int toSquare) : this(fromPiece, fromSquare, toSquare, false)
        {
        }

        public Move(PieceType fromPiece, int fromSquare, PieceType pieceToCapture, int pieceToCaptureSquare)
        {
            FromPiece = fromPiece;
            FromSquare = fromSquare;
            ToSquare = pieceToCaptureSquare;
            Captured = pieceToCapture;
        }

        public Move(PieceType fromPiece, int fromSquare, PieceType pieceToCapture, int captureSquare, PieceType promotedTo, Color promotedToColor)
        {
            FromPiece = fromPiece;
            FromSquare = fromSquare;
            ToSquare = captureSquare;
            Captured = pieceToCapture;
            PromotedTo = promotedTo;
            PromotedToColor = promotedToColor;
            PromotedToSquare = ToSquare;
        }


        public Move(PieceType fromPiece, int fromSquare, int toSquare, PieceType promotedTo, Color promotedToColor)
        {
            FromPiece = fromPiece;
            FromSquare = fromSquare;
            ToSquare = toSquare;
            PromotedTo = promotedTo;
            PromotedToColor = promotedToColor;
            PromotedToSquare = toSquare;
        }

        public Move(PieceType fromPiece, int fromSquare, int toSquare, bool isCastleMove)
        {
            FromPiece = fromPiece;
            FromSquare = fromSquare;
            ToSquare = toSquare;
            if ((fromPiece == PieceType.WhitePawn || fromPiece == PieceType.BlackPawn) && Math.Abs(fromSquare - toSquare) == 20)
            {
                IsPawnStartMove = true;
            }
            IsCastleMove = isCastleMove;
        }

        public override string ToString()
        {
            var move = Lookups.MapSquareToRankFile(FromSquare) + Lookups.MapSquareToRankFile(ToSquare);
            if (PromotedTo != PieceType.Empty)
            {
                move += PromotedTo.ToString();
            }

            return move;
        }
    }
}