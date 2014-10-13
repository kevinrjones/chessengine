using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Game.Test
{
    [TestFixture]
    public class Lookups
    {
        [Test]
        public void ShouldInitializeRanksBoardOffBoardCorrectly()
        {
            Game.Lookups.RanksBoard[0].Should().Be(Game.Lookups.OffBoard);
            Game.Lookups.RanksBoard[9].Should().Be(Game.Lookups.OffBoard);
            Game.Lookups.RanksBoard[10].Should().Be(Game.Lookups.OffBoard);
            Game.Lookups.RanksBoard[19].Should().Be(Game.Lookups.OffBoard);
            Game.Lookups.RanksBoard[110].Should().Be(Game.Lookups.OffBoard);
            Game.Lookups.RanksBoard[111].Should().Be(Game.Lookups.OffBoard);
            Game.Lookups.RanksBoard[118].Should().Be(Game.Lookups.OffBoard);
            Game.Lookups.RanksBoard[119].Should().Be(Game.Lookups.OffBoard);
        }

        [Test]
        public void ShouldInitializeFilesBoardOffBoardCorrectly()
        {
            Game.Lookups.FilesBoard[0].Should().Be(Game.Lookups.OffBoard);
            Game.Lookups.FilesBoard[9].Should().Be(Game.Lookups.OffBoard);
            Game.Lookups.FilesBoard[10].Should().Be(Game.Lookups.OffBoard);
            Game.Lookups.FilesBoard[19].Should().Be(Game.Lookups.OffBoard);
            Game.Lookups.FilesBoard[110].Should().Be(Game.Lookups.OffBoard);
            Game.Lookups.FilesBoard[111].Should().Be(Game.Lookups.OffBoard);
            Game.Lookups.FilesBoard[118].Should().Be(Game.Lookups.OffBoard);
            Game.Lookups.FilesBoard[119].Should().Be(Game.Lookups.OffBoard);
        }

        [Test]
        public void ShouldInitializeFirstRankInRanksBoardCorrectly()
        {
            Game.Lookups.RanksBoard[21].Should().Be(0);
            Game.Lookups.RanksBoard[22].Should().Be(0);
            Game.Lookups.RanksBoard[23].Should().Be(0);
            Game.Lookups.RanksBoard[24].Should().Be(0);
            Game.Lookups.RanksBoard[25].Should().Be(0);
            Game.Lookups.RanksBoard[26].Should().Be(0);
            Game.Lookups.RanksBoard[27].Should().Be(0);
        }
        [Test]
        public void ShouldInitializeSecondRankInRanksBoardCorrectly()
        {
            Game.Lookups.RanksBoard[31].Should().Be(1);
            Game.Lookups.RanksBoard[32].Should().Be(1);
            Game.Lookups.RanksBoard[33].Should().Be(1);
            Game.Lookups.RanksBoard[34].Should().Be(1);
            Game.Lookups.RanksBoard[35].Should().Be(1);
            Game.Lookups.RanksBoard[36].Should().Be(1);
            Game.Lookups.RanksBoard[37].Should().Be(1);
        }
        [Test]
        public void ShouldInitializeLastRankInRanksBoardCorrectly()
        {
            Game.Lookups.RanksBoard[91].Should().Be(7);
            Game.Lookups.RanksBoard[92].Should().Be(7);
            Game.Lookups.RanksBoard[93].Should().Be(7);
            Game.Lookups.RanksBoard[94].Should().Be(7);
            Game.Lookups.RanksBoard[95].Should().Be(7);
            Game.Lookups.RanksBoard[96].Should().Be(7);
            Game.Lookups.RanksBoard[97].Should().Be(7);
        }
        [Test]
        public void ShouldInitializeFirstFileInFilesBoardCorrectly()
        {
            Game.Lookups.FilesBoard[21].Should().Be(0);
            Game.Lookups.FilesBoard[31].Should().Be(0);
            Game.Lookups.FilesBoard[41].Should().Be(0);
            Game.Lookups.FilesBoard[51].Should().Be(0);
            Game.Lookups.FilesBoard[61].Should().Be(0);
            Game.Lookups.FilesBoard[71].Should().Be(0);
            Game.Lookups.FilesBoard[81].Should().Be(0);
        }
        [Test]
        public void ShouldInitializeSecondFileInFilesBoardCorrectly()
        {
            Game.Lookups.FilesBoard[22].Should().Be(1);
            Game.Lookups.FilesBoard[32].Should().Be(1);
            Game.Lookups.FilesBoard[42].Should().Be(1);
            Game.Lookups.FilesBoard[52].Should().Be(1);
            Game.Lookups.FilesBoard[62].Should().Be(1);
            Game.Lookups.FilesBoard[72].Should().Be(1);
            Game.Lookups.FilesBoard[82].Should().Be(1);
        }
        [Test]
        public void ShouldInitializeLastFileInFilesBoardCorrectly()
        {
            Game.Lookups.FilesBoard[28].Should().Be(7);
            Game.Lookups.FilesBoard[38].Should().Be(7);
            Game.Lookups.FilesBoard[48].Should().Be(7);
            Game.Lookups.FilesBoard[58].Should().Be(7);
            Game.Lookups.FilesBoard[68].Should().Be(7);
            Game.Lookups.FilesBoard[78].Should().Be(7);
            Game.Lookups.FilesBoard[88].Should().Be(7);
        }
    }
}
