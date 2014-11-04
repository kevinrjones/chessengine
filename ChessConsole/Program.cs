using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;

namespace ChessConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var board = new Board();
            board.ParseFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            // d2d3
            //board.ParseFen("rnbqkbnr/pppppppp/8/8/8/3P4/PPP1PPPP/RNBQKBNR b KQkq - 0 1");

            // c7c6
            //board.ParseFen("rnbqkbnr/pp1ppppp/2p5/8/8/3P4/PPP1PPPP/RNBQKBNR w KQkq - 0 1");

            // e1d2
            //board.ParseFen("rnbqkbnr/pp1ppppp/2p5/8/8/3P4/PPPKPPPP/RNBQ1BNR b KQkq - 0 1");

            // d8a5
            //board.ParseFen("rnb1kbnr/pp1ppppp/2p5/q7/8/3P4/PPPKPPPP/RNBQ1BNR w KQkq - 0 1");

            var perft = new Perft(board);
            perft.RunWithCounts(5);
            Console.WriteLine(perft.LeafNodes);

            //board.GeneratePieceLists();
            //board.GenerateMoves();
            //Console.WriteLine(board);

            //var m = new Move(new Pawn{Square = 34, Color = Color.White}, 44);

            //board.MakeMove(m);
            //Console.WriteLine(board);

            //board.TakeMove();
            //Console.WriteLine(board);
            //Console.WriteLine(board.Moves.Count + " moves");
            //foreach (var move in board.Moves)
            //{
            //    Console.WriteLine(move);
            //}
        }
    }
}
