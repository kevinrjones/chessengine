using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Tree.Test
{
    [TestFixture]
    public class Tree
    {
        private Mock<IEvaluator> _evaluator;
        private readonly Func<int> _simpleChildCalc = () => 3;

        [SetUp]
        public void Setup()
        {
            _evaluator = new Mock<IEvaluator>();
        }

        [Test]
        public void ShouldHaveChildren()
        {
            var root = new RootNode(_evaluator.Object, _simpleChildCalc);

            root.Add(new MinNode(root, root));

            Assert.AreEqual(1, root.Children.Count);
        }

        [Test]
        public void ShouldBuildTreeWithThreeChildren()
        {
            var root = new RootNode(_evaluator.Object, _simpleChildCalc);

            root.FindBestMove(3);
            root.Children.Should().NotBeNull();
            var firstChild = root.Children[0];
            var secondChild = firstChild.Children[0];
            var thirdChild = secondChild.Children[0];
            thirdChild.Should().NotBeNull();
        }

        [Test]
        public void ShouldBuildTreeWithNoMoreThanThreeChildren()
        {
            var root = new RootNode(_evaluator.Object, _simpleChildCalc);

            root.FindBestMove(3);
            root.Children.Should().NotBeNull();
            var firstChild = root.Children[0];
            var secondChild = firstChild.Children[0];
            var thirdChild = secondChild.Children[0];
            thirdChild.Children.Should().BeEmpty();
        }

        [Test]
        public void ShouldCreateTheCorrectNumberOfChildren()
        {
            var root = new RootNode(new SimpleEvaluator(), _simpleChildCalc);

            root.FindBestMove(1);

            root.Children.Count.Should().Be(3);
        }

        [Test]
        public void ShouldHaveChildrenWithTheCorrectScores()
        {
            var root = new RootNode(new SimpleEvaluator(), _simpleChildCalc);

            root.FindBestMove(1);

            int count = 0;
            root.Children.ForEach(child => child.Score.Should().Be(count++));
        }

        [Test]
        public void ShouldHaveTheCorrectRootMinMaxValueWithOnePly()
        {
            int depth = 1;
            var root = new RootNode(new SimpleEvaluator(), _simpleChildCalc);

            root.FindBestMove(depth);

            root.Score.Should().Be(2);
        }

        [Test]
        public void ShouldHaveTheCorrectRootMinMaxValueWithTwoPly()
        {
            int depth = 2;
            var root = new RootNode(new SimpleEvaluator(), _simpleChildCalc);

            root.FindBestMove(depth);

            root.Score.Should().Be(6);
        }

        [Test]
        public void ShouldHaveTheCorrectRootMinMaxValueWithThreePly()
        {
            int depth = 3;
            var root = new RootNode(new AlphaBeta3PlyEvaluator(), _simpleChildCalc);

            root.FindBestMove(depth);

            root.Score.Should().Be(20);
        }
        [Test]
        public void ShouldHaveTheCorrectRootMinMaxValueWithFourPly()
        {
            int depth = 4;
            var root = new RootNode(new AlphaBeta4PlyEvaluator(), () => 2);

            root.FindBestMove(depth);

            root.Score.Should().Be(3);
        }
    }

    class SimpleEvaluator : IEvaluator
    {
        private int _count;
        public int Evaluate()
        {
            return _count++;
        }
    }

    class AlphaBeta3PlyEvaluator : IEvaluator
    {
        private readonly int[] _values = { 0, 1, 2, 3, 6, 9, 10, 11, 12, 15, 18, 19, 20, 21, 24};
        private int _count;
        public int Evaluate()
        {
            return _values[_count++];
        }
    }

    /*
     * For testing alpha-beta (see: )
     * 
     * Nodes per ply = 2
     * depth = 4
     * evals are 3, 17 | 2, 12 | 15, 16 | 25, 0 | 2, 5 | 3, 1 | 2, 14 | 1, 1 
     */
    class AlphaBeta4PlyEvaluator : IEvaluator
    {
        private readonly int[] _values = { 3, 17, 2, 15, 16, 2, 3 };
        private int _count;
        public int Evaluate()
        {
            return _values[_count++];
        }
    }
}
