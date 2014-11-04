using System;
using System.Collections.Generic;
using Game;

public class Perft
{
    private readonly Board _board;

    public Perft(Board board)
    {
        _board = board;
    }

    public int LeafNodes{ get; set; }
    public void Run(int depth)
    {
        if (depth == 0)
        {
            LeafNodes++;
            return;
        }

        _board.GenerateMoves();

        foreach (var move in _board.Moves[_board.Ply])
        {
            if (!_board.MakeMove(move))
            {
                continue;
            }
            Run(depth-1);
            _board.TakeMove();
        }        
    }

    public void RunWithCounts(int depth)
    {
        int moveNumber = 0;
        LeafNodes = 0;

        _board.GenerateMoves();
        foreach (var move in _board.Moves[_board.Ply])
        {
            if (!_board.MakeMove(move))
            {
                continue;
            }
            moveNumber++;
            int cumalativeNodes = LeafNodes;
            Run(depth - 1);
            _board.TakeMove();
            int oldNodes = LeafNodes - cumalativeNodes;

            Console.WriteLine("move: " + moveNumber + " " + move + " " + oldNodes);
        }        
        
        Console.WriteLine("Test complete " + LeafNodes + " visited"); 
    }
}

