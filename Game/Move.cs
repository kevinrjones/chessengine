namespace Game
{
    public class Move
    {
        public CastlePermissions CastlePermission { get; private set; }
        public int FromSquare { get { return PieceToMove.Square; } }
        internal int ToSquare;
        internal Piece PromotedTo;
        internal Piece PieceToMove;
        // todo
        internal bool IsPawnStartMove = false;
        internal Piece Captured = new EmptyPiece();

        public Move(Piece piece, int toSquare)
        {
            PieceToMove = piece;
            ToSquare = toSquare;
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
            PromotedTo.Square = piece.Square;
        }

        public Move(Piece piece, int toSquare, CastlePermissions castlePermission)
        {
            PieceToMove = piece;
            ToSquare = toSquare;
            CastlePermission = castlePermission;
        }

        public override string ToString()
        {
            var move = Lookups.MapSquareToRankFile(PieceToMove.Square) + Lookups.MapSquareToRankFile(ToSquare);
            if (PromotedTo != null)
            {
                move += PromotedTo.ToString();
            }

            return move + " (" + PieceToMove.Square + " to " + ToSquare + ")";
        }
    }
}