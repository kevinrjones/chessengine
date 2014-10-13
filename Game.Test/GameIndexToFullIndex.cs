using FluentAssertions;
using NUnit.Framework;

namespace Game.Test.Board
{
    [TestFixture]
    public class GameIndexToFullIndex
    {
        [Test]
        public void ShouldReturnCorrectIndexWhenFileIsLessThanEight()
        {
            int result = Game.Board.GameIndexToFullIndex(0);
            result.Should().Be(21);
            result = Game.Board.GameIndexToFullIndex(7);
            result.Should().Be(28);
        }

        [Test]
        public void ShouldReturnCorrectIndexWhenFileIsGreaterThanSevenAndLessThanSixteen()
        {
            int result = Game.Board.GameIndexToFullIndex(8);
            result.Should().Be(31);
            result = Game.Board.GameIndexToFullIndex(15);
            result.Should().Be(38);
        }

        [Test]
        public void ShouldReturnCorrectIndexWhenFileIsGreaterThanTwentyThreeAndLessThanThirtyTwo()
        {
            int result = Game.Board.GameIndexToFullIndex(24);
            result.Should().Be(51);
            result = Game.Board.GameIndexToFullIndex(31);
            result.Should().Be(58);
        }

    }
}