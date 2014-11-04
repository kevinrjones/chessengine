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
