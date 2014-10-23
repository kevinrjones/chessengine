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
            public void ShouldRemovePieceFromList()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetupWhiteToMove);

                board.ClearPiece(board.Squares[21]);

                board.RookPieceList.Count.Should().Be(3);
            }
        }
    }
}