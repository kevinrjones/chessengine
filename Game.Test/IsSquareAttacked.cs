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
        }
    }
}