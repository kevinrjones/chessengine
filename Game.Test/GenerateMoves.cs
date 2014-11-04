using System.Runtime.InteropServices;
using FluentAssertions;
using NUnit.Framework;

namespace Game.Test
{
    namespace Board
    {
        [TestFixture]
        public class GenerateMoves
        {
            [Test]
            public void ShouldGenerateTwentyInitialWhiteMoves()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves();
                board.Moves[board.Ply].Count.Should().Be(20);
            }

            [Test]
            public void ShouldGenerateTwentyInitialBlackMoves()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves();
                board.Moves[board.Ply].Count.Should().Be(20);
            }

            [Test]
            public void ShouldUpdatePlyAfterMakingMove()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves();
                board.MakeMove(board.Moves[0][0]);
                board.Ply.Should().Be(1);
            }

            [Test]
            public void ShouldSetInitialMoveCorrectly()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves();
                board.MakeMove(board.Moves[0][0]);
                board.Moves[0][0].ToSquare.Should().Be(41);
            }

            [Test]
            public void ShouldGenerateTwentyInitialReplies()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves();
                board.MakeMove(board.Moves[0][0]);
                board.GenerateMoves();
                board.Moves[board.Ply].Count.Should().Be(20);
            }

            [Test]
            public void ShouldGenerateInitialBlackMoveCorrectly()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves();
                board.MakeMove(board.Moves[0][0]);
                board.GenerateMoves();
                board.Moves[board.Ply][0].ToSquare.Should().Be(71);
            }

            [Test]
            public void ShouldGenerateFourHundredMovesAtDepthOneFromInitialPosition()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves();
                for (int count = 0; count < board.Moves[board.Ply].Count; count++)
                {
                    if (board.MakeMove(board.Moves[0][count]))
                    {
                        board.GenerateMoves();
                        board.TakeMove();
                    }
                }
                board.Moves[0].Count.Should().Be(20);
                board.Moves[1].Count.Should().Be(20);
            }

            [Test]
            public void ShouldGenerateTwentyNineMovesFromBA3()
            {
                const string boardSetup = "rnbqkbnr/pppp1ppp/4p3/8/1P6/B7/P1PPPPPP/RN1QKBNR b KQkq - 0 1";                                          
                var board = new Game.Board();
                board.ParseFen(boardSetup);

                board.GenerateMoves();
                board.Moves[board.Ply].Count.Should().Be(29);
            }
        }
    }
}