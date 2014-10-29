using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class Board
    {
        public Board()
        {
            Squares = new Piece[Lookups.BoardSize];
            History = new List<History>(Lookups.Maxmoves);
            SideToMove = Color.White;
            Material = new int[2];
            CountOfEachPiece = new int[12];
            ActivePieces = new List<Piece>();
            ResetBoard();
        }

        private readonly ZobristHashing _hashing = new ZobristHashing();
        public int PositionKey;

        protected int GeneratePositionKey()
        {
            int finalKey = 0;
            for (int i = 0; i < Squares.Length; i++)
            {
                if (Squares[i].Type != PieceType.OffBoard && Squares[i].Type != PieceType.Empty)
                {
                    finalKey ^= _hashing.GetKeyForPiece(Squares[i]);
                }
            }

            if (SideToMove == Color.White)
            {
                finalKey ^= _hashing.GetSideKey();
            }

            if (EnPassantSquare != 0)
            {
                finalKey ^= _hashing.GetEnPassantKey(EnPassantSquare);
            }

            finalKey ^= _hashing.GetCastleKey(CastlePermission);

            return finalKey;
        }

        internal void HashPiece(Piece piece)
        {
            PositionKey ^= _hashing.GetKeyForPiece(piece);
        }

        internal void HashCastle()
        {
            PositionKey ^= _hashing.GetCastleKey(CastlePermission);
        }

        internal void HashSide()
        {
            PositionKey ^= _hashing.GetSideKey();
        }

        internal void HashEnPassant(int square)
        {
            PositionKey ^= _hashing.GetEnPassantKey(square);
        }

        internal void ClearPiece(Piece piece)
        {
            HashPiece(piece);
            Squares[piece.Square] = new EmptyPiece { Square = piece.Square };
            Material[(int)SideToMove] -= piece.Value;
            RemovePieceFromPieceList(piece);
        }

        internal void AddPiece(Piece piece)
        {
            // hash piece
            HashPiece(piece);
            // add piece to board
            Squares[piece.Square] = piece;
            // update material
            Material[(int)SideToMove] += piece.Value;
            // update piece list
            AddPieceToPieceList(piece);
        }

        internal void MovePiece(Piece from, Piece to)
        {
            // hash piece out
            HashPiece(from);

            // set square to empty            
            Squares[from.Square] = new EmptyPiece { Square = from.Square };

            // add piece to board
            Squares[to.Square] = to;

            // hash piece in
            HashPiece(to);
            // update the piece list            
            RemovePieceFromPieceList(from);
            AddPieceToPieceList(to);
        }

        internal void MakeMove(Move move)
        {
            // video 36
            // CastlePerms array

            // store current side so we can check king attack later
            Color currentSide = SideToMove;
            // add poskey to history array
            var history = new History {PositionKey = PositionKey, Move = move, EnPassantSquare = EnPassantSquare, CastlePermissions = CastlePermission, FiftyMoveCount = FiftyMoveCount};
            History.Add(history);
            
            // check enpas capture - if so then remove pawn one rank up ClearPiece(to +- 10)
            if (move.IsEnPassantCapture)
            {
                if (move.PieceToMove.Color == Color.White)
                {
                    ClearPiece(Squares[move.ToSquare - 10]);
                }
                else
                {
                    ClearPiece(Squares[move.ToSquare + 10]);
                }
            }

            // check castle move - king has moved, now move rook. Switch on 'to' square and move rook calling MovePiece(to, from) for rook
            if((move.CastlePermission & (CastlePermissions) 0xFFFF) != 0)
            {
                Piece piece;
                Rook rook;
                switch (move.ToSquare)
                {
                    case Lookups.C1:
                        rook = new Rook { Square = Lookups.D1, Color = Color.White};
                        MovePiece(Squares[Lookups.A1], rook);
                        break;
                    case Lookups.G1:
                        rook = new Rook { Square = Lookups.F1, Color = Color.White };
                        MovePiece(Squares[Lookups.H1], rook);
                        break;
                    case Lookups.C8:
                        rook = new Rook { Square = Lookups.D8, Color = Color.Black };
                        MovePiece(Squares[Lookups.D8], rook);
                        break;
                    case Lookups.G8:
                        rook = new Rook { Square = Lookups.F8, Color = Color.Black };
                        MovePiece(Squares[Lookups.F8], rook);
                        break;
                }
            }
            // if enpas is set, hash it out

            // add other bits to the game board history (move, fifty, enpas, castle perms) - 
            // not sure this needs to be here - can we add all these bits when we create the history?
            
            // update castle perms using from and to square pieces
            // hash in castle perms
            // is it a captured piece
            //    ClearPiece(to)
            //    reset fifty move count
            // update hisply
            HistoryPly++;
            // update game ply
            Ply++;

            // if pawn move
            //    reset 50 move
            //    if pawn start
            //      set enpas square (for white or black)
            //      hash enpas square
            // Move(from, to)
            // if promoted
            //    Clear old piece
            //    Add new piece
            // Switch side
            // hash side
            // if kingisincheck
            //    TakeMove (take back the move)
            // return !(king is in check)

        }

        private void RemovePieceFromPieceList(Piece piece)
        {
            switch (piece.Type)
            {
                case PieceType.Pawn:
                    Piece pieceToRemove = PawnPieceList.FirstOrDefault(p => p.Square == piece.Square);
                    PawnPieceList.Remove((Pawn)pieceToRemove);
                    break;
                case PieceType.Rook:
                    pieceToRemove = RookPieceList.FirstOrDefault(p => p.Square == piece.Square);
                    RookPieceList.Remove((Rook)pieceToRemove);
                    break;
                case PieceType.Knight:
                    pieceToRemove = KnightPieceList.FirstOrDefault(p => p.Square == piece.Square);
                    KnightPieceList.Remove((Knight)pieceToRemove);
                    break;
                case PieceType.Bishop:
                    pieceToRemove = BishopPieceList.FirstOrDefault(p => p.Square == piece.Square);
                    BishopPieceList.Remove((Bishop)pieceToRemove);
                    break;
                case PieceType.Queen:
                    pieceToRemove = QueenPieceList.FirstOrDefault(p => p.Square == piece.Square);
                    QueenPieceList.Remove((Queen)pieceToRemove);
                    break;
                case PieceType.King:
                    pieceToRemove = KingPieceList.FirstOrDefault(p => p.Square == piece.Square);
                    KingPieceList.Remove((King)pieceToRemove);
                    break;
            }
        }

        private void AddPieceToPieceList(Piece piece)
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

        public Piece[] Squares { get; private set; }
        public Color SideToMove { get; set; }
        public int FiftyMoveCount { get; set; }
        public int HistoryPly { get; set; }
        public int Ply { get; set; }
        public List<History> History;
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

            PositionKey = GeneratePositionKey();

            GeneratePieceLists();
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

            FiftyMoveCount = 0;
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

        public void GeneratePieceLists()
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
                switch (piece.Type)
                {
                    case PieceType.Empty:
                        EmptySquares[square] = true;
                        break;
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

        public void GenerateMoves(IEnumerable<Piece> pieceList)
        {
            foreach (var piece in pieceList)
            {
                if (piece.Color == SideToMove)
                {
                    switch (piece.Type)
                    {
                        case PieceType.Pawn:
                            GeneratePawnMoves((Pawn) piece);
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
                        AddPawnStartMove(pawn, pawn.Square + 20);                        
                    }
                }
                var possibleCaptureSquares = new[] { pawn.Square + 9, pawn.Square + 11 };
                foreach (var possibleCaptureSquare in possibleCaptureSquares)
                {
                    if (Squares[possibleCaptureSquare].Type != PieceType.OffBoard &&
                        Squares[possibleCaptureSquare].Color == Color.Black)
                    {
                        AddCaptureMove(pawn, Squares[possibleCaptureSquare]);
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
                        AddCaptureMove(pawn, Squares[possibleCaptureSquare]);
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

        private void AddPawnStartMove(Pawn pawn, int square)
        {
            Moves.Add(new Move(pawn, square){IsPawnStartMove = true});
        }

        private void AddEnPassantMove(Piece piece, int square)
        {
            Moves.Add(new Move(piece, square){IsEnPassantCapture = true});
        }

        private void AddCaptureMove(Piece piece, Piece pieceToCapture)
        {
            Moves.Add(new Move(piece, pieceToCapture));
        }

        private void AddQuietMove(Piece piece, int to)
        {
            Moves.Add(new Move(piece, to));
        }

        private void AddPawnMove(Pawn pawn, int to)
        {
            if (pawn.Color == Color.White && to >= Lookups.A8 && to <= Lookups.H8)
            {
                // promoted
                Moves.Add(new Move(pawn, to, new Queen()));
                Moves.Add(new Move(pawn, to, new Rook()));
                Moves.Add(new Move(pawn, to, new Bishop()));
                Moves.Add(new Move(pawn, to, new Knight()));
            }
            else if (pawn.Color == Color.Black && to >= Lookups.A1 && to <= Lookups.H1)
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
                        AddQuietMove(piece, testSquare);
                    }
                    else if (pieceToTest.Color != piece.Color)
                    {
                        AddCaptureMove(piece, pieceToTest);
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
                        AddQuietMove(piece, piece.Square + direction);
                    }
                    else if (pieceToTest.Color != piece.Color)
                    {
                        AddCaptureMove(piece, pieceToTest);
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
                if (CanCastle(CastlePermissions.WhiteQueen, new[] { Lookups.B1, Lookups.C1, Lookups.D1 }, Color.Black))
                {
                    AddCastleMove(king, CastlePermissions.WhiteQueen);
                }

                if (CanCastle(CastlePermissions.WhiteKing, new[] { Lookups.F1, Lookups.G1 }, Color.Black))
                {
                    AddCastleMove(king, CastlePermissions.WhiteKing);
                }
            }
            else
            {
                if (CanCastle(CastlePermissions.BlackQueen, new[] { Lookups.B8, Lookups.C8, Lookups.D8 }, Color.White))
                {
                    AddCastleMove(king, CastlePermissions.BlackQueen);
                }

                if (CanCastle(CastlePermissions.BlackKing, new[] { Lookups.F8, Lookups.G8 }, Color.White))
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
                toSquare = Lookups.C1;
            }
            if (castlePermissions == CastlePermissions.WhiteKing)
            {
                toSquare = Lookups.G1;
            }
            if (castlePermissions == CastlePermissions.BlackQueen)
            {
                toSquare = Lookups.C8;
            }
            if (castlePermissions == CastlePermissions.BlackKing)
            {
                toSquare = Lookups.G8;
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
}
