namespace Game
{
    public class Move
    {
        public CastlePermissions CastlePermission { get; private set; }
        public int FromSquare { get { return PieceToMove.Square; } }
        internal int ToSquare;
        internal Piece PromotedTo;
        internal Piece PieceToMove;

        public Move(Piece piece, int toSquare)
        {
            PieceToMove = piece;
            ToSquare = toSquare;
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
    }
}