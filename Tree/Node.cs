using System;
using System.Collections.Generic;
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
        public Node(IEvaluator evaluator, Func<int> childMoveCalc, int depth = 3) : this()
        {
            Evaluator = evaluator;
            MaxDepth = depth;
        }

        protected Node()
        {
            Children = new List<Node>();            
        }

        public int MaxDepth { get; set; } //?
        public List<Node> Children { get; private set; }

        public void Add(Node node)
        {
            Children.Add(node);
        }

        public int Score { get; set; }

        public Node Parent { get; set; }
        public Node Root { get; set; }

        public void FindBestMove(int depth)
        {
            int nChildren = Root.CalculateNumberOfValidChildMoves();
            if (depth > 0)
            {
                depth--;
                while (nChildren-- > 0)
                {
                    var node = CreateChild();
                    Children.Add(node);
                    node.FindBestMove(depth);
                }
                Evaluate();                
            }
            else
            {
                Evaluate();                
            }
            
        }

        protected abstract Node CreateChild();

        public Func<int> CalculateNumberOfValidChildMoves = () => 3;

        public virtual void Evaluate()
        {
            if (Children.Count == 0)
            {
                Score = Root.Evaluator.Evaluate();
            }
            else
            {
                Score = GetMiniMaxScore();
            }
        }

        protected abstract int GetMiniMaxScore();

        protected IEvaluator Evaluator;
    }

    public class MinNode : Node
    {
        public MinNode(Node root)
        {
            Root = root;
            Score = int.MinValue;
        }

        protected override Node CreateChild()
        {
            var node = new MaxNode(Root);
            return node;
        }

        protected override int GetMiniMaxScore()
        {
            int minimum = int.MaxValue;
            Children.ForEach(child =>
            {
                minimum = child.Score < minimum ? child.Score : minimum;
            });
            return minimum;
        }
    }
    public class MaxNode : Node
    {
        public MaxNode(IEvaluator evaluator, Func<int> childMoveCalc, int depth = 3)
            : base(evaluator, childMoveCalc, depth)
        {
            Root = this;
            Score = int.MaxValue;
        }

        internal MaxNode(Node root)
        {
            Root = root;
        }


        protected override Node CreateChild()
        {
            var node = new MinNode(Root);
            return node;
        }

        protected override int GetMiniMaxScore()
        {
            int maximum = int.MinValue;
            Children.ForEach(child =>
            {
                maximum = child.Score > maximum ? child.Score : maximum;
            });
            return maximum;
        }
    }
}
