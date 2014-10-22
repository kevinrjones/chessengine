using FluentAssertions;
using NUnit.Framework;

namespace Game.Test
{
    namespace Board
    {
        [TestFixture]
        public class IsSquareAttacked
        {
            private Game.Board _board;

            [SetUp]
            public void Setup()
            {
                _board = new Game.Board();
                for (int i = 0; i < _board.Squares.Length; i++)
                {
                    _board.Squares[i] = new OffBoardPiece { Color = Color.White };
                }

            }

            [Test]
            public void ShouldNotBeAttackedByAnEmptySquareFromTheLeft()
            {

                _board.Squares[32] = new EmptyPiece();
                _board.IsSquareAttacked(43, Color.White).Should().BeFalse();
            }

            [Test]
            public void ShouldNotBeAttackedByAnEmptyFromTheRight()
            {
                _board.Squares[34] = new EmptyPiece();
                _board.IsSquareAttacked(43, Color.White).Should().BeFalse();
            }

            [Test]
            public void ShouldNotBeAttackedByAPawnOfTheSameColor()
            {
                _board.Squares[32] = new Pawn { Color = Color.Black };
                _board.Squares[34] = new Pawn { Color = Color.Black };
                _board.IsSquareAttacked(43, Color.White).Should().BeFalse();
            }

            [Test]
            public void ShouldBeAttackedByAPawnOfADifferentColorFromTheLeft()
            {
                _board.Squares[32] = new Pawn { Color = Color.White };
                _board.IsSquareAttacked(43, Color.White).Should().BeTrue();
            }

            [Test]
            public void ShouldBeAttackedByAPawnOfADifferentColorFromTheRight()
            {
                _board.Squares[34] = new Pawn { Color = Color.White };
                _board.IsSquareAttacked(43, Color.White).Should().BeTrue();
            }

            [Test]
            public void ShouldNotBeAttackedByAKnightOfTheSameColor()
            {
                const int attackSquare = 43;
                foreach (var dir in Knight.MoveDirection)
                {
                    _board.Squares[attackSquare + dir] = new Knight { Color = Color.Black };

                }
                _board.IsSquareAttacked(attackSquare, Color.White).Should().BeFalse();
            }

            [TestCase(0)]
            [TestCase(1)]
            [TestCase(2)]
            [TestCase(3)]
            [TestCase(4)]
            [TestCase(5)]
            [TestCase(6)]
            [TestCase(7)]
            public void ShouldBeAttackedAKnightOfADifferentColor(int index)
            {
                const int attackSquare = 43;
                foreach (var dir in Knight.MoveDirection)
                {
                    _board.Squares[attackSquare + dir] = new Knight { Color = Color.Black };

                }

                _board.Squares[attackSquare + Knight.MoveDirection[index]] = new Knight { Color = Color.White };

                _board.IsSquareAttacked(attackSquare, Color.White).Should().BeTrue();
            }

            [Test]
            public void ShouldNotBeAttackedByAKingOfTheSameColor()
            {
                const int attackSquare = 43;
                foreach (var dir in King.MoveDirection)
                {
                    _board.Squares[attackSquare + dir] = new King() { Color = Color.Black };

                }
                _board.IsSquareAttacked(attackSquare, Color.White).Should().BeFalse();
            }

            [TestCase(0)]
            [TestCase(1)]
            [TestCase(2)]
            [TestCase(3)]
            [TestCase(4)]
            [TestCase(5)]
            [TestCase(6)]
            [TestCase(7)]
            public void ShouldBeAttackedAKingOfADifferentColor(int index)
            {
                const int attackSquare = 43;
                foreach (var dir in King.MoveDirection)
                {
                    _board.Squares[attackSquare + dir] = new King { Color = Color.Black };

                }

                _board.Squares[attackSquare + King.MoveDirection[index]] = new King { Color = Color.White };

                _board.IsSquareAttacked(attackSquare, Color.White).Should().BeTrue();

            }

            [Test]
            public void ShouldNotBeAttackedByARookOfTheSameColor()
            {
                const int attackSquare = 43;
                foreach (var dir in Rook.MoveDirection)
                {
                    _board.Squares[attackSquare + dir] = new Rook() { Color = Color.Black };

                }
                _board.IsSquareAttacked(attackSquare, Color.White).Should().BeFalse();
            }

            [Test]
            public void ShouldNotBeAttackedByARookThatIsBehindAPieceOfTheSameColorAsTheSquareUnderAttack()
            {
                const int attackSquare = 64;
                const string initialBoardSetup = "8/3r4/3B4/1rBPBr2/3B4/3r4/8/8 b KQkq - 0 1";
                _board.ParseFen(initialBoardSetup);

                _board.IsSquareAttacked(attackSquare, Color.Black).Should().BeFalse();
            }

            [TestCase("8/8/2b5/3P4/8/8/8/8 b KQkq - 0 1")]
            [TestCase("8/8/4b3/3P4/8/8/8/8 b KQkq - 0 1")]
            [TestCase("8/8/8/3P4/2b5/8/8/8 b KQkq - 0 1")]
            [TestCase("8/8/8/3P4/4b3/8/8/8 b KQkq - 0 1")]
            public void ShouldBeAttackedByABishop(string fen)
            {
                const int attackSquare = 64;
                _board.ParseFen(fen);

                _board.IsSquareAttacked(attackSquare, Color.Black).Should().BeTrue();
            }


            [Test]
            public void ShouldNotBeAttackedByABishopThatIsBehindAPieceOfTheSameColorAsTheSquareUnderAttack()
            {
                const int attackSquare = 64;
                const string initialBoardSetup = "8/1b3b2/2P1P3/3P4/2P1P3/1b3b2/8/8 b KQkq - 0 1";
                _board.ParseFen(initialBoardSetup);

                _board.IsSquareAttacked(attackSquare, Color.Black).Should().BeFalse();
            }
        }
    }
}