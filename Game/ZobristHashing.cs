using System;

namespace Game
{
    public class ZobristHashing : IChessHash
    {
        public Random random { get; private set; }

        // number of pieces = 12, plus Empty = 13
        internal readonly int[] PieceKeys = new int[12 * 120];
        internal readonly int[] EnPassantKeys = new int[16];
        internal readonly int[] CastleKeys = new int[16];
        internal readonly int SideKey;

        public ZobristHashing()
        {
            random = new Random(101010);

            for (int i = 0; i < PieceKeys.Length; i++)
            {
                PieceKeys[i] = random.Next();
            }

            for (int i = 0; i < EnPassantKeys.Length; i++)
            {
                EnPassantKeys[i] = random.Next();
            }

            for (int i = 0; i < CastleKeys.Length; i++)
            {
                CastleKeys[i] = random.Next();
            }

            SideKey = random.Next();
        }

        public int GetKeyForPiece(Piece piece)
        {
            var pieceNum = 0;
            switch (piece.Type)
            {
                case PieceType.Empty:
                    pieceNum = 0;
                    break;
                case PieceType.Pawn:
                    pieceNum = 1;
                    break;
                case PieceType.Rook:
                    pieceNum = 2;
                    break;
                case PieceType.Knight:
                    pieceNum = 3;
                    break;
                case PieceType.Bishop:
                    pieceNum = 4;
                    break;
                case PieceType.Queen:
                    pieceNum = 5;
                    break;
                case PieceType.King:
                    pieceNum = 6;
                    break;
            }
            var index = (pieceNum * 120) + piece.Square;
            return PieceKeys[index];
        }

        public int GetCastleKey(CastlePermissions castlePermissions)
        {
            return CastleKeys[(int)castlePermissions];
        }

        public int GetSideKey()
        {
            return SideKey;
        }

        public int GetEnPassantKey(int square)
        {
            if (square < 49) square -= 41; // 41 => 0; 48 => 7
            if (square > 49) square -= 63; // 71 => 8; 78 => 15
            return EnPassantKeys[square];
        }
    }

    public interface IChessHash
    {
        int GetKeyForPiece(Piece piece );
        int GetCastleKey(CastlePermissions castlePermissions);
        int GetSideKey();
        int GetEnPassantKey(int square);
    }
}