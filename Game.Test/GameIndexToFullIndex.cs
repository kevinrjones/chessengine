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
            int result = Game.Lookups.Map64To120(0);
            result.Should().Be(21);
            result = Game.Lookups.Map64To120(7);
            result.Should().Be(28);
        }

        [Test]
        public void ShouldReturnCorrectIndexWhenFileIsGreaterThanSevenAndLessThanSixteen()
        {
            int result = Game.Lookups.Map64To120(8);
            result.Should().Be(31);
            result = Game.Lookups.Map64To120(15);
            result.Should().Be(38);
        }

        [Test]
        public void ShouldReturnCorrectIndexWhenFileIsGreaterThanTwentyThreeAndLessThanThirtyTwo()
        {
            int result = Game.Lookups.Map64To120(24);
            result.Should().Be(51);
            result = Game.Lookups.Map64To120(31);
            result.Should().Be(58);
        }

    }
}