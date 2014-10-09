using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;

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

        public List<Node> Children { get; private set; }

        public void Add(Node node)
        {
            Children.Add(node);
        }

        public int Alpha = int.MinValue;
        public int Beta = int.MaxValue;

        public int Score { get; set; }
        public Node Parent { get; set; }
        public RootNode Root { get; set; }

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
                Console.WriteLine("Evaluated nodes: {0}, Score: {1}", Root.EvaluatedNodes, Score);
                TrySetAlphaBeta(Score);
            }
            else
            {
                Score = GetMiniMaxScore();
            }
        }

        protected abstract void TrySetAlphaBeta(int score);

        protected abstract Node CreateChild(int alpha, int beta);
        protected abstract int GetMiniMaxScore();

        public void TrySetBeta(int minimum)
        {
            Beta = minimum < Beta ? minimum : Beta;
            Console.WriteLine("Beta: {0}, Score: {1}", Beta, Score);
        }

        public void TrySetAlpha(int maximum)
        {
            Alpha = maximum > Alpha ? maximum : Alpha;
            Console.WriteLine("Alpha: {0}, Score: {1}", Alpha, Score);
        }
    }

    public class MinNode : Node
    {
        public MinNode(RootNode root, Node parent)
        {
            Root = root;
            Parent = parent;
            Score = int.MinValue;
        }

        protected override void TrySetAlphaBeta(int score)
        {
            if (Parent != null) Parent.TrySetAlpha(score);
        }

        protected override Node CreateChild(int alpha, int beta)
        {
            Console.WriteLine("{2}: Alpha: {0}, Beta: {1}", alpha, beta, "Max");
            return new MaxNode(Root, this) { Alpha = alpha, Beta = beta };
        }

        protected override int GetMiniMaxScore()
        {
            int minimum = int.MaxValue;
            Children.ForEach(child =>
            {
                minimum = child.Score < minimum ? child.Score : minimum;
            });
            TrySetAlphaBeta(minimum);
            return minimum;
        }
    }
    public class MaxNode : Node
    {
        public MaxNode()
        {
            
        }
        internal MaxNode(RootNode root, Node parent)
        {
            Root = root;
            Parent = parent;
            Score = int.MaxValue;
        }

        protected override void TrySetAlphaBeta(int score)
        {
            if (Parent != null) Parent.TrySetBeta(score);
        }

        protected override Node CreateChild(int alpha, int beta)
        {
            Console.WriteLine("{2}: Alpha: {0}, Beta: {1}", alpha, beta, "Min");
            return new MinNode(Root, this) { Alpha = alpha, Beta = beta };
        }

        protected override int GetMiniMaxScore()
        {
            int maximum = int.MinValue;
            Children.ForEach(child =>
            {
                maximum = child.Score > maximum ? child.Score : maximum;
            });
            TrySetAlphaBeta(maximum);
            return maximum;
        }
    }

    public class RootNode : MaxNode
    {
        internal IEvaluator Evaluator;
        public Func<int> CalculateNumberOfValidChildMoves = () => 3;
        public int MaxDepth { get; set; }
        public int EvaluatedNodes { get; set; }

        public RootNode(IEvaluator evaluator, Func<int> childMoveCalc, int depth = 3) 
        {
            Root = this;
            Score = int.MaxValue;
            Evaluator = evaluator;
            MaxDepth = depth;
            CalculateNumberOfValidChildMoves = childMoveCalc;
        }        
    }
}
