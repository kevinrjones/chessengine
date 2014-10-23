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

                
                board.HashPiece(new Rook{Square = 21});
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

                board.Material[(int) Color.White].Should().Be(material - new Rook().Value);
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

                board.AddPiece(new Rook { Square = 21, Color = Color.White});
                board.HashPiece(board.Squares[21]);

                var positionKey = board.PositionKey;
                positionKey.Should().Be(initialPositionKey);
            }

            [Test]
            public void ShouldUpdateMaterial()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRook);
                int material = board.Material[(int) Color.White];

                var rook = new Rook {Square = 21, Color = Color.White};
                board.AddPiece(rook);

                board.Material[(int) Color.White].Should().Be(material + rook.Value);
            }

            [Test]
            public void ShouldUpdateSquare()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMoveNoWhiteRook);

                var rook = new Rook {Square = 21, Color = Color.White};
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

                var from = new Rook {Square = 21, Color = Color.White};
                var to = new Rook {Square = 31, Color = Color.White};
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
    }
}