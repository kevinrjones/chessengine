using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    public class Board
    {
        public Board()
        {
            Squares = new PieceType[Lookups.BoardSize];
            History = new List<History>(Lookups.Maxmoves);
            SideToMove = Color.White;
            Material = new int[2];
            CountOfEachPieceType = new int[12];
            ActivePieces = new List<PieceType>();
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
                if (Squares[i] != PieceType.OffBoard && Squares[i] != PieceType.Empty)
                {
                    finalKey ^= _hashing.GetKeyForPiece(Squares[i], i);
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

        internal void HashPieceType(PieceType pieceType, int square)
        {
            PositionKey ^= _hashing.GetKeyForPiece(pieceType, square);
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

        internal void ClearPiece(PieceType pieceType, int square)
        {
            HashPieceType(pieceType, square);
            Squares[square] = PieceType.Empty;
            Material[(int)SideToMove] -= Lookups.PieceValues[pieceType].Value;
            RemovePieceTypeFromPieceTypeList(pieceType, square);
        }

        internal void AddPiece(PieceType pieceType, int square)
        {
            // hash PieceType
            HashPieceType(pieceType, square);
            // add PieceType to board
            Squares[square] = pieceType;
            // update material
            Material[(int)SideToMove] += Lookups.PieceValues[pieceType].Value;
            // update PieceType list
            AddPieceToPieceList(pieceType, square);
        }

        internal void CastlePieceType(int fromSquare, int toSquare)
        {
            PieceType rook = toSquare == Lookups.D1 || toSquare == Lookups.F1 ? PieceType.WhiteRook : PieceType.BlackRook;
            // hash PieceType out
            HashPieceType(rook, fromSquare);

            // set square to empty            
            Squares[fromSquare] = PieceType.Empty;

            // add PieceType to board
            Squares[toSquare] = rook;

            // hash PieceType in
            HashPieceType(rook, toSquare);
            // update the PieceType list            
            RemovePieceTypeFromPieceTypeList(rook, fromSquare);
            AddPieceToPieceList(rook, toSquare);
        }

        internal void MovePiece(PieceType from, int fromSquare, int toSquare)
        {
            // hash PieceType out
            HashPieceType(from, fromSquare);
            RemovePieceTypeFromPieceTypeList(from, fromSquare);

            // set square to empty            
            Squares[fromSquare] = PieceType.Empty;

            // add PieceType to board

            Squares[toSquare] = from;

            // hash PieceType in
            HashPieceType(from, toSquare);
            // update the PieceType list            
            AddPieceToPieceList(from, toSquare);
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

            // check enpas capture - if so then remove pawn one rank up ClearPieceType(to +- 10)
            MakeEnPassantCapture(move);

            // check castle move - king has moved, now move rook. Switch on 'to' square and move rook calling MovePieceType(to, from) for rook
            MoveRookIfCastling(move);

            // if enpas is set, hash it out
            UpdateEnPassantSquare();

            HashOutCastlePermissions();


            // update castle perms using from and to square Pieces
            UpdateCastlePermissions(move);

            ResetEnPassantSquare();

            // hash in castle perms
            HashInCastlePermissions();

            ClearCapturedPieceType(move);

            UpdateThePlyCounts();

            UpdateDetailsIfPawnMove(move);

            MovePiece(move.FromPiece, move.FromSquare, move.ToSquare);

            PromotePieceType(move);

            // Switch side

            SideToMove = SideToMove == Color.Black ? Color.White : Color.Black;

            // hash side
            HashSide();


            int kingSquare = GetSideToMovesKing(currentSide);

            // get king's square? PieceList?
            Color sideThatMoved = SideToMove == Color.White ? Color.Black : Color.White;

            if (IsSideThatIsMovingsKingAttacked(kingSquare, sideThatMoved))
            {
                TakeMove();
                return false;
            }
            return true;


        }

        private bool IsSideThatIsMovingsKingAttacked(int kingSquare, Color sideMoving)
        {
            return IsSquareAttacked(kingSquare, sideMoving);
        }

        private int GetSideToMovesKing(Color currentSide)
        {
            return currentSide == Color.White ? WhiteKingPieceTypeList[0] : BlackKingPieceTypeList[0];
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
                    AddPiece(PieceType.BlackPawn, move.ToSquare - 10);
                }
                else
                {
                    AddPiece(PieceType.WhitePawn, move.ToSquare + 10);
                }
            }
            /* if(castle) put rook back*/
            if (move.IsCastleMove)
            {
                //Rook rook;
                switch (move.ToSquare)
                {
                    case Lookups.C1:
                        //rook = new Rook { Square = Lookups.D1, Color = Color.White };
                        MovePiece(PieceType.WhiteRook, Lookups.D1, Lookups.A1);
                        break;
                    case Lookups.G1:
                        //rook = new Rook { Square = Lookups.F1, Color = Color.White };
                        MovePiece(PieceType.WhiteRook, Lookups.F1, Lookups.H1);
                        break;
                    case Lookups.C8:
                        //rook = new Rook { Square = Lookups.D8, Color = Color.Black };
                        MovePiece(PieceType.BlackRook, Lookups.D8, Lookups.A8);
                        break;
                    case Lookups.G8:
                        //rook = new Rook { Square = Lookups.F8, Color = Color.Black };
                        MovePiece(PieceType.BlackRook, Lookups.F8, Lookups.H8);
                        break;
                }

            }

            // If there's been a promotion then the
            // PieceType we're moving back won't be the PieceType we moved
            // originally, it'll be the promoted PieceType
            // so grab the PieceType from the square not from the move
            var pieceTypeToMove = Squares[move.ToSquare];
            MovePiece(pieceTypeToMove, move.ToSquare, move.FromSquare);
            // if capture, add PieceType to 'to' square

            if (move.Captured != PieceType.Empty)
            {
                AddPiece(move.Captured, move.ToSquare);
            }

            // if promoted, clear (from) addPieceType, either white or black depending on side to move
            if (move.PromotedTo != PieceType.Empty)
            {
                ClearPiece(Squares[move.FromSquare], move.FromSquare);
                //var PieceType = new PieceType { Square = move.FromSquare, Color = SideToMove };
                var piece = SideToMove == Color.White ? PieceType.WhitePawn : PieceType.BlackPawn;
                AddPiece(piece, move.FromSquare);
            }

        }

        private void PromotePieceType(Move move)
        {
            // if promoted
            if (move.PromotedTo != PieceType.Empty)
            {
                //    Clear old PieceType
                ClearPiece(Squares[move.ToSquare], move.ToSquare);
                //    Add new PieceType
                AddPiece(move.PromotedTo, move.ToSquare);
            }
        }

        private void UpdateDetailsIfPawnMove(Move move)
        {
            if (move.FromPiece == PieceType.WhitePawn || move.FromPiece == PieceType.BlackPawn)
            {
                FiftyMoveCount = 0;
                if (move.IsPawnStartMove)
                {
                    EnPassantSquare = SideToMove == Color.White
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

        private void ClearCapturedPieceType(Move move)
        {
            if (move.Captured != PieceType.Empty)
            {
                ClearPiece(move.Captured, move.ToSquare);
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
                //CastlePieceType(int fromSquare,  int toSquare)
                switch (move.ToSquare)
                {
                    case Lookups.C1:
                        //rook = new Rook { Square = Lookups.D1, Color = Color.White };
                        CastlePieceType(Lookups.A1, Lookups.D1);
                        break;
                    case Lookups.G1:
                        //rook = new Rook { Square = Lookups.F1, Color = Color.White };
                        CastlePieceType(Lookups.H1, Lookups.F1);
                        break;
                    case Lookups.C8:
                        //rook = new Rook { Square = Lookups.D8, Color = Color.Black };
                        CastlePieceType(Lookups.A8, Lookups.D8);
                        break;
                    case Lookups.G8:
                        //rook = new Rook { Square = Lookups.F8, Color = Color.Black };
                        CastlePieceType(Lookups.H8, Lookups.F8);
                        break;
                }
            }
        }

        private void MakeEnPassantCapture(Move move)
        {
            if (move.IsEnPassantCapture)
            {
                if (SideToMove == Color.White)
                {
                    ClearPiece(Squares[move.ToSquare - 10], move.ToSquare - 10);
                }
                else
                {
                    ClearPiece(Squares[move.ToSquare + 10], move.ToSquare + 10);
                }
            }
        }

        private void RemovePieceTypeFromPieceTypeList(PieceType pieceType, int square)
        {
            int pieceTypeNum;
            int count;
            Color color = pieceType < (PieceType)7 ? Color.White : Color.Black;
            switch (pieceType)
            {
                case PieceType.WhitePawn:
                case PieceType.BlackPawn:
                    switch (color)
                    {
                        case Color.Black:
                            pieceTypeNum = (int)Lookups.Pieces.bP;
                            count = Lookups.PieceCounts[pieceTypeNum];
                            for (int i = 0; i < count; i++)
                            {
                                if (BlackPawnPieceTypeList[i] == square)
                                {
                                    BlackPawnPieceTypeList[i] = BlackPawnPieceTypeList[count - 1];
                                    Lookups.PieceCounts[pieceTypeNum]--;
                                    break;
                                }
                            }
                            break;
                        case Color.White:
                            pieceTypeNum = (int)Lookups.Pieces.wP;
                            count = Lookups.PieceCounts[pieceTypeNum];
                            for (int i = 0; i < count; i++)
                            {
                                if (WhitePawnPieceTypeList[i] == square)
                                {
                                    WhitePawnPieceTypeList[i] = WhitePawnPieceTypeList[count - 1];
                                    Lookups.PieceCounts[pieceTypeNum]--;
                                    break;
                                }
                            }
                            break;
                    }
                    break;
                case PieceType.WhiteRook:
                case PieceType.BlackRook:
                    switch (color)
                    {
                        case Color.Black:
                            pieceTypeNum = (int)Lookups.Pieces.bR;
                            count = Lookups.PieceCounts[pieceTypeNum];
                            for (int i = 0; i < count; i++)
                            {
                                if (BlackRookPieceTypeList[i] == square)
                                {
                                    BlackRookPieceTypeList[i] = BlackRookPieceTypeList[count - 1];
                                    Lookups.PieceCounts[pieceTypeNum]--;
                                    break;
                                }
                            }
                            break;
                        case Color.White:
                            pieceTypeNum = (int)Lookups.Pieces.wR;
                            count = Lookups.PieceCounts[pieceTypeNum];
                            for (int i = 0; i < count; i++)
                            {
                                if (WhiteRookPieceTypeList[i] == square)
                                {
                                    WhiteRookPieceTypeList[i] = WhiteRookPieceTypeList[count - 1];
                                    Lookups.PieceCounts[pieceTypeNum]--;
                                    break;
                                }
                            }
                            break;
                    }
                    break;
                case PieceType.WhiteKnight:
                case PieceType.BlackKnight:
                    switch (color)
                    {
                        case Color.Black:
                            pieceTypeNum = (int)Lookups.Pieces.bN;
                            count = Lookups.PieceCounts[pieceTypeNum];
                            for (int i = 0; i < count; i++)
                            {
                                if (BlackKnightPieceTypeList[i] == square)
                                {
                                    BlackKnightPieceTypeList[i] = BlackKnightPieceTypeList[count - 1];
                                    Lookups.PieceCounts[pieceTypeNum]--;
                                    break;
                                }
                            }
                            break;
                        case Color.White:
                            pieceTypeNum = (int)Lookups.Pieces.wN;
                            count = Lookups.PieceCounts[pieceTypeNum];
                            for (int i = 0; i < count; i++)
                            {
                                if (WhiteKnightPieceTypeList[i] == square)
                                {
                                    WhiteKnightPieceTypeList[i] = WhiteKnightPieceTypeList[count - 1];
                                    Lookups.PieceCounts[pieceTypeNum]--;
                                    break;
                                }
                            }
                            break;
                    }
                    break;
                case PieceType.WhiteBishop:
                case PieceType.BlackBishop:
                    switch (color)
                    {
                        case Color.Black:
                            pieceTypeNum = (int)Lookups.Pieces.bB;
                            count = Lookups.PieceCounts[pieceTypeNum];
                            for (int i = 0; i < count; i++)
                            {
                                if (BlackBishopPieceTypeList[i] == square)
                                {
                                    BlackBishopPieceTypeList[i] = BlackBishopPieceTypeList[count - 1];
                                    Lookups.PieceCounts[pieceTypeNum]--;
                                    break;
                                }
                            }
                            break;
                        case Color.White:
                            pieceTypeNum = (int)Lookups.Pieces.wB;
                            count = Lookups.PieceCounts[pieceTypeNum];
                            for (int i = 0; i < count; i++)
                            {
                                if (WhiteBishopPieceTypeList[i] == square)
                                {
                                    WhiteBishopPieceTypeList[i] = WhiteBishopPieceTypeList[count - 1];
                                    Lookups.PieceCounts[pieceTypeNum]--;
                                    break;
                                }
                            }
                            break;
                    }
                    break;
                case PieceType.WhiteQueen:
                case PieceType.BlackQueen:
                    switch (color)
                    {
                        case Color.Black:
                            pieceTypeNum = (int)Lookups.Pieces.bQ;
                            count = Lookups.PieceCounts[pieceTypeNum];
                            for (int i = 0; i < count; i++)
                            {
                                if (BlackQueenPieceTypeList[i] == square)
                                {
                                    BlackQueenPieceTypeList[i] = BlackQueenPieceTypeList[count - 1];
                                    Lookups.PieceCounts[pieceTypeNum]--;
                                    break;
                                }
                            }
                            break;
                        case Color.White:
                            pieceTypeNum = (int)Lookups.Pieces.wQ;
                            count = Lookups.PieceCounts[pieceTypeNum];
                            for (int i = 0; i < count; i++)
                            {
                                if (WhiteQueenPieceTypeList[i] == square)
                                {
                                    WhiteQueenPieceTypeList[i] = WhiteQueenPieceTypeList[count - 1];
                                    Lookups.PieceCounts[pieceTypeNum]--;
                                    break;
                                }
                            }
                            break;
                    }
                    break;
                case PieceType.WhiteKing:
                case PieceType.BlackKing:
                    switch (color)
                    {
                        case Color.Black:
                            pieceTypeNum = (int)Lookups.Pieces.bK;
                            count = Lookups.PieceCounts[pieceTypeNum];
                            for (int i = 0; i < count; i++)
                            {
                                if (BlackKingPieceTypeList[i] == square)
                                {
                                    BlackKingPieceTypeList[i] = BlackKingPieceTypeList[count - 1];
                                    Lookups.PieceCounts[pieceTypeNum]--;
                                    break;
                                }
                            }
                            break;
                        case Color.White:
                            pieceTypeNum = (int)Lookups.Pieces.wK;
                            count = Lookups.PieceCounts[pieceTypeNum];
                            for (int i = 0; i < count; i++)
                            {
                                if (WhiteKingPieceTypeList[i] == square)
                                {
                                    WhiteKingPieceTypeList[i] = WhiteKingPieceTypeList[count - 1];
                                    Lookups.PieceCounts[pieceTypeNum]--;
                                    break;
                                }
                            }
                            break;
                    }
                    break;
            }
        }

        private void AddPieceToPieceList(PieceType pieceType, int square)
        {
            int pieceTypeNum;
            Color color = pieceType < (PieceType)7 ? Color.White : Color.Black;
            switch (pieceType)
            {
                case PieceType.WhitePawn:
                case PieceType.BlackPawn:
                    switch (color)
                    {
                        case Color.Black:
                            pieceTypeNum = (int)Lookups.Pieces.bP;
                            BlackPawnPieceTypeList[Lookups.PieceCounts[pieceTypeNum]] = square;
                            Lookups.PieceCounts[pieceTypeNum]++;
                            break;
                        case Color.White:
                            pieceTypeNum = (int)Lookups.Pieces.wP;
                            WhitePawnPieceTypeList[Lookups.PieceCounts[pieceTypeNum]] = square;
                            Lookups.PieceCounts[pieceTypeNum]++;
                            break;
                    }
                    break;
                case PieceType.WhiteRook:
                case PieceType.BlackRook:
                    switch (color)
                    {
                        case Color.Black:
                            pieceTypeNum = (int)Lookups.Pieces.bR;
                            BlackRookPieceTypeList[Lookups.PieceCounts[pieceTypeNum]] = square;
                            Lookups.PieceCounts[pieceTypeNum]++;
                            break;
                        case Color.White:
                            pieceTypeNum = (int)Lookups.Pieces.wR;
                            WhiteRookPieceTypeList[Lookups.PieceCounts[pieceTypeNum]] = square;
                            Lookups.PieceCounts[pieceTypeNum]++;
                            break;
                    }
                    break;
                case PieceType.WhiteKnight:
                case PieceType.BlackKnight:
                    switch (color)
                    {
                        case Color.Black:
                            pieceTypeNum = (int)Lookups.Pieces.bN;
                            BlackKnightPieceTypeList[Lookups.PieceCounts[pieceTypeNum]] = square;
                            Lookups.PieceCounts[pieceTypeNum]++;
                            break;
                        case Color.White:
                            pieceTypeNum = (int)Lookups.Pieces.wN;
                            WhiteKnightPieceTypeList[Lookups.PieceCounts[pieceTypeNum]] = square;
                            Lookups.PieceCounts[pieceTypeNum]++;
                            break;
                    }
                    break;
                case PieceType.WhiteBishop:
                case PieceType.BlackBishop:
                    switch (color)
                    {
                        case Color.Black:
                            pieceTypeNum = (int)Lookups.Pieces.bB;
                            BlackBishopPieceTypeList[Lookups.PieceCounts[pieceTypeNum]] = square;
                            Lookups.PieceCounts[pieceTypeNum]++;
                            break;
                        case Color.White:
                            pieceTypeNum = (int)Lookups.Pieces.wB;
                            WhiteBishopPieceTypeList[Lookups.PieceCounts[pieceTypeNum]] = square;
                            Lookups.PieceCounts[pieceTypeNum]++;
                            break;
                    }
                    break;
                case PieceType.WhiteQueen:
                case PieceType.BlackQueen:
                    switch (color)
                    {
                        case Color.Black:
                            pieceTypeNum = (int)Lookups.Pieces.bQ;
                            BlackQueenPieceTypeList[Lookups.PieceCounts[pieceTypeNum]] = square;
                            Lookups.PieceCounts[pieceTypeNum]++;
                            break;
                        case Color.White:
                            pieceTypeNum = (int)Lookups.Pieces.wQ;
                            WhiteQueenPieceTypeList[Lookups.PieceCounts[pieceTypeNum]] = square;
                            Lookups.PieceCounts[pieceTypeNum]++;
                            break;
                    }
                    break;
                case PieceType.WhiteKing:
                case PieceType.BlackKing:
                    switch (color)
                    {
                        case Color.Black:
                            pieceTypeNum = (int)Lookups.Pieces.bK;
                            BlackKingPieceTypeList[Lookups.PieceCounts[pieceTypeNum]] = square;
                            Lookups.PieceCounts[pieceTypeNum]++;
                            break;
                        case Color.White:
                            pieceTypeNum = (int)Lookups.Pieces.wK;
                            WhiteKingPieceTypeList[Lookups.PieceCounts[pieceTypeNum]] = square;
                            Lookups.PieceCounts[pieceTypeNum]++;
                            break;
                    }
                    break;
            }
        }

        public PieceType[] Squares { get; private set; }
        public Color SideToMove { get; set; }
        public int FiftyMoveCount { get; set; }
        public int HistoryPly { get; set; }
        public int Ply { get; set; }
        public List<History> History;
        public int EnPassantSquare { get; set; }
        public CastlePermissions CastlePermission { get; set; }
        public int[] Material { get; set; }
        public int[] CountOfEachPieceType { get; set; }
        public List<PieceType> ActivePieces;

        public override string ToString()
        {
            var builder = new StringBuilder();
            for (int rank = 7; rank >= 0; rank--)
            {
                for (int file = 0; file <= 7; file++)
                {
                    int boardNdx = Lookups.FileRankToSquare(file, rank);
                    var pieceType = Squares[boardNdx];
                    builder.Append(pieceType);
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

            GeneratePieceTypeLists();
        }

        private void UpdateMaterial()
        {
            for (int PieceTypeNdx = 0; PieceTypeNdx < 64; PieceTypeNdx++)
            {
                var sq = Lookups.Map64To120(PieceTypeNdx);
                var PieceType = Squares[sq];
                if (PieceType != PieceType.Empty)
                {
                    Material[(int)SideToMove] += Lookups.PieceValues[PieceType].Value;
                }
            }
        }

        internal void ResetBoard()
        {
            for (int i = 0; i < Squares.Length; i++)
            {
                Squares[i] = PieceType.OffBoard;
            }

            for (int i = 0; i < 64; i++)
            {
                Squares[Lookups.Map64To120(i)] = PieceType.Empty;
            }

            for (int i = 0; i < Material.Length; i++)
            {
                Material[i] = 0;
            }

            for (int i = 0; i < CountOfEachPieceType.Length; i++)
            {
                CountOfEachPieceType[i] = 0;
            }

            SideToMove = Color.Neither;

            FiftyMoveCount = 0;
            HistoryPly = 0;
            Ply = 0;
            EnPassantSquare = 0;
            CastlePermission = 0;
        }


        public bool IsSquareAttacked(int square, Color colorOfMovingSide)
        {
            if (IsAttackedByPawn(square, colorOfMovingSide)) return true;
            if (IsAttackedByAKnight(square, colorOfMovingSide)) return true;
            if (IsAttackedByAKing(square, colorOfMovingSide)) return true;
            if (IsSquareAttackedByABishop(square, colorOfMovingSide)) return true;
            if (IsSquareAttackedByARook(square, colorOfMovingSide)) return true;

            return false;
        }

        internal readonly int[] WhitePawnPieceTypeList = new int[10];
        internal readonly int[] BlackPawnPieceTypeList = new int[16];
        internal readonly int[] WhiteRookPieceTypeList = new int[20];
        internal readonly int[] BlackRookPieceTypeList = new int[20];
        internal readonly int[] WhiteKnightPieceTypeList = new int[14];
        internal readonly int[] BlackKnightPieceTypeList = new int[14];
        internal readonly int[] WhiteBishopPieceTypeList = new int[20];
        internal readonly int[] BlackBishopPieceTypeList = new int[20];
        internal readonly int[] WhiteQueenPieceTypeList = new int[20];
        internal readonly int[] BlackQueenPieceTypeList = new int[20];
        internal readonly int[] WhiteKingPieceTypeList = new int[10];
        internal readonly int[] BlackKingPieceTypeList = new int[10];

        internal readonly bool[] EmptySquares = new bool[120];

        public List<Move>[] Moves = new List<Move>[Lookups.MaxDepth];

        public void GeneratePieceTypeLists()
        {
            for (int i = 0; i < EmptySquares.Length; i++)
            {
                EmptySquares[0] = false;
            }

            for (int i = 0; i < Lookups.PieceCounts.Length; i++)
            {
                Lookups.PieceCounts[i] = 0;
            }

            for (var square = 0; square < Squares.Length; square++)
            {
                var pieceType = Squares[square];
                var color = pieceType < (PieceType)7 ? Color.White : Color.Black;
                if (pieceType == PieceType.OffBoard)
                {
                    continue;
                }
                int PieceTypeNum;
                switch (pieceType)
                {
                    case PieceType.Empty:
                        EmptySquares[square] = true;
                        break;
                    case PieceType.WhitePawn:
                    case PieceType.BlackPawn:
                        switch (color)
                        {
                            case Color.Black:
                                PieceTypeNum = (int)Lookups.Pieces.bP;
                                BlackPawnPieceTypeList[Lookups.PieceCounts[PieceTypeNum]] = square;
                                Lookups.PieceCounts[PieceTypeNum]++;
                                break;
                            case Color.White:
                                PieceTypeNum = (int)Lookups.Pieces.wP;
                                WhitePawnPieceTypeList[Lookups.PieceCounts[PieceTypeNum]] = square;
                                Lookups.PieceCounts[PieceTypeNum]++;
                                break;
                        }
                        break;
                    case PieceType.WhiteRook:
                    case PieceType.BlackRook:
                        switch (color)
                        {
                            case Color.Black:
                                PieceTypeNum = (int)Lookups.Pieces.bR;
                                BlackRookPieceTypeList[Lookups.PieceCounts[PieceTypeNum]] = square;
                                Lookups.PieceCounts[PieceTypeNum]++;
                                break;
                            case Color.White:
                                PieceTypeNum = (int)Lookups.Pieces.wR;
                                WhiteRookPieceTypeList[Lookups.PieceCounts[PieceTypeNum]] = square;
                                Lookups.PieceCounts[PieceTypeNum]++;
                                break;
                        }
                        break;
                    case PieceType.WhiteKnight:
                    case PieceType.BlackKnight:
                        switch (color)
                        {
                            case Color.Black:
                                PieceTypeNum = (int)Lookups.Pieces.bN;
                                BlackKnightPieceTypeList[Lookups.PieceCounts[PieceTypeNum]] = square;
                                Lookups.PieceCounts[PieceTypeNum]++;
                                break;
                            case Color.White:
                                PieceTypeNum = (int)Lookups.Pieces.wN;
                                WhiteKnightPieceTypeList[Lookups.PieceCounts[PieceTypeNum]] = square;
                                Lookups.PieceCounts[PieceTypeNum]++;
                                break;
                        }
                        break;
                    case PieceType.WhiteBishop:
                    case PieceType.BlackBishop:
                        switch (color)
                        {
                            case Color.Black:
                                PieceTypeNum = (int)Lookups.Pieces.bB;
                                BlackBishopPieceTypeList[Lookups.PieceCounts[PieceTypeNum]] = square;
                                Lookups.PieceCounts[PieceTypeNum]++;
                                break;
                            case Color.White:
                                PieceTypeNum = (int)Lookups.Pieces.wB;
                                WhiteBishopPieceTypeList[Lookups.PieceCounts[PieceTypeNum]] = square;
                                Lookups.PieceCounts[PieceTypeNum]++;
                                break;
                        }
                        break;
                    case PieceType.WhiteQueen:
                    case PieceType.BlackQueen:
                        switch (color)
                        {
                            case Color.Black:
                                PieceTypeNum = (int)Lookups.Pieces.bQ;
                                BlackQueenPieceTypeList[Lookups.PieceCounts[PieceTypeNum]] = square;
                                Lookups.PieceCounts[PieceTypeNum]++;
                                break;
                            case Color.White:
                                PieceTypeNum = (int)Lookups.Pieces.wQ;
                                WhiteQueenPieceTypeList[Lookups.PieceCounts[PieceTypeNum]] = square;
                                Lookups.PieceCounts[PieceTypeNum]++;
                                break;
                        }
                        break;
                    case PieceType.WhiteKing:
                    case PieceType.BlackKing:
                        switch (color)
                        {
                            case Color.Black:
                                PieceTypeNum = (int)Lookups.Pieces.bK;
                                BlackKingPieceTypeList[Lookups.PieceCounts[PieceTypeNum]] = square;
                                Lookups.PieceCounts[PieceTypeNum]++;
                                break;
                            case Color.White:
                                PieceTypeNum = (int)Lookups.Pieces.wK;
                                WhiteKingPieceTypeList[Lookups.PieceCounts[PieceTypeNum]] = square;
                                Lookups.PieceCounts[PieceTypeNum]++;
                                break;
                        }
                        break;
                }
            }
        }

        public void GenerateMoves(int[] pieceTypeList, int pieceTypeCount, PieceType pieceType)
        {
            for (int ndx = 0; ndx < pieceTypeCount; ndx++)
            {
                int square = pieceTypeList[ndx];
                switch (pieceType)
                {
                    case PieceType.WhitePawn:
                        GeneratePawnMoves(square, Color.White);
                        break;
                    case PieceType.BlackPawn:
                        GeneratePawnMoves(square, Color.Black);
                        break;
                    case PieceType.WhiteRook:
                        GenerateSlidingPieceMoves(pieceType, square, Lookups.MoveDirections[PieceType.WhiteRook]);
                        break;
                    case PieceType.BlackRook:
                        GenerateSlidingPieceMoves(pieceType, square, Lookups.MoveDirections[PieceType.BlackRook]);
                        break;
                    case PieceType.WhiteKnight:
                        GenerateNonSlidingPieceMoves(pieceType, square, Lookups.MoveDirections[PieceType.WhiteKnight]);
                        break;
                    case PieceType.BlackKnight:
                        GenerateNonSlidingPieceMoves(pieceType, square, Lookups.MoveDirections[PieceType.BlackKnight]);
                        break;
                    case PieceType.WhiteBishop:
                        GenerateSlidingPieceMoves(pieceType, square, Lookups.MoveDirections[PieceType.WhiteBishop]);
                        break;
                    case PieceType.BlackBishop:
                        GenerateSlidingPieceMoves(pieceType, square, Lookups.MoveDirections[PieceType.BlackBishop]);
                        break;
                    case PieceType.WhiteQueen:
                        GenerateSlidingPieceMoves(pieceType, square, Lookups.MoveDirections[PieceType.WhiteQueen]);
                        break;
                    case PieceType.BlackQueen:
                        GenerateSlidingPieceMoves(pieceType, square, Lookups.MoveDirections[PieceType.BlackQueen]);
                        break;
                    case PieceType.WhiteKing:
                        GenerateNonSlidingPieceMoves(pieceType, square, Lookups.MoveDirections[PieceType.WhiteKing]);
                        break;
                    case PieceType.BlackKing:
                        GenerateNonSlidingPieceMoves(pieceType, square, Lookups.MoveDirections[PieceType.BlackKing]);
                        break;
                }
            }
        }

        private void GeneratePawnMoves(int fromSquare, Color color)
        {
            var rank7 = fromSquare >= 81 && fromSquare <= 88;
            var rank2 = fromSquare >= 31 && fromSquare <= 38;
            if (SideToMove == Color.White)
            {
                if (Squares[fromSquare + 10] == PieceType.Empty)
                {
                    AddPawnMove(fromSquare, fromSquare + 10);
                    if (rank2 && Squares[fromSquare + 20] == PieceType.Empty)
                    {
                        AddPawnStartMove(PieceType.WhitePawn, fromSquare, fromSquare + 20);
                    }
                }
                var possibleCaptureSquares = new[] { fromSquare + 9, fromSquare + 11 };
                foreach (var possibleCaptureSquare in possibleCaptureSquares)
                {
                    if (Squares[possibleCaptureSquare] != PieceType.Empty)
                    {
                        color = Squares[possibleCaptureSquare] < PieceType.BlackPawn ? Color.White : Color.Black;
                        if (Squares[possibleCaptureSquare] != PieceType.OffBoard && color == Color.Black)
                        {
                            AddCaptureMove(PieceType.WhitePawn, fromSquare, Squares[possibleCaptureSquare],
                                possibleCaptureSquare);
                        }
                    }
                }

                var possibleEnPassantSquares = new[] { fromSquare + 9, fromSquare + 11 };
                if (EnPassantSquare != 0)
                {
                    foreach (var possibleEnPassantSquare in possibleEnPassantSquares)
                    {
                        if (possibleEnPassantSquare == EnPassantSquare)
                        {
                            AddEnPassantMove(PieceType.WhitePawn, fromSquare, possibleEnPassantSquare);
                        }
                    }
                }
            }
            if (SideToMove == Color.Black)
            {
                if (Squares[fromSquare - 10] == PieceType.Empty)
                {
                    AddPawnMove(fromSquare, fromSquare - 10);
                    if (rank7 && Squares[fromSquare - 20] == PieceType.Empty)
                    {
                        AddPawnStartMove(PieceType.BlackPawn, fromSquare, fromSquare - 20);
                    }
                }
                var possibleCaptureSquares = new[] { fromSquare - 9, fromSquare - 11 };
                foreach (var possibleCaptureSquare in possibleCaptureSquares)
                {
                    if (Squares[possibleCaptureSquare] != PieceType.Empty)
                    {
                        color = Squares[possibleCaptureSquare] < PieceType.BlackPawn ? Color.White : Color.Black;
                        if (Squares[possibleCaptureSquare] != PieceType.OffBoard && color == Color.White)
                        {
                            AddCaptureMove(PieceType.BlackPawn, fromSquare, Squares[possibleCaptureSquare],
                                possibleCaptureSquare);
                        }
                    }
                }

                var possibleEnPassantSquares = new[] { fromSquare - 9, fromSquare - 11 };
                if (EnPassantSquare != 0)
                {
                    foreach (var possibleEnPassantSquare in possibleEnPassantSquares)
                    {
                        if (possibleEnPassantSquare == EnPassantSquare)
                        {
                            AddEnPassantMove(PieceType.BlackPawn, fromSquare, possibleEnPassantSquare);
                        }
                    }
                }
            }
        }

        private void AddPawnStartMove(PieceType pawn, int fromSquare, int toSquare)
        {
            Moves[Ply].Add(new Move(pawn, fromSquare, toSquare) { IsPawnStartMove = true });
        }

        private void AddEnPassantMove(PieceType pawn, int fromSquare, int toSquare)
        {
            Moves[Ply].Add(new Move(pawn, fromSquare, toSquare) { IsEnPassantCapture = true });
        }

        private void AddCaptureMove(PieceType pieceType, int fromSquare, PieceType pieceTypeToCapture, int toSquare)
        {
            if (pieceType == PieceType.WhitePawn || pieceType == PieceType.BlackPawn)
            {
                if (SideToMove == Color.White && toSquare >= Lookups.A8 && toSquare <= Lookups.H8)
                {
                    // promoted
                    Moves[Ply].Add(new Move(pieceType, fromSquare, pieceTypeToCapture, toSquare, PieceType.WhiteQueen, SideToMove));
                    Moves[Ply].Add(new Move(pieceType, fromSquare, pieceTypeToCapture, toSquare, PieceType.WhiteRook, SideToMove));
                    Moves[Ply].Add(new Move(pieceType, fromSquare, pieceTypeToCapture, toSquare, PieceType.WhiteBishop, SideToMove));
                    Moves[Ply].Add(new Move(pieceType, fromSquare, pieceTypeToCapture, toSquare, PieceType.WhiteKnight, SideToMove));
                }
                else if (SideToMove == Color.Black && toSquare >= Lookups.A1 && toSquare <= Lookups.H1)
                {
                    Moves[Ply].Add(new Move(pieceType, fromSquare, pieceTypeToCapture, toSquare, PieceType.BlackQueen, SideToMove));
                    Moves[Ply].Add(new Move(pieceType, fromSquare, pieceTypeToCapture, toSquare, PieceType.BlackRook, SideToMove));
                    Moves[Ply].Add(new Move(pieceType, fromSquare, pieceTypeToCapture, toSquare, PieceType.BlackBishop, SideToMove));
                    Moves[Ply].Add(new Move(pieceType, fromSquare, pieceTypeToCapture, toSquare, PieceType.BlackKnight, SideToMove));
                }
                else
                {
                    Moves[Ply].Add(new Move(pieceType, fromSquare, pieceTypeToCapture, toSquare));
                }
            }
            else
            {
                Moves[Ply].Add(new Move(pieceType, fromSquare, pieceTypeToCapture, toSquare));
            }
        }

        private void AddQuietMove(PieceType pieceType, int @from, int to)
        {
            Moves[Ply].Add(new Move(pieceType, from, to));
        }

        private void AddPawnMove(int from, int to)
        {
            if (SideToMove == Color.White && to >= Lookups.A8 && to <= Lookups.H8)
            {
                // promoted
                Moves[Ply].Add(new Move(PieceType.WhitePawn, from, to, PieceType.WhiteQueen, SideToMove));
                Moves[Ply].Add(new Move(PieceType.WhitePawn, from, to, PieceType.WhiteRook, SideToMove));
                Moves[Ply].Add(new Move(PieceType.WhitePawn, from, to, PieceType.WhiteBishop, SideToMove));
                Moves[Ply].Add(new Move(PieceType.WhitePawn, from, to, PieceType.WhiteKnight, SideToMove));
            }
            else if (SideToMove == Color.Black && to >= Lookups.A1 && to <= Lookups.H1)
            {
                Moves[Ply].Add(new Move(PieceType.BlackPawn, from, to, PieceType.BlackQueen, SideToMove));
                Moves[Ply].Add(new Move(PieceType.BlackPawn, from, to, PieceType.BlackRook, SideToMove));
                Moves[Ply].Add(new Move(PieceType.BlackPawn, from, to, PieceType.BlackBishop, SideToMove));
                Moves[Ply].Add(new Move(PieceType.BlackPawn, from, to, PieceType.BlackKnight, SideToMove));
            }
            else
            {
                Moves[Ply].Add(SideToMove == Color.White
                    ? new Move(PieceType.WhitePawn, @from, to)
                    : new Move(PieceType.BlackPawn, @from, to));
            }
        }

        private void GenerateSlidingPieceMoves(PieceType pieceType, int square, IEnumerable<int> directions)
        {
            if (pieceType != PieceType.WhiteRook && pieceType != PieceType.WhiteQueen && pieceType != PieceType.WhiteBishop
                && pieceType != PieceType.BlackRook && pieceType != PieceType.BlackQueen && pieceType != PieceType.BlackBishop)
            {
                throw new ArgumentException("PieceType");
            }
            foreach (var direction in directions)
            {
                var testSquare = square + direction;
                var pieceTypeToTest = Squares[testSquare];

                Color color;
                if (pieceTypeToTest == PieceType.Empty)
                {
                    color = Color.Neither;
                }
                else
                {
                    color = pieceTypeToTest < (PieceType)7 ? Color.White : Color.Black;
                }
                while (pieceTypeToTest != PieceType.OffBoard)
                {
                    if (color == SideToMove) break;
                    if (pieceTypeToTest == PieceType.Empty)
                    {
                        AddQuietMove(pieceType, square, testSquare);
                    }
                    else if (color != SideToMove)
                    {
                        AddCaptureMove(pieceType, square, pieceTypeToTest, testSquare);
                        break;
                    }
                    testSquare += direction;
                    pieceTypeToTest = Squares[testSquare];
                    if (pieceTypeToTest == PieceType.Empty)
                    {
                        color = Color.Neither;
                    }
                    else
                    {
                        color = pieceTypeToTest < (PieceType)7 ? Color.White : Color.Black;
                    }

                }
            }
        }

        private void GenerateNonSlidingPieceMoves(PieceType pieceType, int square, IEnumerable<int> directions)
        {
            foreach (var direction in directions)
            {
                var pieceTypeToTest = Squares[square + direction];
                var color = pieceTypeToTest < (PieceType)7 ? Color.White : Color.Black;
                if (pieceTypeToTest != PieceType.OffBoard)
                {
                    if (pieceTypeToTest == PieceType.Empty)
                    {
                        AddQuietMove(pieceType, square, square + direction);
                    }
                    else if (color != SideToMove)
                    {
                        AddCaptureMove(pieceType, square, pieceTypeToTest, square + direction);
                    }
                }
            }
            //            Color attackColor = SideToMove == Color.White ? Color.Black : Color.White;
            if ((pieceType == PieceType.WhiteKing || pieceType == PieceType.BlackKing) && !IsSquareAttacked(square, SideToMove))
            {
                GenerateCastlingMoves(pieceType, square);
            }
        }

        private void GenerateCastlingMoves(PieceType pieceType, int fromSquare)
        {
            var color = pieceType == PieceType.WhiteKing ? Color.White : Color.Black;
            if (color == Color.White)
            {
                if (CanCastle(CastlePermissions.WhiteQueen, new[] { Lookups.B1, Lookups.C1, Lookups.D1 }, Color.White))
                {
                    AddCastleMove(CastlePermissions.WhiteQueen, fromSquare);
                }

                if (CanCastle(CastlePermissions.WhiteKing, new[] { Lookups.G1, Lookups.F1 }, Color.White))
                {
                    AddCastleMove(CastlePermissions.WhiteKing, fromSquare);
                }
            }
            else
            {
                if (CanCastle(CastlePermissions.BlackQueen, new[] { Lookups.B8, Lookups.C8, Lookups.D8 }, Color.Black))
                {
                    AddCastleMove(CastlePermissions.BlackQueen, fromSquare);
                }

                if (CanCastle(CastlePermissions.BlackKing, new[] { Lookups.G8, Lookups.F8 }, Color.Black))
                {
                    AddCastleMove(CastlePermissions.BlackKing, fromSquare);
                }
            }
        }

        private bool CanCastle(CastlePermissions permission, IList<int> squaresToCheck, Color colorToCheck)
        {
            var canCastle = false;
            if ((CastlePermission & permission) > 0)
            {
                canCastle = true;
                for (int square = 0; square < squaresToCheck.Count; square++)
                {
                    int ndx = squaresToCheck[square];
                    if (Squares[ndx] != PieceType.Empty || (square > 0 && IsSquareAttacked(ndx, colorToCheck)))
                    {
                        canCastle = false;
                        break;
                    }
                }
            }
            return canCastle;
        }

        private void AddCastleMove(CastlePermissions castlePermissions, int fromSquare)
        {
            var king = PieceType.Empty;
            int toSquare = 0;
            if (castlePermissions == CastlePermissions.WhiteQueen)
            {
                king = PieceType.WhiteKing;
                toSquare = Lookups.C1;
            }
            if (castlePermissions == CastlePermissions.WhiteKing)
            {
                king = PieceType.WhiteKing;
                toSquare = Lookups.G1;
            }
            if (castlePermissions == CastlePermissions.BlackQueen)
            {
                king = PieceType.BlackKing;
                toSquare = Lookups.C8;
            }
            if (castlePermissions == CastlePermissions.BlackKing)
            {
                king = PieceType.BlackKing;
                toSquare = Lookups.G8;
            }
            Moves[Ply].Add(new Move(king, fromSquare, toSquare, true));
        }

        private bool IsSquareAttackedByARook(int square, Color colorOfMovingSide)
        {
            for (int ndx = 0; ndx < Lookups.MoveDirections[PieceType.WhiteRook].Length; ndx++)
            {
                var direction = Lookups.MoveDirections[PieceType.WhiteRook][ndx];
                var testSquare = direction;
                var pieceType = Squares[square + testSquare];
                var pieceColor = pieceType < PieceType.BlackPawn ? Color.White : Color.Black;
                while (IsValidAttackingPieceType(pieceType))
                {
                    if (pieceType == PieceType.Empty)
                    {
                        testSquare += direction;
                        pieceType = Squares[square + testSquare];
                        pieceColor = pieceType < PieceType.BlackPawn ? Color.White : Color.Black;
                    }
                    else
                    {
                        if (pieceColor == colorOfMovingSide) break;
                        if (colorOfMovingSide == Color.Black)
                        {
                            if (pieceColor == Color.White)
                            {
                                if (pieceType == PieceType.WhiteRook || pieceType == PieceType.WhiteQueen)
                                {
                                    return true;
                                }
                                break;
                            }
                        }
                        else if (colorOfMovingSide == Color.White)
                        {
                            if (pieceType == PieceType.BlackRook || pieceType == PieceType.BlackQueen)
                            {
                                return true;
                            }
                            break;

                        }
                    }
                }
            }
            return false;
        }

        private bool IsSquareAttackedByABishop(int square, Color colorOfMovingSide)
        {
            for (int ndx = 0; ndx < Lookups.MoveDirections[PieceType.WhiteBishop].Length; ndx++)
            {
                var direction = Lookups.MoveDirections[PieceType.WhiteBishop][ndx];
                var testSquare = direction;
                var pieceType = Squares[square + testSquare];
                var pieceColor = pieceType < PieceType.BlackPawn ? Color.White : Color.Black;
                while (IsValidAttackingPieceType(pieceType))
                {
                    if (pieceType == PieceType.Empty)
                    {
                        testSquare += direction;
                        pieceType = Squares[square + testSquare];
                        pieceColor = pieceType < PieceType.BlackPawn ? Color.White : Color.Black;
                    }
                    else
                    {
                        if (pieceColor == colorOfMovingSide) break;
                        if (colorOfMovingSide == Color.Black)
                        {
                            if (pieceColor == Color.White)
                            {
                                if (pieceType == PieceType.WhiteBishop || pieceType == PieceType.WhiteQueen)
                                {
                                    return true;
                                }
                                break;
                            }
                        }
                        else if (colorOfMovingSide == Color.White)
                        {
                            if (pieceType == PieceType.BlackBishop || pieceType == PieceType.BlackQueen)
                            {
                                return true;
                            }
                            break;
                        }
                    }
                }
            }
            return false;
        }

        private bool IsValidAttackingPieceType(PieceType pieceType)
        {
            return pieceType != PieceType.OffBoard;
        }

        private bool IsAttackedByAKing(int square, Color colorOfMovingSide)
        {
            var attackingKing = colorOfMovingSide == Color.White ? PieceType.BlackKing : PieceType.WhiteKing;
            foreach (var direction in Lookups.MoveDirections[attackingKing])
            {
                var pieceType = Squares[square + direction];
                var pieceColor = pieceType < PieceType.BlackPawn ? Color.White : Color.Black;
                if (pieceColor != colorOfMovingSide && pieceType == attackingKing) return true;
            }
            return false;
        }

        private bool IsAttackedByAKnight(int square, Color colorOfMovingSide)
        {
            var attackingKinght = colorOfMovingSide == Color.White ? PieceType.BlackKnight : PieceType.WhiteKnight;
            for (int i = 0; i < Lookups.MoveDirections[PieceType.WhiteKnight].Length; i++)
            {
                var pieceType = Squares[square + Lookups.MoveDirections[PieceType.WhiteKnight][i]];
                if (pieceType == attackingKinght) return true;

            }
            return false;
        }

        private bool IsAttackedByPawn(int square, Color colorOfMovingSide)
        {
            if (colorOfMovingSide == Color.White)
            {
                var pieceType = Squares[square + 11];
                if (pieceType == PieceType.BlackPawn) return true;
                pieceType = Squares[square + 9];
                if (pieceType == PieceType.BlackPawn) return true;
            }
            else
            {
                var pieceType = Squares[square - 11];
                if (pieceType == PieceType.WhitePawn) return true;
                pieceType = Squares[square - 9];
                if (pieceType == PieceType.WhitePawn) return true;
            }
            return false;
        }

        public void GenerateMoves()
        {
            int PieceTypeNdx;
            int PieceTypeCount;
            Moves[Ply].Clear();

            if (SideToMove == Color.White)
            {

                PieceTypeNdx = (int)Lookups.Pieces.wK;
                PieceTypeCount = Lookups.PieceCounts[PieceTypeNdx];
                GenerateMoves(WhiteKingPieceTypeList, PieceTypeCount, PieceType.WhiteKing);

                PieceTypeNdx = (int)Lookups.Pieces.wP;
                PieceTypeCount = Lookups.PieceCounts[PieceTypeNdx];
                GenerateMoves(WhitePawnPieceTypeList, PieceTypeCount, PieceType.WhitePawn);


                PieceTypeNdx = (int)Lookups.Pieces.wR;
                PieceTypeCount = Lookups.PieceCounts[PieceTypeNdx];
                GenerateMoves(WhiteRookPieceTypeList, PieceTypeCount, PieceType.WhiteRook);

                PieceTypeNdx = (int)Lookups.Pieces.wN;
                PieceTypeCount = Lookups.PieceCounts[PieceTypeNdx];
                GenerateMoves(WhiteKnightPieceTypeList, PieceTypeCount, PieceType.WhiteKnight);

                PieceTypeNdx = (int)Lookups.Pieces.wB;
                PieceTypeCount = Lookups.PieceCounts[PieceTypeNdx];
                GenerateMoves(WhiteBishopPieceTypeList, PieceTypeCount, PieceType.WhiteBishop);

                PieceTypeNdx = (int)Lookups.Pieces.wQ;
                PieceTypeCount = Lookups.PieceCounts[PieceTypeNdx];
                GenerateMoves(WhiteQueenPieceTypeList, PieceTypeCount, PieceType.WhiteQueen);

            }
            else
            {
                PieceTypeNdx = (int)Lookups.Pieces.bK;
                PieceTypeCount = Lookups.PieceCounts[PieceTypeNdx];
                GenerateMoves(BlackKingPieceTypeList, PieceTypeCount, PieceType.BlackKing);

                PieceTypeNdx = (int)Lookups.Pieces.bP;
                PieceTypeCount = Lookups.PieceCounts[PieceTypeNdx];
                GenerateMoves(BlackPawnPieceTypeList, PieceTypeCount, PieceType.BlackPawn);

                PieceTypeNdx = (int)Lookups.Pieces.bR;
                PieceTypeCount = Lookups.PieceCounts[PieceTypeNdx];
                GenerateMoves(BlackRookPieceTypeList, PieceTypeCount, PieceType.BlackRook);

                PieceTypeNdx = (int)Lookups.Pieces.bN;
                PieceTypeCount = Lookups.PieceCounts[PieceTypeNdx];
                GenerateMoves(BlackKnightPieceTypeList, PieceTypeCount, PieceType.BlackKnight);

                PieceTypeNdx = (int)Lookups.Pieces.bB;
                PieceTypeCount = Lookups.PieceCounts[PieceTypeNdx];
                GenerateMoves(BlackBishopPieceTypeList, PieceTypeCount, PieceType.BlackBishop);

                PieceTypeNdx = (int)Lookups.Pieces.bQ;
                PieceTypeCount = Lookups.PieceCounts[PieceTypeNdx];
                GenerateMoves(BlackQueenPieceTypeList, PieceTypeCount, PieceType.BlackQueen);
            }
        }
    }

}