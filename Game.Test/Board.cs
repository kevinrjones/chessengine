using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FluentAssertions;
using NUnit.Framework;

namespace Game.Test.Board
{
    namespace DefaultString
    {
        [TestFixture]
        public class ParseFen
        {
            private const string InitialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

            [Test]
            public void ShouldPutPiecesOnCorrectSquares()
            {
                Game.Board board = new Game.Board();
                board.ParseFen(InitialBoardSetup);

                board.Squares[Game.Board.FileRankToSquare(0, 0)].Should().BeOfType<Rook>();
                board.Squares[Game.Board.FileRankToSquare(0, 1)].Should().BeOfType<Pawn>();
                board.Squares[Game.Board.FileRankToSquare(0, 2)].Should().BeOfType<EmptyPiece>();
                board.Squares[Game.Board.FileRankToSquare(0, 3)].Should().BeOfType<EmptyPiece>();
                board.Squares[Game.Board.FileRankToSquare(0, 4)].Should().BeOfType<EmptyPiece>();
                board.Squares[Game.Board.FileRankToSquare(0, 5)].Should().BeOfType<EmptyPiece>();
                board.Squares[Game.Board.FileRankToSquare(0, 6)].Should().BeOfType<Pawn>();
                board.Squares[Game.Board.FileRankToSquare(0, 7)].Should().BeOfType<Rook>();

                board.Squares[Game.Board.FileRankToSquare(7, 0)].Should().BeOfType<Rook>();
                board.Squares[Game.Board.FileRankToSquare(7, 1)].Should().BeOfType<Pawn>();
                board.Squares[Game.Board.FileRankToSquare(7, 2)].Should().BeOfType<EmptyPiece>();
                board.Squares[Game.Board.FileRankToSquare(7, 3)].Should().BeOfType<EmptyPiece>();
                board.Squares[Game.Board.FileRankToSquare(7, 4)].Should().BeOfType<EmptyPiece>();
                board.Squares[Game.Board.FileRankToSquare(7, 5)].Should().BeOfType<EmptyPiece>();
                board.Squares[Game.Board.FileRankToSquare(7, 6)].Should().BeOfType<Pawn>();
                board.Squares[Game.Board.FileRankToSquare(7, 7)].Should().BeOfType<Rook>();
            }

            [Test]
            public void ShouldCreateCorrectlyColoredPieces()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetup);

                board.Squares[Game.Board.FileRankToSquare(0, 0)].Color.Should().Be(Color.Black);
                board.Squares[Game.Board.FileRankToSquare(0, 1)].Color.Should().Be(Color.Black);
                board.Squares[Game.Board.FileRankToSquare(0, 6)].Color.Should().Be(Color.White);
                board.Squares[Game.Board.FileRankToSquare(0, 7)].Color.Should().Be(Color.White);
            }

            [Test]
            public void ShouldSetSideToMove()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetup);

                board.Side.Should().Be(Color.White);
            }

            [Test]
            public void ShouldSetCastlePermission()
            {
                var board = new Game.Board();
                var permission = CastlePermissions.WhiteKing | CastlePermissions.WhiteQueen |
                                 CastlePermissions.BlackKing | CastlePermissions.BlackQueen;
                board.ParseFen(InitialBoardSetup);

                board.CastlePermission.Should().Be(permission);
            }

            [Test]
            public void ShouldLeaveEnPassantSquareEnmpty()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetup);

                board.EnPassant.Should().Be(0);
            }
        }


    }

    namespace OtherFenString
    {
        [TestFixture]
        public class ParseFen
        {
            private const string InitialBoardSetup = "r3k2r/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b - e6 0 1";

            [Test]
            public void ShouldPutPiecesOnCorrectSquares()
            {
                Game.Board board = new Game.Board();
                board.ParseFen(InitialBoardSetup);

                board.Squares[Game.Board.FileRankToSquare(0, 0)].Should().BeOfType<Rook>();
                board.Squares[Game.Board.FileRankToSquare(1, 0)].Should().BeOfType<EmptyPiece>();
                board.Squares[Game.Board.FileRankToSquare(2, 0)].Should().BeOfType<EmptyPiece>();
                board.Squares[Game.Board.FileRankToSquare(3, 0)].Should().BeOfType<EmptyPiece>();
                board.Squares[Game.Board.FileRankToSquare(4, 0)].Should().BeOfType<King>();
                board.Squares[Game.Board.FileRankToSquare(5, 0)].Should().BeOfType<EmptyPiece>();
                board.Squares[Game.Board.FileRankToSquare(6, 0)].Should().BeOfType<EmptyPiece>();
                board.Squares[Game.Board.FileRankToSquare(7, 0)].Should().BeOfType<Rook>();

            }

            [Test]
            public void ShouldCreateCorrectlyColoredPieces()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetup);

                board.Squares[Game.Board.FileRankToSquare(0, 0)].Color.Should().Be(Color.Black);
                board.Squares[Game.Board.FileRankToSquare(0, 1)].Color.Should().Be(Color.Black);
                board.Squares[Game.Board.FileRankToSquare(0, 6)].Color.Should().Be(Color.White);
                board.Squares[Game.Board.FileRankToSquare(0, 7)].Color.Should().Be(Color.White);
            }

            [Test]
            public void ShouldSetSideToMove()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetup);

                board.Side.Should().Be(Color.Black);
            }

            [Test]
            public void ShouldSetCastlePermission()
            {
                var board = new Game.Board();

                board.ParseFen(InitialBoardSetup);

                board.CastlePermission.As<int>().Should().Be(0);
            }

            [Test]
            public void ShouldSetEnPassantSquare()
            {
                var board = new Game.Board();
                board.ParseFen(InitialBoardSetup);

                // ep square == e6, from FEN string set above
                board.EnPassant.Should().Be(75);
            }
        }
    }

    [TestFixture]
    public class ResetBoard
    {
        [Test]
        public void ShouldSetOffBoardSquaresToOffBoard()
        {
            var board = new Game.Board();
            board.ResetBoard();

            board.Squares[0].Should().BeOfType<OffBoardPiece>();
            board.Squares[9].Should().BeOfType<OffBoardPiece>();
            board.Squares[10].Should().BeOfType<OffBoardPiece>();
            board.Squares[19].Should().BeOfType<OffBoardPiece>();
            board.Squares[20].Should().BeOfType<OffBoardPiece>();
            board.Squares[29].Should().BeOfType<OffBoardPiece>();
            board.Squares[110].Should().BeOfType<OffBoardPiece>();
            board.Squares[119].Should().BeOfType<OffBoardPiece>();
        }

        [Test]
        public void ShouldSetNonOffBoardSquaresToOffEmpty()
        {
            var board = new Game.Board();
            board.ResetBoard();

            board.Squares[21].Should().BeOfType<EmptyPiece>();
            board.Squares[28].Should().BeOfType<EmptyPiece>();
            board.Squares[31].Should().BeOfType<EmptyPiece>();
            board.Squares[38].Should().BeOfType<EmptyPiece>();
            board.Squares[81].Should().BeOfType<EmptyPiece>();
            board.Squares[88].Should().BeOfType<EmptyPiece>();
            board.Squares[91].Should().BeOfType<EmptyPiece>();
            board.Squares[98].Should().BeOfType<EmptyPiece>();
        }
    }
}
