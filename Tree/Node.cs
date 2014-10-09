using System;
using System.Collections.Generic;

namespace Tree
{
    public interface IEvaluator
    {
        int Evaluate();
    }
    public class ChessEvaluator : IEvaluator
    {
        public int Evaluate()
        {
            return 1;
        }
    }

    public abstract class Node
    {

        protected Node()
        {
            Children = new List<Node>();
        }

        public void Add(Node node)
        {
            Children.Add(node);
        }

        public List<Node> Children { get; private set; }
        public int Score { get; set; }

        internal int Alpha = int.MinValue;
        internal int Beta = int.MaxValue;
        protected Node Parent { get; set; }
        protected RootNode Root { get; set; }
        protected int InitialScoreForMiniMax { get; set; }

        public void FindBestMove(int depth)
        {
            if (depth > 0)
            {
                depth--;
                int nChildren = Root.CalculateNumberOfValidChildMoves();
                while (nChildren-- > 0)
                {
                    if (Beta <= Alpha) break;
                    var node = CreateChild(Alpha, Beta);
                    Children.Add(node);
                    node.FindBestMove(depth);
                }
                if (Beta > Alpha)
                {
                    Evaluate();
                }
            }
            else
            {
                Evaluate();
            }
        }

        public virtual void Evaluate()
        {
            if (Children.Count <= 0)
            {
                Root.EvaluatedNodes++;
                Score = Root.Evaluator.Evaluate();
                TrySetAlphaBeta(Score);
            }
            else
            {
                Score = GetMiniMaxScore();
            }
        }

        internal void TrySetBeta(int minimum)
        {
            Beta = minimum < Beta ? minimum : Beta;
        }

        internal void TrySetAlpha(int maximum)
        {
            Alpha = maximum > Alpha ? maximum : Alpha;
        }

        protected abstract void TrySetAlphaBeta(int score);
        protected abstract Node CreateChild(int alpha, int beta);
        protected abstract int GetScore(Node child, int score);
        protected virtual int GetMiniMaxScore()
        {
            int score = InitialScoreForMiniMax;
            Children.ForEach(child =>
            {
                score = GetScore(child, score);
            });
            TrySetAlphaBeta(score);
            return score;
        }

    }

    public class MinNode : Node
    {
        public MinNode(RootNode root, Node parent)
        {
            Root = root;
            Parent = parent;
            Score = int.MinValue;
            InitialScoreForMiniMax = int.MaxValue;
        }

        protected override void TrySetAlphaBeta(int score)
        {
            if (Parent != null) Parent.TrySetAlpha(score);
        }

        protected override Node CreateChild(int alpha, int beta)
        {
            return new MaxNode(Root, this) { Alpha = alpha, Beta = beta };
        }

        protected override int GetScore(Node child, int score)
        {
            return child.Score < score ? child.Score : score;
        }
    }
    public class MaxNode : Node
    {
        protected MaxNode() { }

        internal MaxNode(RootNode root, Node parent)
        {
            Root = root;
            Parent = parent;
            Score = int.MaxValue;
            InitialScoreForMiniMax = int.MinValue;
        }

        protected override void TrySetAlphaBeta(int score)
        {
            if (Parent != null) Parent.TrySetBeta(score);
        }

        protected override Node CreateChild(int alpha, int beta)
        {
            return new MinNode(Root, this) { Alpha = alpha, Beta = beta };
        }

        protected override int GetScore(Node child, int score)
        {
            return child.Score > score ? child.Score : score;
        }
    }

    public class RootNode : MaxNode
    {
        internal IEvaluator Evaluator;
        public Func<int> CalculateNumberOfValidChildMoves = () => 3;
        public int EvaluatedNodes { get; set; }

        public RootNode(IEvaluator evaluator, Func<int> childMoveCalc)
        {
            Root = this;
            Evaluator = evaluator;
            CalculateNumberOfValidChildMoves = childMoveCalc;
        }
    }
}
