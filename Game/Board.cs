﻿using System;
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
            for (int listNdx = 0; listNdx < Lookups.MaxDepth; listNdx++)
            {
                Moves[listNdx] = new List<Move>();
            }
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
            Squares[piece.Square] = piece.Clone();
            // update material
            Material[(int)SideToMove] += piece.Value;
            // update piece list
            AddPieceToPieceList(piece.Clone());
        }

        internal void CastlePiece(Piece from, Rook to)
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
            AddPieceToPieceList(to.Clone());
        }

        internal void MovePiece(Piece from, int toSquare)
        {
            // hash piece out
            HashPiece(from);
            RemovePieceFromPieceList(from);

            // set square to empty            
            Squares[from.Square] = new EmptyPiece { Square = from.Square };

            // add piece to board
            var to = from.Clone();
            to.Square = toSquare;

            //from.Square = toSquare;

            Squares[toSquare] = to;

            // hash piece in
            HashPiece(to);
            // update the piece list            
            AddPieceToPieceList(to.Clone());
        }


        internal bool MakeMove(Move move)
        {
            // store current side so we can check king attack later
            Color currentSide = SideToMove;
            // add poskey to history array
            var history = new History
            {
                PositionKey = PositionKey,
                Move = move,
                EnPassantSquare = EnPassantSquare,
                CastlePermissions = CastlePermission,
                FiftyMoveCount = FiftyMoveCount
            };
            History.Add(history);

            // check enpas capture - if so then remove pawn one rank up ClearPiece(to +- 10)
            MakeEnPassantCapture(move);

            // check castle move - king has moved, now move rook. Switch on 'to' square and move rook calling MovePiece(to, from) for rook
            MoveRookIfCastling(move);
            // if enpas is set, hash it out
            UpdateEnPassantSquare();

            HashOutCastlePermissions();
            // add other bits to the game board history (move, fifty, enpas, castle perms) - 
            // not sure this needs to be here - can we add all these bits when we create the history?


            // update castle perms using from and to square pieces
            UpdateCastlePermissions(move);

            ResetEnPassantSquare();

            // hash in castle perms

            HashInCastlePermissions();

            ClearCapturedPiece(move);

            UpdateThePlyCounts();

            UpdateDetailsIfPawnMove(move);

            MovePiece(move.PieceToMove, move.ToSquare);

            PromotePiece(move);

            // Switch side
            SideToMove = SideToMove == Color.Black ? Color.White : Color.Black;

            // hash side
            HashSide();


            var king = GetSideToMovesKing(currentSide);

            if (IsSideThatIsMovingsKingAttacked(king))
            {
                TakeMove();
                return false;
            }
            return true;


        }

        private bool IsSideThatIsMovingsKingAttacked(King king)
        {
            return IsSquareAttacked(king.Square, SideToMove);
        }

        private King GetSideToMovesKing(Color currentSide)
        {
            King king = KingPieceList[0];
            if (KingPieceList.Count == 2)
            {
                king = KingPieceList[0].Color == currentSide ? KingPieceList[0] : KingPieceList[1];
            }
            return king;
        }

        internal void TakeMove()
        {
            // decrement the plys
            HistoryPly--;
            Ply--;

            History item = History[HistoryPly];
            History.RemoveAt(HistoryPly);
            Move move = item.Move;

            // hash EP
            if (EnPassantSquare != 0)
            {
                HashEnPassant(EnPassantSquare);
            }

            // hashCastle()
            HashCastle();

            // set castleperm, fiftyMove and enPas from move (from history)
            CastlePermission = item.CastlePermissions;
            FiftyMoveCount = item.FiftyMoveCount;
            EnPassantSquare = item.EnPassantSquare;

            // if enpas sq set, hashep()
            if (EnPassantSquare != 0)
            {
                HashEnPassant(EnPassantSquare);
            }

            // hashCastle
            HashCastle();

            SideToMove = SideToMove == Color.White ? Color.Black : Color.White;
            HashSide();

            // if en pass capture - add a pawn
            /* if side == white add black at to-10
             * else add white at to+10
             */
            if (move.IsEnPassantCapture)
            {
                if (SideToMove == Color.White)
                {
                    AddPiece(new Pawn { Square = move.ToSquare - 10, Color = Color.Black });
                }
                else
                {
                    AddPiece(new Pawn { Square = move.ToSquare + 10, Color = Color.White });
                }
            }
            /* if(castle) put rook back*/
            if (move.IsCastleMove)
            {
                Rook rook;
                switch (move.ToSquare)
                {
                    case Lookups.C1:
                        rook = new Rook { Square = Lookups.D1, Color = Color.White };
                        MovePiece(rook, Lookups.A1);
                        break;
                    case Lookups.G1:
                        rook = new Rook { Square = Lookups.F1, Color = Color.White };
                        MovePiece(rook, Lookups.H1);
                        break;
                    case Lookups.C8:
                        rook = new Rook { Square = Lookups.D8, Color = Color.Black };
                        MovePiece(rook, Lookups.A8);
                        break;
                    case Lookups.G8:
                        rook = new Rook { Square = Lookups.F8, Color = Color.Black };
                        MovePiece(rook, Lookups.H8);
                        break;
                }

            }

            // If there's been a promotion then the
            // piece we're moving back won't be the piece we moved
            // originally, it'll be the promoted piece
            // so grab the piece from the square not from the move
            var pieceToMove = Squares[move.ToSquare];
            MovePiece(pieceToMove, move.FromSquare);
            // if capture, add piece to 'to' square

            if (move.Captured.Type != PieceType.Empty)
            {
                AddPiece(move.Captured);
            }

            // if promoted, clear (from) addPiece, either white or black depending on side to move
            if (move.PromotedTo.Type != PieceType.Empty)
            {
                ClearPiece(Squares[move.FromSquare]);
                var piece = new Pawn { Square = move.FromSquare, Color = SideToMove };
                AddPiece(piece);
            }

        }

        private void PromotePiece(Move move)
        {
            // if promoted
            if (move.PromotedTo.Type != PieceType.Empty)
            {
                //    Clear old piece
                ClearPiece(Squares[move.ToSquare]);
                //    Add new piece
                AddPiece(move.PromotedTo);
            }
        }

        private void UpdateDetailsIfPawnMove(Move move)
        {
            if (move.PieceToMove.Type == PieceType.Pawn)
            {
                FiftyMoveCount = 0;
                if (move.IsPawnStartMove)
                {
                    EnPassantSquare = move.PieceToMove.Color == Color.White
                        ? move.FromSquare + 10
                        : move.FromSquare - 10;
                    HashEnPassant(EnPassantSquare);
                }
            }
        }

        private void UpdateThePlyCounts()
        {
            HistoryPly++;
            Ply++;
        }

        private void ClearCapturedPiece(Move move)
        {
            if (move.Captured.Type != PieceType.Empty)
            {
                ClearPiece(move.Captured);
                FiftyMoveCount = 0;
            }
        }

        private void HashInCastlePermissions()
        {
            HashCastle();
        }

        private void ResetEnPassantSquare()
        {
            EnPassantSquare = 0;
        }

        private void UpdateCastlePermissions(Move move)
        {
            CastlePermission &= (CastlePermissions)Lookups.CastlePerms[move.FromSquare];
            CastlePermission &= (CastlePermissions)Lookups.CastlePerms[move.ToSquare];
        }

        private void HashOutCastlePermissions()
        {
            HashCastle();
        }

        private void UpdateEnPassantSquare()
        {
            if (EnPassantSquare != 0)
            {
                PositionKey ^= _hashing.GetEnPassantKey(EnPassantSquare);
            }
        }

        private void MoveRookIfCastling(Move move)
        {
            if (move.IsCastleMove)
            {
                Rook rook;
                switch (move.ToSquare)
                {
                    case Lookups.C1:
                        rook = new Rook { Square = Lookups.D1, Color = Color.White };
                        CastlePiece(Squares[Lookups.A1], rook);
                        break;
                    case Lookups.G1:
                        rook = new Rook { Square = Lookups.F1, Color = Color.White };
                        CastlePiece(Squares[Lookups.H1], rook);
                        break;
                    case Lookups.C8:
                        rook = new Rook { Square = Lookups.D8, Color = Color.Black };
                        CastlePiece(Squares[Lookups.A8], rook);
                        break;
                    case Lookups.G8:
                        rook = new Rook { Square = Lookups.F8, Color = Color.Black };
                        CastlePiece(Squares[Lookups.H8], rook);
                        break;
                }
            }
        }

        private void MakeEnPassantCapture(Move move)
        {
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
        }

        private void RemovePieceFromPieceList(Piece piece)
        {
            Piece pieceToRemove;
            switch (piece.Type)
            {
                case PieceType.Pawn:
                    for (int i = 0; i < PawnPieceList.Count; i++)
                    {
                        if (PawnPieceList[i].Square == piece.Square)
                        {
                            PawnPieceList.RemoveAt(i);
                            break;
                        }
                    }                    
                    break;
                case PieceType.Rook:
                    for (int i = 0; i < RookPieceList.Count; i++)
                    {
                        if (RookPieceList[i].Square == piece.Square)
                        {
                            RookPieceList.RemoveAt(i);
                            break;
                        }
                    }
                    break;
                case PieceType.Knight:                    
                    for (int i = 0; i < KnightPieceList.Count; i++)
                    {
                        if (KnightPieceList[i].Square == piece.Square)
                        {
                            KnightPieceList.RemoveAt(i);
                            break;
                        }
                    }
                    break;
                case PieceType.Bishop:
                    for (int i = 0; i < BishopPieceList.Count; i++)
                    {
                        if (BishopPieceList[i].Square == piece.Square)
                        {
                            BishopPieceList.RemoveAt(i);
                            break;
                        }
                    }
                    break;
                case PieceType.Queen:
                    for (int i = 0; i < QueenPieceList.Count; i++)
                    {
                        if (QueenPieceList[i].Square == piece.Square)
                        {
                            QueenPieceList.RemoveAt(i);
                            break;
                        }
                    }
                    break;
                case PieceType.King:
                    for (int i = 0; i < KingPieceList.Count; i++)
                    {
                        if (KingPieceList[i].Square == piece.Square)
                        {
                            KingPieceList.RemoveAt(i);
                            break;
                        }
                    }
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
                for (int file = 0; file <= 7; file++)
                {
                    int boardNdx = Lookups.FileRankToSquare(file, rank);
                    var piece = Squares[boardNdx];
                    builder.Append(piece);
                }
                builder.Append("/");
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

        public List<Move>[] Moves = new List<Move>[Lookups.MaxDepth];

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
        }

        private void GeneratePawnMoves(Pawn pawn)
        {
            var rank7 = pawn.Square >= 81 && pawn.Square <= 88;
            var rank2 = pawn.Square >= 31 && pawn.Square <= 38;
            if (pawn.Color == Color.White)
            {
                if (Squares[pawn.Square + 10].Type == PieceType.Empty)
                {
                    AddPawnMove(pawn, pawn.Square + 10);
                    if (rank2 && Squares[pawn.Square + 20].Type == PieceType.Empty)
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
                if (Squares[pawn.Square - 10].Type == PieceType.Empty)
                {
                    AddPawnMove(pawn, pawn.Square - 10);
                    if (rank7 && Squares[pawn.Square - 20].Type == PieceType.Empty)
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
            Moves[Ply].Add(new Move(pawn, square) { IsPawnStartMove = true });
        }

        private void AddEnPassantMove(Piece piece, int square)
        {
            Moves[Ply].Add(new Move(piece, square) { IsEnPassantCapture = true });
        }

        private void AddCaptureMove(Piece piece, Piece pieceToCapture)
        {
            Moves[Ply].Add(new Move(piece, pieceToCapture));
        }

        private void AddQuietMove(Piece piece, int to)
        {
            Moves[Ply].Add(new Move(piece, to));
        }

        private void AddPawnMove(Pawn pawn, int to)
        {
            if (pawn.Color == Color.White && to >= Lookups.A8 && to <= Lookups.H8)
            {
                // promoted
                Moves[Ply].Add(new Move(pawn, to, new Queen()));
                Moves[Ply].Add(new Move(pawn, to, new Rook()));
                Moves[Ply].Add(new Move(pawn, to, new Bishop()));
                Moves[Ply].Add(new Move(pawn, to, new Knight()));
            }
            else if (pawn.Color == Color.Black && to >= Lookups.A1 && to <= Lookups.H1)
            {
                Moves[Ply].Add(new Move(pawn, to, new Queen()));
                Moves[Ply].Add(new Move(pawn, to, new Rook()));
                Moves[Ply].Add(new Move(pawn, to, new Bishop()));
                Moves[Ply].Add(new Move(pawn, to, new Knight()));
            }
            else
            {
                Moves[Ply].Add(new Move(pawn, to));
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
            Moves[Ply].Add(new Move(king, toSquare, true));
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
                    if (piece.Color == side)
                    {
                        if (piece.Type == PieceType.Rook || piece.Type == PieceType.Queen)
                        {
                            return true;
                        }
                        // piece is the same color but can't attack so blocks anything behind
                        break;
                    }

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
                    if (piece.Color == side)
                    {
                        if (piece.Type == PieceType.Bishop || piece.Type == PieceType.Queen)
                        {
                            return true;
                        }
                        // piece is the same color but can't attack so blocks anything behind
                        break;
                    }

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
            Moves[Ply].Clear();
            GenerateMoves(PawnPieceList);
            GenerateMoves(KnightPieceList);
            GenerateMoves(BishopPieceList);
            GenerateMoves(RookPieceList);
            GenerateMoves(QueenPieceList);
            GenerateMoves(KingPieceList);
        }
    }

}