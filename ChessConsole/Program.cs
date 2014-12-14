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
        const bool Trace = false;
        static void Main(string[] args)
        {
            using (var stream = new StreamReader(new FileStream("../../perfsuite.epd", FileMode.Open)))
            {
                string fen;
                int count;
                int depth;
                string perftEntry;
                Stopwatch timer = new Stopwatch();
                while ((perftEntry = stream.ReadLine()) != null)
                {
                    depth = 6;
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
                        perft.Run(depth);
                        timer.Stop();
                        if (count == perft.LeafNodes)
                        {
                            Console.WriteLine("expected {0}, returned {1} in {2}", count, perft.LeafNodes, timer.Elapsed);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("expected {0}, returned {1} in {2}", count, perft.LeafNodes, timer.Elapsed);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        Console.WriteLine("=============================================================");
                        depth++;
                        if (Trace) break;
                    }
                    if (Trace) break;
                }
            }
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
