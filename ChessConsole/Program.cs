using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;

namespace ChessConsole
{
    class Program
    {
        const bool Trace = true;
        static void Main(string[] args)
        {
            var stream = new StreamReader(new FileStream("../../perfsuite.epd", FileMode.Open));
            string fen;
            int count;
            int depth;
            string perftEntry;
            Stopwatch timer = new Stopwatch();
            while ((perftEntry = stream.ReadLine()) != null)
            {
                depth = 5;
                var entries = perftEntry.Split(';');
                fen = entries[0];

                while (depth <= 6)
                {
                    int.TryParse(entries[depth].Split(' ')[1], out count);

                    timer.Start();
                    Console.WriteLine("Running with Fen {0} at depth {1}", fen, depth);
                    var board = new Board();
                    board.ParseFen(fen);

                    var perft = new Perft(board, new ConsoleReporter());
//                    perft.Run(4);
                    perft.Run(depth);
                    timer.Stop();
                    Console.WriteLine("expected {0}, returned {1} in {2}", count, perft.LeafNodes, timer.Elapsed);
                    Console.WriteLine("=============================================================");
                    depth++;
                    if (Trace) break;
                }
                if (Trace) break;
            }
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

    class ConsoleReporter : IReporter
    {
        public void Report(int count)
        {
            throw new NotImplementedException();
        }

        public void Report(string message)
        {
            Console.WriteLine(message);
        }
    }
}
