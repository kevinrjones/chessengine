using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    public class Board
    {
        public Board()
        {
            Squares = new Piece[120];
            SideToMove = Color.White;
            Material = new int[2];
            CountOfEachPiece = new int[12];
            ActivePieces = new List<Piece>();
            ResetBoard();
        }

        // todo - do I need this function - can I use positionKey.GeneratePositionKey directly
        protected int GeneratePositionKey()
        {
            var positionKey = new PositionKey();
            return positionKey.GeneratePositionKey(Squares, SideToMove, EnPassantSquare, CastlePermission);
        }

        public Piece[] Squares { get; private set; }
        public Color SideToMove { get; set; }
        public int FiftyMove { get; set; }
        public int HistoryPly { get; set; }
        public int Ply { get; set; }
        public int EnPassantSquare { get; set; }
        public CastlePermissions CastlePermission { get; set; }
        public int[] Material { get; set; }
        public int[] CountOfEachPiece { get; set; }
        public List<Piece> ActivePieces;

        public override string ToString()
        {
            var builder = new StringBuilder();
            for (int rank = 7; rank >= 0; rank--)
            {
                for (int file = 0; file < 7; file++)
                {
                    int boardNdx = Lookups.FileRankToSquare(file, rank);
                    var piece = Squares[boardNdx];
                    builder.Append(piece);
                }
                builder.Append(Environment.NewLine);
            }
            return builder.ToString();
        }

        public void ParseFen(string fen)
        {
            ResetBoard();

            var parseFen = new ParseFen(fen);

            Squares = parseFen.ParseRankAndFile();
            SideToMove = parseFen.SideToMove();
            CastlePermission = parseFen.ParseCastleSection();
            EnPassantSquare = parseFen.ParseEnPassantSection();

            UpdateMaterial();
        }

        private void UpdateMaterial()
        {
            for (int pieceNdx = 0; pieceNdx < 64; pieceNdx++)
            {
                var sq = Lookups.Map64To120(pieceNdx);
                var piece = Squares[sq];
                if (piece.Type != PieceType.Empty)
                {
                    Material[(int)piece.Color] += piece.Value;
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
                Squares[Lookups.Map64To120(i)] = new EmptyPiece();
            }

            for (int i = 0; i < Material.Length; i++)
            {
                Material[i] = 0;
            }

            for (int i = 0; i < CountOfEachPiece.Length; i++)
            {
                CountOfEachPiece[i] = 0;
            }

            SideToMove = Color.Neither;

            FiftyMove = 0;
            HistoryPly = 0;
            Ply = 0;
            EnPassantSquare = 0;
            CastlePermission = 0;
        }


        public bool IsSquareAttacked(int square, Color side)
        {
            if (IsAttackedByPawn(square, side)) return true;
            if (IsAttackedByAKnight(square, side)) return true;
            if (IsAttackedByAKing(square, side)) return true;
            if (IsSquareAttackedByARook(square, side)) return true;
            if (IsSquareAttackedByABishop(square, side)) return true;

            return false;
        }

        internal readonly List<Pawn> PawnPieceList = new List<Pawn>(16);
        internal readonly List<Rook> RookPieceList = new List<Rook>(4);
        internal readonly List<Knight> KnightPieceList = new List<Knight>(4);
        internal readonly List<Bishop> BishopPieceList = new List<Bishop>(4);
        internal readonly List<Queen> QueenPieceList = new List<Queen>(4);
        internal readonly List<King> KingPieceList = new List<King>(2);
        internal readonly bool[] EmptySquares = new bool[120];

        public List<Move> Moves = new List<Move>();
        public void GeneratePieceList()
        {
            for (int i = 0; i < EmptySquares.Length; i++)
            {
                EmptySquares[0] = false;
            }

            for (var square = 0; square < Squares.Length; square++)
            {
                var piece = Squares[square];
                if (piece.Type == PieceType.Empty)
                {
                    EmptySquares[square] = true;
                }
                else if (piece.Color == SideToMove)
                {
                    switch (piece.Type)
                    {
                        case PieceType.Pawn:
                            PawnPieceList.Add((Pawn)piece);
                            break;
                        case PieceType.Rook:
                            RookPieceList.Add((Rook)piece);
                            break;
                        case PieceType.Knight:
                            KnightPieceList.Add((Knight)piece);
                            break;
                        case PieceType.Bishop:
                            BishopPieceList.Add((Bishop)piece);
                            break;
                        case PieceType.Queen:
                            QueenPieceList.Add((Queen)piece);
                            break;
                        case PieceType.King:
                            KingPieceList.Add((King)piece);
                            break;
                    }
                }
            }
        }

        public void GenerateMoves(IEnumerable<Piece> pieceList)
        {
            foreach (var piece in pieceList)
            {
                switch (piece.Type)
                {
                    case PieceType.Pawn:
                        GeneratePawnMoves((Pawn)piece);
                        break;
                    case PieceType.Rook:
                        GenerateSlidingPieceMoves(piece, Rook.MoveDirection);
                        break;
                    case PieceType.Knight:
                        GenerateNonSlidingPieceMoves(piece, Knight.MoveDirection);
                        break;
                    case PieceType.Bishop:
                        GenerateSlidingPieceMoves(piece, Bishop.MoveDirection);
                        break;
                    case PieceType.Queen:
                        GenerateSlidingPieceMoves(piece, Queen.MoveDirection);
                        break;
                    case PieceType.King:
                        GenerateNonSlidingPieceMoves(piece, King.MoveDirection);
                        break;
                }
            }
        }

        private void GeneratePawnMoves(Pawn pawn)
        {
            var rank7 = pawn.Square >= 81 && pawn.Square <= 88;
            var rank2 = pawn.Square >= 31 && pawn.Square <= 38;
            if (pawn.Color == Color.White)
            {
                if (EmptySquares[pawn.Square + 10])
                {
                    AddPawnMove(pawn, pawn.Square + 10);
                    if (rank2 && EmptySquares[pawn.Square + 20])
                    {
                        AddQuietMove(pawn, pawn.Square + 20);
                    }
                }
                var possibleCaptureSquares = new[] { pawn.Square + 9, pawn.Square + 11 };
                foreach (var possibleCaptureSquare in possibleCaptureSquares)
                {
                    if (Squares[possibleCaptureSquare].Type != PieceType.OffBoard &&
                        Squares[possibleCaptureSquare].Color == Color.Black)
                    {
                        AddCaptureMove(pawn, possibleCaptureSquare);
                    }
                }

                var possibleEnPassantSquares = new[] { pawn.Square + 9, pawn.Square + 11 };
                if (EnPassantSquare != 0)
                {
                    foreach (var possibleEnPassantSquare in possibleEnPassantSquares)
                    {
                        if (possibleEnPassantSquare == EnPassantSquare)
                        {
                            AddEnPassantMove(pawn, possibleEnPassantSquare);
                        }
                    }
                }
            }
            if (pawn.Color == Color.Black)
            {
                if (EmptySquares[pawn.Square - 10])
                {
                    AddPawnMove(pawn, pawn.Square - 10);
                    if (rank7 && EmptySquares[pawn.Square - 20])
                    {
                        AddQuietMove(pawn, pawn.Square - 20);
                    }
                }
                var possibleCaptureSquares = new[] { pawn.Square - 9, pawn.Square - 11 };
                foreach (var possibleCaptureSquare in possibleCaptureSquares)
                {
                    if (Squares[possibleCaptureSquare].Type != PieceType.OffBoard &&
                        Squares[possibleCaptureSquare].Color == Color.White)
                    {
                        AddCaptureMove(pawn, possibleCaptureSquare);
                    }
                }

                var possibleEnPassantSquares = new[] { pawn.Square - 9, pawn.Square - 11 };
                if (EnPassantSquare != 0)
                {
                    foreach (var possibleEnPassantSquare in possibleEnPassantSquares)
                    {
                        if (possibleEnPassantSquare == EnPassantSquare)
                        {
                            AddEnPassantMove(pawn, possibleEnPassantSquare);
                        }
                    }
                }
            }
        }

        private void AddQuiteMove(Piece piece, int square)
        {
            Moves.Add(new Move(piece, square));
        }

        private void AddEnPassantMove(Piece piece, int square)
        {
            Moves.Add(new Move(piece, square));
        }

        private void AddCaptureMove(Piece piece, int possibleCaptureSquare)
        {
            Moves.Add(new Move(piece, possibleCaptureSquare));
        }

        private void AddQuietMove(Piece piece, int to)
        {
            Moves.Add(new Move(piece, to));
        }

        private void AddPawnMove(Pawn pawn, int to)
        {
            if (pawn.Color == Color.White && to >= Lookups.H1 && to <= Lookups.H8)
            {
                // promoted
                Moves.Add(new Move(pawn, to, new Queen()));
                Moves.Add(new Move(pawn, to, new Rook()));
                Moves.Add(new Move(pawn, to, new Bishop()));
                Moves.Add(new Move(pawn, to, new Knight()));
            }
            else if (pawn.Color == Color.Black && to >= Lookups.A1 && to <= Lookups.A8)
            {
                Moves.Add(new Move(pawn, to, new Queen()));
                Moves.Add(new Move(pawn, to, new Rook()));
                Moves.Add(new Move(pawn, to, new Bishop()));
                Moves.Add(new Move(pawn, to, new Knight()));
            }
            else
            {
                Moves.Add(new Move(pawn, to));
            }
        }

        private void GenerateSlidingPieceMoves(Piece piece, IEnumerable<int> directions)
        {
            if (piece.Type != PieceType.Rook && piece.Type != PieceType.Queen && piece.Type != PieceType.Bishop)
            {
                throw new ArgumentException("piece");
            }
            foreach (var direction in directions)
            {
                var testSquare = piece.Square + direction;
                var pieceToTest = Squares[testSquare];

                while (pieceToTest.Type != PieceType.OffBoard)
                {
                    if (pieceToTest.Color == piece.Color) break;
                    if (pieceToTest.Type == PieceType.Empty)
                    {
                        AddQuiteMove(piece, testSquare);
                    }
                    else if (pieceToTest.Color != piece.Color)
                    {
                        AddCaptureMove(piece, testSquare);
                        break;
                    }
                    testSquare += direction;
                    pieceToTest = Squares[testSquare];
                }
            }
        }

        private void GenerateNonSlidingPieceMoves(Piece piece, IEnumerable<int> directions)
        {
            foreach (var direction in directions)
            {
                var pieceToTest = Squares[piece.Square + direction];
                if (pieceToTest.Type != PieceType.OffBoard)
                {
                    if (pieceToTest.Type == PieceType.Empty)
                    {
                        AddQuiteMove(piece, piece.Square + direction);
                    }
                    else if (pieceToTest.Color != piece.Color)
                    {
                        AddCaptureMove(piece, piece.Square + direction);
                    }
                }
            }
            if (piece.Type == PieceType.King)
            {
                GenerateCastlingMoves((King)piece);
            }
        }

        private void GenerateCastlingMoves(King king)
        {
            if (king.Color == Color.White)
            {
                if (CanCastle(CastlePermissions.WhiteQueen, new[] { Lookups.A2, Lookups.A3, Lookups.A4 }, Color.Black))
                {
                    AddCastleMove(king, CastlePermissions.WhiteQueen);
                }

                if (CanCastle(CastlePermissions.WhiteKing, new[] { Lookups.A6, Lookups.A7 }, Color.Black))
                {
                    AddCastleMove(king, CastlePermissions.WhiteKing);
                }
            }
            else
            {
                if (CanCastle(CastlePermissions.BlackQueen, new[] { Lookups.H2, Lookups.H3, Lookups.H4 }, Color.White))
                {
                    AddCastleMove(king, CastlePermissions.BlackQueen);
                }

                if (CanCastle(CastlePermissions.BlackKing, new[] { Lookups.H6, Lookups.H7 }, Color.White))
                {
                    AddCastleMove(king, CastlePermissions.BlackKing);
                }
            }
        }

        private bool CanCastle(CastlePermissions permission, IEnumerable<int> squaresToCheck, Color colorToCheck)
        {
            var canCastle = false;
            if ((CastlePermission & permission) > 0)
            {
                canCastle = true;
                foreach (var ndx in squaresToCheck)
                {
                    if (Squares[ndx].Type != PieceType.Empty || IsSquareAttacked(ndx, colorToCheck))
                    {
                        canCastle = false;
                    }
                }
            }
            return canCastle;
        }

        private void AddCastleMove(King king, CastlePermissions castlePermissions)
        {
            int toSquare = 0;
            if (castlePermissions == CastlePermissions.WhiteQueen)
            {
                toSquare = Lookups.A2;
            }
            if (castlePermissions == CastlePermissions.WhiteKing)
            {
                toSquare = Lookups.A7;
            }
            if (castlePermissions == CastlePermissions.BlackQueen)
            {
                toSquare = Lookups.H2;
            }
            if (castlePermissions == CastlePermissions.BlackKing)
            {
                toSquare = Lookups.H7;
            }
            Moves.Add(new Move(king, toSquare, castlePermissions));
        }

        private bool IsSquareAttackedByARook(int square, Color side)
        {
            var thisSideColor = side == Color.White ? Color.Black : Color.White;
            foreach (var direction in Rook.MoveDirection)
            {
                var testSquare = direction;
                var piece = Squares[square + testSquare];
                while (IsValidAttackingPiece(piece, thisSideColor))
                {
                    if (piece.Color == side && piece.Type == PieceType.Rook ||
                        piece.Color == side && piece.Type == PieceType.Queen) return true;

                    testSquare += direction;
                    piece = Squares[square + testSquare];
                }
            }
            return false;
        }

        private bool IsSquareAttackedByABishop(int square, Color side)
        {
            var thisSideColor = side == Color.White ? Color.Black : Color.White;
            foreach (var direction in Bishop.MoveDirection)
            {
                var testSquare = direction;
                var piece = Squares[square + testSquare];
                while (IsValidAttackingPiece(piece, thisSideColor))
                {
                    if (piece.Color == side && piece.Type == PieceType.Bishop ||
                        piece.Color == side && piece.Type == PieceType.Queen) return true;

                    testSquare += direction;
                    piece = Squares[square + testSquare];
                }
            }
            return false;
        }

        private static bool IsValidAttackingPiece(Piece piece, Color thisSideColor)
        {
            return piece.Type != PieceType.OffBoard && thisSideColor != piece.Color;
        }

        private bool IsAttackedByAKing(int square, Color side)
        {
            foreach (var direction in King.MoveDirection)
            {
                var piece = Squares[square + direction];
                if (piece.Color == side && piece.Type == PieceType.King) return true;
            }
            return false;
        }

        private bool IsAttackedByAKnight(int square, Color side)
        {
            foreach (var direction in Knight.MoveDirection)
            {
                var piece = Squares[square + direction];
                if (piece.Color == side && piece.Type == PieceType.Knight) return true;
            }
            return false;
        }

        private bool IsAttackedByPawn(int square, Color side)
        {
            if (side == Color.White)
            {
                var piece = Squares[square - 11];
                if (piece.Color == side && piece.Type == PieceType.Pawn) return true;
                piece = Squares[square - 9];
                if (piece.Color == side && piece.Type == PieceType.Pawn) return true;
            }
            else
            {
                var piece = Squares[square + 11];
                if (piece.Color == side && piece.Type == PieceType.Pawn) return true;
                piece = Squares[square + 9];
                if (piece.Color == side && piece.Type == PieceType.Pawn) return true;
            }
            return false;
        }

        public void GenerateMoves()
        {
            GenerateMoves(PawnPieceList);
            GenerateMoves(KnightPieceList);
            GenerateMoves(BishopPieceList);
            GenerateMoves(RookPieceList);
            GenerateMoves(QueenPieceList);
            GenerateMoves(KingPieceList);
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
                // todo: position keys
                //finalKey |= squares[ndxSquares].PositionKeys[ndxSquares];
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

