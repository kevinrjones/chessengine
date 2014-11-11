using FluentAssertions;
using NUnit.Framework;

namespace Game.Test
{
    namespace Board
    {
        [TestFixture]
        public class ClearPiece
        {
            private const string InitialBoardSetupWhiteToMove = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

            [Test]
            public void ShouldHashOutPiece()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);

                int initialPositionKey = board.PositionKey;

                board.ClearPiece(board.Squares[21]);


                board.HashPiece(new Rook { Square = 21 });
                var positionKey = board.PositionKey;
                initialPositionKey.Should().Be(positionKey);
            }

            [Test]
            public void ShouldEmptyTheSquare()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);

                board.ClearPiece(board.Squares[21]);

                board.Squares[21].Type.Should().Be(PieceType.Empty);

            }

            [Test]
            public void ShouldUpdateTheMaterial()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);
                int material = board.Material[(int)Color.White];

                board.ClearPiece(board.Squares[21]);

                board.Material[(int)Color.White].Should().Be(material - new Rook().Value);
            }

            [Test]
            public void ShouldRemovePieceFromList()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);

                board.ClearPiece(board.Squares[21]);

                //board.WhiteRookPieceList.Count.Should().Be(3);
            }
        }

        [TestFixture]
        public class AddPiece
        {
            private const string InitialBoardSetupWhiteToMoveNoWhiteRook = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/1NBQKBNR w KQkq - 0 1";

            [Test]
            public void ShouldHashPieceIn()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRook);

                int initialPositionKey = board.PositionKey;

                board.AddPiece(new Rook { Square = 21, Color = Color.White });
                board.HashPiece(board.Squares[21]);

                var positionKey = board.PositionKey;
                positionKey.Should().Be(initialPositionKey);
            }

            [Test]
            public void ShouldUpdateMaterial()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRook);
                int material = board.Material[(int)Color.White];

                var rook = new Rook { Square = 21, Color = Color.White };
                board.AddPiece(rook);

                board.Material[(int)Color.White].Should().Be(material + rook.Value);
            }

            [Test]
            public void ShouldUpdateSquare()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRook);

                var rook = new Rook { Square = 21, Color = Color.White };
                board.AddPiece(rook);

                board.Squares[21].Type.Should().Be(rook.Type);
            }

            [Test]
            public void ShouldUpdatePieceList()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRook);

                var rook = new Rook { Square = 21, Color = Color.White };
                board.AddPiece(rook);

                //board.WhiteRookPieceList.Count.Should().Be(4);
            }
        }

        [TestFixture]
        public class MovePiece
        {
            private const string InitialBoardSetupWhiteToMoveNoWhiteRooksPawn = "rnbqkbnr/pppppppp/8/8/8/8/1PPPPPPP/RNBQKBNR w KQkq - 0 1";

            [Test]
            public void ShouldHashPieceIn()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRooksPawn);

                int initialPositionKey = board.PositionKey;

                var from = new Rook { Square = 21, Color = Color.White };
                var to = new Rook { Square = 31, Color = Color.White };
                board.CastlePiece(from, to);

                board.HashPiece(from);
                board.HashPiece(to);

                var positionKey = board.PositionKey;
                positionKey.Should().Be(initialPositionKey);
            }

            [Test]
            public void ShouldClearFromSquare()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRooksPawn);

                var from = new Rook { Square = 21, Color = Color.White };
                var to = new Rook { Square = 31, Color = Color.White };
                board.CastlePiece(from, to);

                board.Squares[21].Type.Should().Be(PieceType.Empty);
            }

            [Test]
            public void ShouldMoveToToSquare()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRooksPawn);

                var from = new Rook { Square = 21, Color = Color.White };
                var to = new Rook { Square = 31, Color = Color.White };
                board.CastlePiece(from, to);

                board.Squares[31].Type.Should().Be(to.Type);
            }

            [Test]
            public void ShouldKeepThePieceListCountTheSameWhenMovingOnePiece()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRooksPawn);

                var from = new Rook { Square = 21, Color = Color.White };
                var to = new Rook { Square = 31, Color = Color.White };
                board.CastlePiece(from, to);

                //board.WhiteRookPieceList.Count.Should().Be(4);
            }

            [Test]
            public void ShouldChangeThePieceListCountsWhenCastling()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRooksPawn);

                var from = new Rook { Square = 21, Color = Color.White };
                var to = new Rook { Square = 31, Color = Color.White };
                board.CastlePiece(from, to);

                //board.WhiteRookPieceList.Count.Should().Be(4);
            }
        }

        [TestFixture]
        public class MakeMove
        {
            private const string InitialBoardSetupWhiteToMove = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

            [Test]
            public void ShouldAddANewEntryToHistory()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);
                var m = new Move(new Pawn { Square = 22 }, 23);
                board.MakeMove(m);

                board.History.Count.Should().Be(1);
            }

            [Test]
            public void ShouldUpdateThePositionInHistoryEntry()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);

                // move to the same square
                var m = new Move(new Pawn { Square = 22 }, 22);
                board.MakeMove(m);

                // undo the side change
                board.HashSide();


                board.History[0].PositionKey.Should().Be(board.PositionKey);
            }

            [Test]
            public void ShouldUpdateTheCastlePermissionsInHistoryEntry()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);
                var m = new Move(new Pawn { Square = 22 }, 23);
                board.MakeMove(m);

                board.History[0].CastlePermissions.Should().Be(board.CastlePermission);
            }

            [Test]
            public void ShouldUpdateTheEnPassantSquareHistoryEntry()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);
                var m = new Move(new Pawn { Square = 22 }, 23);
                board.MakeMove(m);

                board.History[0].EnPassantSquare.Should().Be(board.EnPassantSquare);
            }

            [Test]
            public void ShouldUpdateTheFifyMoveCountInHistoryEntry()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);
                var m = new Move(new Pawn { Square = 22 }, 23);
                board.MakeMove(m);

                board.History[0].FiftyMoveCount.Should().Be(board.FiftyMoveCount);
            }

            [Test]
            public void ShouldClearTheBlackPawnWhenItIsAWhiteEnPassantCapture()
            {
                const string initialBoardSetupForEnPassantCaptureWhiteToMove = "rnbqkbnr/pp1ppppp/8/2pP4/8/8/PPP1PPPP/RNBQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForEnPassantCaptureWhiteToMove);
                var m = new Move(new Pawn { Square = 64, Color = Color.White }, 73) { IsEnPassantCapture = true };
                board.Squares[63].Type.Should().Be(PieceType.Pawn);
                board.MakeMove(m);

                board.Squares[63].Type.Should().Be(PieceType.Empty);
            }

            [Test]
            public void ShouldClearTheWhitePawnWhenItIsABlackEnPassantCapture()
            {
                const string initialBoardSetupForEnPassantCaptureWhiteToMove = "rnbqkbnr/pp1ppppp/8/8/2pP4/8/PPP1PPPP/RNBQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForEnPassantCaptureWhiteToMove);
                var m = new Move(new Pawn { Square = 53, Color = Color.Black }, 44) { IsEnPassantCapture = true };
                board.Squares[54].Type.Should().Be(PieceType.Pawn);
                board.MakeMove(m);

                board.Squares[54].Type.Should().Be(PieceType.Empty);
            }

            [Test]
            public void ShouldMoveTheWhiteQueenRookWhenItIsAWhiteQueenSideCastle()
            {
                const string initialBoardSetupForWhiteQueenSideCastle = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteQueenSideCastle);
                var m = new Move(new King(), 23, true);
                board.MakeMove(m);
                board.Squares[24].Type.Should().Be(PieceType.Rook);
            }

            [Test]
            public void ShouldSetTheWhiteQueenRookSquareWhenItIsAWhiteQueenSideCastle()
            {
                const string initialBoardSetupForWhiteQueenSideCastle = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteQueenSideCastle);
                var m = new Move(new King(), 23, true);
                board.MakeMove(m);
                board.Squares[21].Type.Should().Be(PieceType.Empty);
            }

            [Test]
            public void ShouldMoveTheWhiteKingRookWhenItIsAWhiteKingSideCastle()
            {
                const string initialBoardSetupForWhiteKingSideCastle = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteKingSideCastle);
                var m = new Move(new King(), 27, true);
                board.MakeMove(m);
                board.Squares[26].Type.Should().Be(PieceType.Rook);
            }

            [Test]
            public void ShouldSetTheWhiteKingRookSquareWhenItIsAWhiteKingSideCastle()
            {
                const string initialBoardSetupForWhiteKingSideCastle = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteKingSideCastle);
                var m = new Move(new King(), 27, true);
                board.MakeMove(m);
                board.Squares[28].Type.Should().Be(PieceType.Empty);
            }

            [Test]
            public void ShouldMoveTheBlackQueenRookWhenItIsABlackQueenSideCastle()
            {
                const string initialBoardSetupForWhiteQueenSideCastle = "r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteQueenSideCastle);
                var m = new Move(new King(), 93, true);
                board.MakeMove(m);
                board.Squares[94].Type.Should().Be(PieceType.Rook);
            }

            [Test]
            public void ShouldSetTheBlackKingRookSquareWhenItIsABlackKingSideCastle()
            {
                const string initialBoardSetupForWhiteQueenSideCastle = "r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteQueenSideCastle);
                var m = new Move(new King(), 93, true);
                board.MakeMove(m);
                board.Squares[91].Type.Should().Be(PieceType.Empty);
            }

            [Test]
            public void ShouldMoveTheBlackKingRookWhenItIsABlackKingSideCastle()
            {
                const string initialBoardSetupForWhiteQueenSideCastle = "rnbqk2r/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteQueenSideCastle);
                var m = new Move(new King(), 97, true);
                board.MakeMove(m);
                board.Squares[96].Type.Should().Be(PieceType.Rook);
            }

            [Test]
            public void ShouldSetTheBlackQueenRookSquareWhenItIsABlackQueenSideCastle()
            {
                const string initialBoardSetupForWhiteQueenSideCastle = "rnbqk2r/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteQueenSideCastle);
                var m = new Move(new King(), 97, true);
                board.MakeMove(m);
                board.Squares[98].Type.Should().Be(PieceType.Empty);
            }

            [Test]
            public void ShouldHashOutTheEnPassantKeyIfItIsSet()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);
                var initialPosKey = board.PositionKey;
                board.EnPassantSquare = 41;
                var m = new Move(new Pawn(), 23);
                board.MakeMove(m);
                board.PositionKey.Should().NotBe(initialPosKey);

            }

            [Test]
            public void ShouldNotHashOutTheEnPassantKeyIfItIsSet()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);
                var initialPosKey = board.PositionKey;
                board.EnPassantSquare = 0;

                // move to the same square
                var m = new Move(new Pawn { Color = Color.White, Square = 22 }, 22);
                board.MakeMove(m);

                // undo the side change
                board.HashSide();

                board.PositionKey.Should().Be(initialPosKey);

            }

            [Test]
            public void ShouldUpdateTheCastlePermissionsIfTheWhiteQueenRookHasMoved()
            {
                const string initialBoardSetupForWhiteRookMove = "rnbqkbnr/pppppppp/8/8/8/8/1PPPPPPP/RBNQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteRookMove);
                var m = new Move(new Rook { Color = Color.White, Square = 21 }, 22);
                board.MakeMove(m);
                ((int)(board.CastlePermission & CastlePermissions.WhiteQueen)).Should().Be(0);
            }

            [Test]
            public void ShouldUpdateTheCastlePermissionsIfTheWhiteKingRookHasMoved()
            {
                const string initialBoardSetupForWhiteRookMove = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPP1/RBNQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteRookMove);
                var m = new Move(new Rook { Color = Color.White, Square = 28 }, 29);
                board.MakeMove(m);
                ((int)(board.CastlePermission & CastlePermissions.WhiteKing)).Should().Be(0);
            }

            [Test]
            public void ShouldUpdateTheCastlePermissionsIfTheBlackQueenRookHasMoved()
            {
                const string initialBoardSetupForWhiteRookMove = "rnbqkbnr/1ppppppp/8/8/8/8/PPPPPPPP/RBNQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteRookMove);
                var m = new Move(new Rook { Color = Color.Black, Square = 91 }, 81);
                board.MakeMove(m);
                ((int)(board.CastlePermission & CastlePermissions.BlackQueen)).Should().Be(0);
            }

            [Test]
            public void ShouldUpdateTheCastlePermissionsIfTheBlackKingRookHasMoved()
            {
                const string initialBoardSetupForWhiteRookMove = "rnbqkbnr/ppppppp1/8/8/8/8/PPPPPPPP/RBNQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteRookMove);
                var m = new Move(new Rook { Color = Color.Black, Square = 98 }, 88);
                board.MakeMove(m);
                ((int)(board.CastlePermission & CastlePermissions.BlackKing)).Should().Be(0);
            }

            [Test]
            public void ShouldChangePositionKeyIfTheWhiteQueenRookMoves()
            {
                const string initialBoardSetupForWhiteRookMove = "rnbqkbnr/pppppppp/8/8/8/8/1PPPPPPP/RBNQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteRookMove);
                var initialPosKey = board.PositionKey;
                var m = new Move(new Rook { Color = Color.White, Square = 21 }, 31);
                board.MakeMove(m);
                board.PositionKey.Should().NotBe(initialPosKey);

            }

            [Test]
            public void ShouldChangePositionKeyIfTheWhiteKingRookMoves()
            {
                const string initialBoardSetupForWhiteRookMove = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPP1/RBNQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteRookMove);
                var initialPosKey = board.PositionKey;
                var m = new Move(new Rook { Color = Color.White, Square = 28 }, 38);
                board.MakeMove(m);
                board.PositionKey.Should().NotBe(initialPosKey);

            }

            [Test]
            public void ShouldChangePositionKeyIfTheBlackQueenRookMoves()
            {
                const string initialBoardSetupForWhiteRookMove = "rnbqkbnr/1ppppppp/8/8/8/8/PPPPPPPP/RBNQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteRookMove);
                var initialPosKey = board.PositionKey;
                var m = new Move(new Rook { Color = Color.White, Square = 91 }, 81);
                board.MakeMove(m);
                board.PositionKey.Should().NotBe(initialPosKey);

            }

            [Test]
            public void ShouldChangePositionKeyIfTheBlackKingRookMoves()
            {
                const string initialBoardSetupForWhiteRookMove = "rnbqkbnr/ppppppp1/8/8/8/8/1PPPPPPP/RBNQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteRookMove);
                var initialPosKey = board.PositionKey;
                var m = new Move(new Rook { Color = Color.White, Square = 98 }, 88);
                board.MakeMove(m);
                board.PositionKey.Should().NotBe(initialPosKey);

            }

            [Test]
            public void ShouldUpdateThePieceWhenItIsCaptured()
            {
                const string initialBoardSetupForWhiteRookMove = "rnbqkbnr/ppppppp1/8/8/8/8/1PPPPPPP/RBNQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteRookMove);

                board.Squares[28].Type.Should().Be(PieceType.Rook);

                var m = new Move(new Rook { Color = Color.Black, Square = 98 },
                    new Rook { Color = Color.White, Square = 28 });

                board.MakeMove(m);
                board.Squares[28].Type.Should().Be(PieceType.Rook);
                board.Squares[28].Color.Should().Be(Color.Black);
            }

            [Test]
            public void ShouldResetTheFiftyMoveCountWhenAPieceWhenIsCaptured()
            {
                const string initialBoardSetupForWhiteRookMove = "rnbqkbnr/ppppppp1/8/8/8/8/1PPPPPPP/RBNQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteRookMove);

                board.Squares[28].Type.Should().Be(PieceType.Rook);

                var m = new Move(new Rook { Color = Color.Black, Square = 98 },
                    new Rook { Color = Color.White, Square = 28 });

                board.FiftyMoveCount = 1;
                board.MakeMove(m);
                board.FiftyMoveCount.Should().Be(0);
            }

            [Test]
            public void ShouldUpdateTheHistoryPly()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);
                var m = new Move(new Pawn { Square = 22 }, 23);
                board.MakeMove(m);

                board.HistoryPly.Should().Be(1);
            }

            [Test]
            public void ShouldUpdateThePly()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);
                var m = new Move(new Pawn { Square = 22 }, 23);
                board.MakeMove(m);

                board.Ply.Should().Be(1);
            }

            [Test]
            public void ShouldResetTheFiftyMoveCountIfItIsAPawnMove()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);
                var m = new Move(new Pawn { Square = 22 }, 23);
                board.FiftyMoveCount = 1;

                board.MakeMove(m);

                board.FiftyMoveCount.Should().Be(0);
            }

            [Test]
            public void ShouldSetTheEnPassantSquareIfItIsAWhitePawnStartMove()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);
                var m = new Move(new Pawn { Square = 31, Color = Color.White }, 51) { IsPawnStartMove = true };
                board.MakeMove(m);
                board.EnPassantSquare.Should().Be(41);
            }

            [Test]
            public void ShouldSetTheEnPassantSquareIfItIsABlackPawnStartMove()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);
                var m = new Move(new Pawn { Square = 81, Color = Color.Black }, 61) { IsPawnStartMove = true };
                board.MakeMove(m);
                board.EnPassantSquare.Should().Be(71);
            }

            [Test]
            public void ShouldUpdateThePositionKeyWhenItItAnEnPassantMove()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);
                var m = new Move(new Pawn { Square = 81, Color = Color.Black }, 61) { IsPawnStartMove = true };
                var originalPositionKey = board.PositionKey;
                board.MakeMove(m);
                originalPositionKey.Should().NotBe(board.PositionKey);
            }

            [Test]
            public void ShouldSwitchSide()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);
                var m = new Move(new Pawn { Square = 31, Color = Color.White }, 41);
                board.MakeMove(m);
                board.SideToMove.Should().Be(Color.Black);
            }

            [Test]
            public void ShouldNotPromoteIfThereIsNoPromotion()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);
                var m = new Move(new Pawn { Square = 31, Color = Color.White }, 41);
                board.MakeMove(m);
                board.Squares[41].Type.Should().Be(PieceType.Pawn);
            }

            [Test]
            public void ShouldPromoteIfThereIsAPromotion()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);
                var m = new Move(new Pawn { Square = 81, Color = Color.White }, 91, new Queen { Square = 91 });
                board.MakeMove(m);
                board.Squares[91].Type.Should().Be(PieceType.Queen);
            }

            [Test]
            public void ShouldHasInTheNewSide()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);
                var oldPosition = board.PositionKey;
                var m = new Move(new Pawn { Square = 31, Color = Color.White }, 31);
                board.MakeMove(m);

                board.PositionKey.Should().NotBe(oldPosition);
            }

            [Test]
            public void ShouldReturnTrueIfTheKingIsNotInCheck()
            {
                const string blackKingNotAttackedAfterRookMove = "3k4/3r4/3r4/3R4/8/8/8/4K3 b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(blackKingNotAttackedAfterRookMove);
                var m = new Move(new Rook { Square = 84, Color = Color.Black }, 86);
                board.MakeMove(m).Should().BeTrue();
            }

            [Test]
            public void ShouldReturnFalseIfTheKingIsInCheck()
            {
                const string blackKingAttackedAfterRookMove = "3k4/3r4/8/3R4/8/8/8/4K3 b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(blackKingAttackedAfterRookMove);
                var m = new Move(new Rook { Square = 84, Color = Color.Black }, 86);
                board.MakeMove(m).Should().BeFalse();
            }

            [Test]
            public void ShouldTakeBackTheMoveIfTheKingIsInCheck()
            {
            }
        }

        [TestFixture]
        public class TakeMove
        {
            [Test]
            public void ShouldDecrementThePlys()
            {
                var board = new Game.Board();
                board.HistoryPly++;
                board.Ply++;
                board.History.Add(new History { Move = new Move(new Pawn { Square = 32 }, 42) });

                board.TakeMove();
                board.HistoryPly.Should().Be(0);
                board.Ply.Should().Be(0);
            }

            [Test]
            public void ShouldResetThePositionKeyAfterTakingBackTheOpeningMove()
            {
                const string initialBoardSetupWhiteToMove = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupWhiteToMove);
                var m = new Move(new Pawn { Square = 34, Color = Color.White }, 54) { IsPawnStartMove = true };

                var initialPositon = board.PositionKey;

                board.MakeMove(m);


                board.TakeMove();


                board.PositionKey.Should().Be(initialPositon);

            }

            [Test]
            public void ShouldResetThePositionKeyAfterCastling()
            {
                const string initialBoardSetupWhiteToMove = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupWhiteToMove);
                var m = new Move(new King { Square = 25, Color = Color.White }, 23, true);

                var initialPositon = board.PositionKey;

                board.MakeMove(m);

                board.TakeMove();

                board.PositionKey.Should().Be(initialPositon);

            }

            [Test]
            public void ShouldResetThePositionKeyAfterCapture()
            {
                const string initialBoardSetupWhiteToMove = "rnbqkbnr/pppppppp/8/8/8/8/1PPPPPPP/RNBQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupWhiteToMove);
                var m = new Move(new Rook { Square = 21, Color = Color.White }, new Pawn { Square = 81, Color = Color.Black });

                var initialPositon = board.PositionKey;

                board.MakeMove(m);

                board.TakeMove();

                board.PositionKey.Should().Be(initialPositon);

            }

            [Test]
            public void ShouldResetThePositionKeyPromotion()
            {
                const string initialBoardSetupWhiteToMove = "1nbqkbnr/Pppppppp/8/8/8/8/1PPPPPPP/RNBQKBNR w KQk - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupWhiteToMove);
                var m = new Move(new Pawn { Square = 81, Color = Color.White }, 91, new Queen { Square = 91, Color = Color.White});

                var initialPositon = board.PositionKey;

                board.MakeMove(m);

                board.TakeMove();

                board.PositionKey.Should().Be(initialPositon);

            }

        }
    }
}