﻿using System;
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
            Board board = new Board();
            board.ParseFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            board.GeneratePieceList();
            board.GenerateMoves();
            Console.WriteLine(board);
            
            //Console.WriteLine(board.Moves.Count + " moves");
            //foreach (var move in board.Moves)
            //{
            //    Console.WriteLine(move);
            //}
        }
    }
}