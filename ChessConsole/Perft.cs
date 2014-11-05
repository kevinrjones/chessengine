using System;
using System.Collections.Generic;
using ChessConsole;
using Game;

public class Perft
{
    private readonly Board _board;
    private readonly IReporter _reporter;

    public Perft(Board board, IReporter reporter)
    {
        _board = board;
        _reporter = reporter;
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

            _reporter.Report("move: " + moveNumber + " " + move + " " + oldNodes);
        }

        _reporter.Report("Test complete " + LeafNodes + " visited"); 
    }
}