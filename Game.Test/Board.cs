using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FluentAssertions;
using NUnit.Framework;

namespace Game.Test
{
    namespace Board
    {
        namespace Fen.WithDefaultString
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

                    board.Squares[21].Should().BeOfType<Rook>();
                    board.Squares[31].Should().BeOfType<Pawn>();
                    board.Squares[41].Should().BeOfType<EmptyPiece>();
                    board.Squares[51].Should().BeOfType<EmptyPiece>();
                    board.Squares[61].Should().BeOfType<EmptyPiece>();
                    board.Squares[71].Should().BeOfType<EmptyPiece>();
                    board.Squares[81].Should().BeOfType<Pawn>();
                    board.Squares[91].Should().BeOfType<Rook>();

                    board.Squares[28].Should().BeOfType<Rook>();
                    board.Squares[38].Should().BeOfType<Pawn>();
                    board.Squares[48].Should().BeOfType<EmptyPiece>();
                    board.Squares[58].Should().BeOfType<EmptyPiece>();
                    board.Squares[68].Should().BeOfType<EmptyPiece>();
                    board.Squares[78].Should().BeOfType<EmptyPiece>();
                    board.Squares[88].Should().BeOfType<Pawn>();
                    board.Squares[98].Should().BeOfType<Rook>();
                }

                [Test]
                public void ShouldHavePiecesWithCorrectSquares()
                {
                    var board = new Game.Board();
                    board.ParseFen(InitialBoardSetup);

                    var squares = new[]
                    {
                        21, 22, 23, 24, 25, 26, 27, 28,
                        31, 32, 33, 34, 35, 36, 37, 38,
                        41, 42, 43, 44, 45, 46, 47, 48,
                        51, 52, 53, 54, 55, 56, 57, 58,
                        61, 62, 63, 64, 65, 66, 67, 68,
                        71, 72, 73, 74, 75, 76, 77, 78,
                        81, 82, 83, 84, 85, 86, 87, 88,
                        91, 92, 93, 94, 95, 96, 97, 98,
                    };

                    foreach (var square in squares)
                    {
                        board.Squares[square].Square.Should().Be(square);
                    }
                }


                [Test]
                public void ShouldCreateCorrectlyColoredPieces()
                {
                    var board = new Game.Board();
                    board.ParseFen(InitialBoardSetup);

                    board.Squares[21].Color.Should().Be(Color.White);
                    board.Squares[22].Color.Should().Be(Color.White);
                    board.Squares[81].Color.Should().Be(Color.Black);
                    board.Squares[91].Color.Should().Be(Color.Black);
                }

                [Test]
                public void ShouldSetSideToMove()
                {
                    var board = new Game.Board();
                    board.ParseFen(InitialBoardSetup);

                    board.SideToMove.Should().Be(Color.White);
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

                    board.EnPassantSquare.Should().Be(0);
                }
            }


        }

        namespace Fen.WithOtherFenString
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

                    board.Squares[91].Should().BeOfType<Rook>();
                    board.Squares[92].Should().BeOfType<EmptyPiece>();
                    board.Squares[93].Should().BeOfType<EmptyPiece>();
                    board.Squares[94].Should().BeOfType<EmptyPiece>();
                    board.Squares[95].Should().BeOfType<King>();
                    board.Squares[96].Should().BeOfType<EmptyPiece>();
                    board.Squares[97].Should().BeOfType<EmptyPiece>();
                    board.Squares[98].Should().BeOfType<Rook>();

                }

                [Test]
                public void ShouldCreateCorrectlyColoredPieces()
                {
                    var board = new Game.Board();
                    board.ParseFen(InitialBoardSetup);

                    board.Squares[21].Color.Should().Be(Color.White);
                    board.Squares[22].Color.Should().Be(Color.White);
                    board.Squares[81].Color.Should().Be(Color.Black);
                    board.Squares[82].Color.Should().Be(Color.Black);
                }

                [Test]
                public void ShouldSetSideToMove()
                {
                    var board = new Game.Board();
                    board.ParseFen(InitialBoardSetup);

                    board.SideToMove.Should().Be(Color.Black);
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
                    board.EnPassantSquare.Should().Be(75);
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

        [TestFixture]
        public class PieceList
        {
            private const string InitialBoardSetupWhiteToMove =
                "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

            private const string InitialBoardSetupBlackToMove =
                "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";

            [Test]
            public void ShouldContainTheCorrectNumberOfWhitePawns()
            {
                var b = new Game.Board();
                b.ParseFen(InitialBoardSetupWhiteToMove);

                b.WhitePawnPieceList.Count(p => p.Color == Color.White).Should().Be(8);
                Game.Lookups.PieceCounts[1].Should().Be(8);
            }

            [Test]
            public void ShouldContainTheCorrectNumberOfWhiteQueens()
            {
                var b = new Game.Board();
                b.ParseFen(InitialBoardSetupWhiteToMove);

//                b.WhiteQueenPieceList.Count(p => p.Color == Color.White).Should().Be(1);
                Game.Lookups.PieceCounts[5].Should().Be(1);
            }

            [Test]
            public void ShouldContainTheCorrectNumberOfWhiteKings()
            {
                var b = new Game.Board();
                b.ParseFen(InitialBoardSetupWhiteToMove);

//                b.WhiteKingPieceList.Count(p => p.Color == Color.White).Should().Be(1);
                Game.Lookups.PieceCounts[6].Should().Be(1);
            }

            [Test]
            public void ShouldContainTheCorrectNumberOfWhiteBishops()
            {
                var b = new Game.Board();
                b.ParseFen(InitialBoardSetupWhiteToMove);

                b.WhiteBishopPieceList.Count(p => p.Color == Color.White).Should().Be(2);
                Game.Lookups.PieceCounts[4].Should().Be(2);
            }

            [Test]
            public void ShouldContainTheCorrectNumberOfWhiteKnights()
            {
                var b = new Game.Board();
                b.ParseFen(InitialBoardSetupWhiteToMove);

                b.WhiteKnightPieceList.Count(p => p.Color == Color.White).Should().Be(2);
                Game.Lookups.PieceCounts[3].Should().Be(2);
            }

            [Test]
            public void ShouldContainTheCorrectNumberOfWhiteRooks()
            {
                var b = new Game.Board();
                b.ParseFen(InitialBoardSetupWhiteToMove);

                b.WhiteRookPieceList.Count(p => p.Color == Color.White).Should().Be(2);
                Game.Lookups.PieceCounts[2].Should().Be(2);
            }

            [Test]
            public void ShouldContainTheCorrectNumberOfBlackPawns()
            {
                var b = new Game.Board();
                b.ParseFen(InitialBoardSetupBlackToMove);

                b.WhitePawnPieceList.Count(p => p.Color == Color.Black).Should().Be(8);
                Game.Lookups.PieceCounts[7].Should().Be(8);
            }

            [Test]
            public void ShouldContainTheCorrectNumberOfBlackQueens()
            {
                var b = new Game.Board();
                b.ParseFen(InitialBoardSetupBlackToMove);

                b.WhiteQueenPieceList.Count(p => p.Color == Color.Black).Should().Be(1);
                Game.Lookups.PieceCounts[11].Should().Be(1);
            }

            [Test]
            public void ShouldContainTheCorrectNumberOfBlackKings()
            {
                var b = new Game.Board();
                b.ParseFen(InitialBoardSetupBlackToMove);

                b.WhiteKingPieceList.Count(p => p.Color == Color.Black).Should().Be(1);
                Game.Lookups.PieceCounts[12].Should().Be(1);
            }

            [Test]
            public void ShouldContainTheCorrectNumberOfBlackBishops()
            {
                var b = new Game.Board();
                b.ParseFen(InitialBoardSetupBlackToMove);

                b.WhiteBishopPieceList.Count(p => p.Color == Color.Black).Should().Be(2);
                Game.Lookups.PieceCounts[10].Should().Be(2);
            }

            [Test]
            public void ShouldContainTheCorrectNumberOfBlackKnights()
            {
                var b = new Game.Board();
                b.ParseFen(InitialBoardSetupBlackToMove);

                b.WhiteKnightPieceList.Count(p => p.Color == Color.Black).Should().Be(2);
                Game.Lookups.PieceCounts[9].Should().Be(2);
            }

            [Test]
            public void ShouldContainTheCorrectNumberOfBlackRooks()
            {
                var b = new Game.Board();
                b.ParseFen(InitialBoardSetupBlackToMove);

                b.WhiteRookPieceList.Count(p => p.Color == Color.Black).Should().Be(2);
                Game.Lookups.PieceCounts[8].Should().Be(2);
            }
        }

        [TestFixture]
        public class GeneratePawnMoves
        {
            [Test]
                public void ShouldGenerateSixteenInitialWhiteMoves()
                {
                    const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                    var board = new Game.Board();
                    board.ParseFen(initialBoardSetup);

                    board.GenerateMoves(board.WhitePawnPieceList);
                    board.Moves[board.Ply].Count.Should().Be(16);
                }

            [Test]
            public void ShouldGenerateSixteenInitialBlackMoves()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhitePawnPieceList);
                board.Moves[board.Ply].Count.Should().Be(16);
            }

            [Test]
            public void ShouldGenerateNoMoveWhenNoMovesAreAvailable()
            {
                const string boardSetup =
                    "rnbqkbnr/rnbqkbnr/rnbqkbnr/rnBqKbnr/rnbPkbnr/rnbqkbnr/rnbqkbnr/rnbqkbnr w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(boardSetup);

                board.GenerateMoves(board.WhitePawnPieceList);
                board.Moves[board.Ply].Count.Should().Be(0);
            }

            [Test]
            public void ShouldGenerateFourMovesWhenWhitePawnIsPromoted()
            {
                const string boardSetup = "8/rnbPkbnr/8/8/8/8/8/8 w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(boardSetup);

                board.GenerateMoves(board.WhitePawnPieceList);
                board.Moves[board.Ply].Count.Should().Be(4);
            }

            [Test]
            public void ShouldGenerateFourMovesWhenBlackPawnIsPromoted()
            {
                const string boardSetup = "8/8/8/8/8/8/rrrrrrrp/8 b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(boardSetup);

                board.GenerateMoves(board.WhitePawnPieceList);
                board.Moves[board.Ply].Count.Should().Be(4);
            }

            [Test]
            public void ShouldGenerateTwoMoveWhenOnlyTwoCapturesAreAvailable()
            {
                const string boardSetup =
                    "rnbqkbnr/rnbqkbnr/rnbqkbnr/rnbqbbnr/rnbPkbnr/rnbqkbnr/rnbqkbnr/rnbqkbnr w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(boardSetup);

                board.GenerateMoves(board.WhitePawnPieceList);
                board.Moves[board.Ply].Count.Should().Be(2);
            }
        }

        [TestFixture]
        public class GenerateRookMoves
        {
            [Test]
            public void ShouldGenerateNoInitialMoves()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteRookPieceList);
                board.Moves[board.Ply].Count.Should().Be(0);
            }

            [Test]
            public void ShouldMoveAndCaptureSingleAvailableTarget()
            {
                const string initialBoardSetup = "8/8/7P/6qr/7b/8/8/8 b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteRookPieceList);
                board.Moves[board.Ply].Count.Should().Be(1);
            }

            [Test]
            public void ShouldMoveAndCaptureAllAvailableTargets()
            {
                const string initialBoardSetup = "8/8/8/3Q4/2QrQ3/3Q4/8/8 b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteRookPieceList);
                board.Moves[board.Ply].Count.Should().Be(4);
            }

            [Test]
            public void ShouldMoveFromCenterToEdgesOfBoard()
            {
                const string initialBoardSetup = "8/8/8/8/3r4/8/8/8 b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteRookPieceList);
                board.Moves[board.Ply].Count.Should().Be(14);
            }

            [Test]
            public void ShouldGenerateOneMoveIfOneWhiteMoveIsAvailable()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKB1R w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteRookPieceList);
                board.Moves[board.Ply].Count.Should().Be(1);
            }

            [Test]
            public void ShouldGenerateSevenMovesIfSevenWhiteMovesAreAvailable()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/7R w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteRookPieceList);
                board.Moves[board.Ply].Count.Should().Be(7);
            }

            [Test]
            public void ShouldGenerateFourteenMovesIfFourteenWhiteMovesAreAvailable()
            {
                const string initialBoardSetup = "rnbqkbn1/ppppppp1/8/8/8/8/PPPPPPP1/7R w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteRookPieceList);
                board.Moves[board.Ply].Count.Should().Be(14);
            }

            [Test]
            public void ShouldGenerateOneMoveIfOneBlackMoveIsAvailable()
            {
                const string initialBoardSetup = "r1bqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteRookPieceList);
                board.Moves[board.Ply].Count.Should().Be(1);
            }

            [Test]
            public void ShouldGenerateSevenMovesIfSevenBlackMovesAreAvailable()
            {
                const string initialBoardSetup = "r7/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteRookPieceList);
                board.Moves[board.Ply].Count.Should().Be(7);
            }

            [Test]
            public void ShouldGenerateFourteenMovesIfFourteenBlackMovesAreAvailable()
            {
                const string initialBoardSetup = "r7/1ppppppp/8/8/8/8/1PPPPPPP/1NBQKBNR b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteRookPieceList);
                board.Moves[board.Ply].Count.Should().Be(14);
            }

        }

        [TestFixture]
        public class GenerateBishopMoves
        {
            [Test]
            public void ShouldGenerateNoInitialMoves()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteBishopPieceList);
                board.Moves[board.Ply].Count.Should().Be(0);
            }

            [Test]
            public void ShouldMoveAndCaptureSingleAvailableTarget()
            {
                const string initialBoardSetup = "8/8/6P1/7b/6p1/8/8/8 b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteBishopPieceList);
                board.Moves[board.Ply].Count.Should().Be(1);
            }

            [Test]
            public void ShouldMoveAndCaptureAllAvailableTargets()
            {
                const string initialBoardSetup = "8/8/8/2P1P3/3b4/2P1P3/8/8 b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteBishopPieceList);
                board.Moves[board.Ply].Count.Should().Be(4);
            }

            [Test]
            public void ShouldMoveFromCenterToEdgesOfBoard()
            {
                const string initialBoardSetup = "8/8/8/8/3b4/8/8/8 b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteBishopPieceList);
                board.Moves[board.Ply].Count.Should().Be(13);
            }

            [Test]
            public void ShouldGenerateOneMoveIfOneWhiteMoveIsAvailable()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/PPPPPPPP/PPPPPP1P/RNBQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteBishopPieceList);
                board.Moves[board.Ply].Count.Should().Be(1);
            }

        }

        [TestFixture]
        public class GenerateKnightMoves
        {
            [Test]
            public void ShouldHaveFourInitialMovesForBlack()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";

                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteKnightPieceList);
                board.Moves[board.Ply].Count.Should().Be(4);
            }

            [Test]
            public void ShouldHaveTwoMovesForBlackKnightOnTheFirstRank()
            {
                const string initialBoardSetup = "rn6/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";

                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteKnightPieceList);
                board.Moves[board.Ply].Count.Should().Be(2);
            }

            [Test]
            public void ShouldHaveTwoMovesForBlackKnightOnTheFirstRankToTheCorrectSquares()
            {
                const string initialBoardSetup = "rn6/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";

                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteKnightPieceList);
                board.Moves[board.Ply][0].PieceToMove.Square.Should().Be(92);
                board.Moves[board.Ply][1].PieceToMove.Square.Should().Be(92);
            }


            [Test]
            public void ShouldHaveFourInitialMovesForBlackToTheCorrectSquares()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";

                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteKnightPieceList);
                board.Moves[board.Ply][0].PieceToMove.Square.Should().Be(92);
                board.Moves[board.Ply][0].ToSquare.Should().Be(73);
                board.Moves[board.Ply][1].ToSquare.Should().Be(71);
            }

            [Test]
            public void ShouldHaveFourInitialMovesForWhite()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteKnightPieceList);
                board.Moves[board.Ply].Count.Should().Be(4);
            }

            [Test]
            public void ShouldHaveEightMovesFromTheMiddleOfTheBoard()
            {
                const string initialBoardSetup = "8/8/8/3n4/8/8/8/8 b KQkq - 0 1";

                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteKnightPieceList);
                board.Moves[board.Ply].Count.Should().Be(8);
            }

            [Test]
            public void ShouldHaveEightCaptureMovesFromTheMiddleOfTheBoard()
            {
                const string initialBoardSetup =
                    "pppppppp/ppBpBppp/pBpppBpp/3n4/pBpppBpp/ppBpBppp/pppppppp/pppppppp b KQkq - 0 1";

                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteKnightPieceList);
                board.Moves[board.Ply].Count.Should().Be(8);
            }
        }

        [TestFixture]
        public class GenerateQueenMoves
        {
            [Test]
            public void ShouldGenerateNoInitialMoves()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteQueenPieceList);
                board.Moves[board.Ply].Count.Should().Be(0);
            }

            [Test]
            public void ShouldMoveAndCaptureSingleAvailableTarget()
            {
                const string initialBoardSetup = "8/8/pppppppP/6rq/bbbbbbbb/8/8/8 b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteQueenPieceList);
                board.Moves[board.Ply].Count.Should().Be(1);
            }

            [Test]
            public void ShouldMoveAndCaptureAllAvailableTargets()
            {
                const string initialBoardSetup = "8/8/8/2BBB2/2BqB3/2BBB2/8/8 b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteQueenPieceList);
                board.Moves[board.Ply].Count.Should().Be(8);
            }

            [Test]
            public void ShouldMoveFromCenterToEdgesOfBoard()
            {
                const string initialBoardSetup = "8/8/8/8/3q4/8/8/8 b KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteQueenPieceList);
                board.Moves[board.Ply].Count.Should().Be(27);
            }

            [Test]
            public void ShouldGenerateOneMoveIfOneWhiteMoveIsAvailable()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQ1BNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteQueenPieceList);
                board.Moves[board.Ply].Count.Should().Be(1);
            }
        }

        [TestFixture]
        public class GenerateKingMoves
        {
            [Test]
            public void ShouldGenerateNoInitialMoves()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteKingPieceList);
                board.Moves[board.Ply].Count.Should().Be(0);
            }

            [Test]
            public void ShouldGenerateAllMovesIfAllDirectionsAreAvailable()
            {
                const string initialBoardSetup = "8/8/8/3K4/8/8/8/8 w - - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteKingPieceList);
                board.Moves[board.Ply].Count.Should().Be(8);
            }

            [Test]
            public void ShouldGenerateAllCaptureMovesIfAllDirectionsAreAvailable()
            {
                const string initialBoardSetup = "8/8/pppppppp/pppKpppp/pppppppp/8/8/8 w - - 0 1";
                var board = new Game.Board();
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteKingPieceList);
                board.Moves[board.Ply].Count.Should().Be(8);
            }

            [Test]
            public void ShouldCastleQueenSideWhenWhiteToMove()
            {
                const string initialBoardSetup = "rnbkbnr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w KQkq - 0 1";
                var board = new Game.Board();
                board.CastlePermission = CastlePermissions.WhiteKing | CastlePermissions.WhiteQueen |
                                         CastlePermissions.BlackKing | CastlePermissions.BlackQueen;
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteKingPieceList);
                board.Moves[board.Ply].Count.Should().Be(2);
            }

            [Test]
            public void ShouldCastleKingSideWhenWhiteToMove()
            {
                const string initialBoardSetup = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RBNQK2R w KQkq - 0 1";
                var board = new Game.Board();
                board.CastlePermission = CastlePermissions.WhiteKing | CastlePermissions.WhiteQueen |
                                         CastlePermissions.BlackKing | CastlePermissions.BlackQueen;
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteKingPieceList);
                board.Moves[board.Ply].Count.Should().Be(2);
            }

            [Test]
            public void ShouldCastleQueenSideWhenBlackToMove()
            {
                const string initialBoardSetup = "r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";
                var board = new Game.Board();
                board.CastlePermission = CastlePermissions.WhiteKing | CastlePermissions.WhiteQueen |
                                         CastlePermissions.BlackKing | CastlePermissions.BlackQueen;
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteKingPieceList);
                board.Moves[board.Ply].Count.Should().Be(2);
            }

            [Test]
            public void ShouldCastleKingSideWhenBlackToMove()
            {
                const string initialBoardSetup = "rnbqk2r/pppppppp/8/8/8/8/PPPPPPPP/RBNQKBNR b KQkq - 0 1";
                var board = new Game.Board();
                board.CastlePermission = CastlePermissions.WhiteKing | CastlePermissions.WhiteQueen |
                                         CastlePermissions.BlackKing | CastlePermissions.BlackQueen;
                board.ParseFen(initialBoardSetup);

                board.GenerateMoves(board.WhiteKingPieceList);
                board.Moves[board.Ply].Count.Should().Be(2);
            }

        }
    }
}