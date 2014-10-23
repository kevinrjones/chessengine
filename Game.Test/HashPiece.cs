using FluentAssertions;
using NUnit.Framework;

namespace Game.Test.Board
{
    [TestFixture]
    public class HashPiece
    {
        private const int HashPawnSquareOne = 1500507919;
        private const int HashRookSquareOne = 942865737;

        [Test]
        public void ShouldHashASinglePiece()
        {
            var board = new Game.Board();
            var piece = new Pawn { Square = 1 };
            board.HashPiece(piece);

            board.PositionKey.Should().Be(HashPawnSquareOne);
        }

        [Test]
        public void ShouldHashOutASinglePiece()
        {
            var board = new Game.Board();
            var piece = new Pawn { Square = 1 };
            board.HashPiece(piece);
            board.HashPiece(piece);

            board.PositionKey.Should().Be(0);
        }

        [Test]
        public void ShouldHashOutASecondPiece()
        {
            var board = new Game.Board();
            var pawn1 = new Pawn { Square = 1 };
            var pawn2 = new Pawn { Square = 2 };
            board.HashPiece(pawn2);
            board.HashPiece(pawn1);
            board.HashPiece(pawn2);

            board.PositionKey.Should().Be(HashPawnSquareOne);
        }

        [Test]
        public void ShouldHashInTheSide()
        {
            var board = new Game.Board();
            var pawn1 = new Pawn { Square = 1 };
            board.HashSide();
            board.HashPiece(pawn1);

            board.PositionKey.Should().NotBe(HashPawnSquareOne);
            board.HashSide();
            board.PositionKey.Should().Be(HashPawnSquareOne);
        }

        [Test]
        public void ShouldHashInAnEnPassentSquare()
        {
            var board = new Game.Board();
            var enPassantSquare = 42;
            var pawn1 = new Pawn { Square = 1 };
            board.HashEnPassant(enPassantSquare);
            board.HashPiece(pawn1);

            board.PositionKey.Should().NotBe(HashPawnSquareOne);
            board.HashEnPassant(enPassantSquare);
            board.PositionKey.Should().Be(HashPawnSquareOne);
        }

        [Test]
        [Ignore("Longish test; random keys are always the same so this needs only to be run if the algorithm changes")]
        public void NoTeoKeysShouldBeTheSame()
        {
            ZobristHashing hash = new ZobristHashing();

            for (int i = 0; i < hash.PieceKeys.Length; i++)
            {
                for (int j = i + 1; j < hash.PieceKeys.Length; j++)
                {
                    (hash.PieceKeys[i] == hash.PieceKeys[j]).Should().BeFalse();
                }
                for (int j = 0; j < hash.CastleKeys.Length; j++)
                {
                    (hash.PieceKeys[i] == hash.CastleKeys[j]).Should().BeFalse();
                }
                for (int j = 0; j < hash.EnPassantKeys.Length; j++)
                {
                    (hash.PieceKeys[i] == hash.EnPassantKeys[j]).Should().BeFalse();
                }
                (hash.PieceKeys[i] == hash.SideKey).Should().BeFalse();
            }

            for (int i = 0; i < hash.CastleKeys.Length; i++)
            {
                for (int j = i + 1; j < hash.CastleKeys.Length; j++)
                {
                    (hash.CastleKeys[i] == hash.CastleKeys[j]).Should().BeFalse();
                }
                for (int j = 0; j < hash.EnPassantKeys.Length; j++)
                {
                    (hash.CastleKeys[i] == hash.EnPassantKeys[j]).Should().BeFalse();
                }
                (hash.CastleKeys[i] == hash.SideKey).Should().BeFalse();
            }
            for (int i = 0; i < hash.EnPassantKeys.Length; i++)
            {
                for (int j = i + 1; j < hash.EnPassantKeys.Length; j++)
                {
                    (hash.EnPassantKeys[i] == hash.EnPassantKeys[j]).Should().BeFalse();
                }
                (hash.EnPassantKeys[i] == hash.SideKey).Should().BeFalse();
            }
        }
    }
}