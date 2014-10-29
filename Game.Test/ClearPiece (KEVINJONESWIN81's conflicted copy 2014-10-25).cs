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

                board.RookPieceList.Count.Should().Be(3);
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

                board.RookPieceList.Count.Should().Be(4);
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
                board.MovePiece(from, to);

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
                board.MovePiece(from, to);

                board.Squares[21].Type.Should().Be(new EmptyPiece().Type);
            }

            [Test]
            public void ShouldMoveToToSquare()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRooksPawn);

                var from = new Rook { Square = 21, Color = Color.White };
                var to = new Rook { Square = 31, Color = Color.White };
                board.MovePiece(from, to);

                board.Squares[31].Type.Should().Be(to.Type);
            }

            [Test]
            public void ShouldKeepThePieceListCountTheSameWhenMovingOnePiece()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRooksPawn);

                var from = new Rook { Square = 21, Color = Color.White };
                var to = new Rook { Square = 31, Color = Color.White };
                board.MovePiece(from, to);

                board.RookPieceList.Count.Should().Be(4);
            }

            [Test]
            public void ShouldChangeThePieceListCountsWhenCheangingTheMovedPiece()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRooksPawn);

                var from = new Rook { Square = 21, Color = Color.White };
                var to = new Pawn { Square = 31, Color = Color.White };
                board.MovePiece(from, to);

                board.RookPieceList.Count.Should().Be(3);
                board.PawnPieceList.Count.Should().Be(16);
            }
        }

        [TestFixture]
        public class MakeMove
        {
            private const string InitialBoardSetupWhiteToMoveNoWhiteRooksPawn = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

            [Test]
            public void ShouldAddANewEntryToHistory()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRooksPawn);
                var m = new Move(new Pawn { Square = 21 }, 22);
                board.MakeMove(m);

                board.History.Count.Should().Be(1);
            }

            [Test]
            public void ShouldUpdateThePositionInHistoryEntry()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRooksPawn);
                var m = new Move(new Pawn { Square = 21 }, 22);
                board.MakeMove(m);

                board.History[0].PositionKey.Should().Be(board.PositionKey);
            }

            [Test]
            public void ShouldUpdateTheCastlePermissionsInHistoryEntry()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRooksPawn);
                var m = new Move(new Pawn { Square = 21 }, 22);
                board.MakeMove(m);

                board.History[0].CastlePermissions.Should().Be(board.CastlePermission);
            }

            [Test]
            public void ShouldUpdateTheEnPassantSquareHistoryEntry()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRooksPawn);
                var m = new Move(new Pawn { Square = 21 }, 22);
                board.MakeMove(m);

                board.History[0].EnPassantSquare.Should().Be(board.EnPassantSquare);
            }

            [Test]
            public void ShouldUpdateTheFifyMoveCountInHistoryEntry()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRooksPawn);
                var m = new Move(new Pawn { Square = 21 }, 22);
                board.MakeMove(m);

                board.History[0].FiftyMoveCount.Should().Be(board.FiftyMoveCount);
            }

            [Test]
            public void ShouldClearTheBlackPawnWhenItIsAWhiteEnPassantCapture()
            {
                const string initialBoardSetupForEnPassantCaptureWhiteToMove = "rnbqkbnr/pp1ppppp/8/2pP4/8/8/PPP1PPPP/RNBQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForEnPassantCaptureWhiteToMove);
                var m = new Move(new Pawn { Square = 64,Color = Color.White}, 73) { IsEnPassantCapture = true };
                board.Squares[63].Type.Should().Be(new Pawn().Type);
                board.MakeMove(m);

                board.Squares[63].Type.Should().Be(new EmptyPiece().Type);
            }

            [Test]
            public void ShouldClearTheWhitePawnWhenItIsABlackEnPassantCapture()
            {
                const string initialBoardSetupForEnPassantCaptureWhiteToMove = "rnbqkbnr/pp1ppppp/8/8/2pP4/8/PPP1PPPP/RNBQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForEnPassantCaptureWhiteToMove);
                var m = new Move(new Pawn { Square = 53, Color = Color.Black }, 44) { IsEnPassantCapture = true };
                board.Squares[54].Type.Should().Be(new Pawn().Type);
                board.MakeMove(m);

                board.Squares[54].Type.Should().Be(new EmptyPiece().Type);
            }

            [Test]
            public void ShouldMoveTheWhiteQueenRookWhenItIsAWhiteQueenSideCastle()
            {
                const string initialBoardSetupForWhiteQueenSideCastle = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteQueenSideCastle);
                var m = new Move(new King(), 23, CastlePermissions.WhiteQueen);
                board.MakeMove(m);
                board.Squares[24].Type.Should().Be(new Rook().Type);
            }

            [Test]
            public void ShouldMoveTheWhiteKingRookWhenItIsAWhiteKingSideCastle()
            {
                const string initialBoardSetupForWhiteKingSideCastle = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteKingSideCastle);
                var m = new Move(new King(), 27, CastlePermissions.WhiteKing);
                board.MakeMove(m);
                board.Squares[26].Type.Should().Be(new Rook().Type);
            }

            [Test]
            public void ShouldMoveTheBlackQueenRookWhenItIsABlackQueenSideCastle()
            {
                const string initialBoardSetupForWhiteQueenSideCastle = "r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteQueenSideCastle);
                var m = new Move(new King(), 93, CastlePermissions.BlackQueen);
                board.MakeMove(m);
                board.Squares[94].Type.Should().Be(new Rook().Type);
            }

            [Test]
            public void ShouldMoveTheBlackKingRookWhenItIsABlackKingSideCastle()
            {
                const string initialBoardSetupForWhiteQueenSideCastle = "rnbqk2r/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetupForWhiteQueenSideCastle);
                var m = new Move(new King(), 97, CastlePermissions.BlackKing);
                board.MakeMove(m);
                board.Squares[96].Type.Should().Be(new Rook().Type);
            }

            [Test]
            public void ShouldHashOutTheEnPassantKeyIfItIsSet()
            {
            }

            [Test]
            public void ShouldUpdateTheCastlePermissionsIfTheWhiteQueenRookHasMoved()
            {
            }

            [Test]
            public void ShouldUpdateTheCastlePermissionsIfTheWhiteBlackRookHasMoved()
            {
            }

            [Test]
            public void ShouldUpdateTheCastlePermissionsIfTheBlackQueenRookHasMoved()
            {
            }

            [Test]
            public void ShouldUpdateTheCastlePermissionsIfTheBlackKingRookHasMoved()
            {
            }

            [Test]
            public void ShouldHashInTheCastlePermissions()
            {
            }

            [Test]
            public void ShouldClearThePieceWhenItIsCaptured()
            {
            }

            [Test]
            public void ShouldResetTheFiftyMoveCountWhenAPieceWhenIsCaptured()
            {
            }

            [Test]
            public void ShouldUpdateTheHistoryPly()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRooksPawn);
                var m = new Move(new Pawn { Square = 21 }, 22);
                board.MakeMove(m);

                board.HistoryPly.Should().Be(1);
            }

            [Test]
            public void ShouldUpdateThePly()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRooksPawn);
                var m = new Move(new Pawn { Square = 21 }, 22);
                board.MakeMove(m);

                board.Ply.Should().Be(1);
            }

            [Test]
            public void ShouldResetTheFiftyMoveCountIfItIsAPawnMove()
            {
            }

            [Test]
            public void ShouldSetTheEnPassantSquareIfItIsAWhitePawnStartMove()
            {
            }

            [Test]
            public void ShouldSetTheEnPassantSquareIfItIsABlackPawnStartMove()
            {
            }

            [Test]
            public void ShouldSwitchSide()
            {
            }

            [Test]
            public void ShouldReturnTrueIfTheKingIsNotInCheck()
            {

            }

            [Test]
            public void ShouldReturnFalseIfTheKingIsInCheck()
            {

            }

            [Test]
            public void ShouldTakeBackTheMoveIfTheKingIsInCheck()
            {
            }
        }
    }
}