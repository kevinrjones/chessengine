using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var root = new MaxNode(_evaluator.Object, _simpleChildCalc);

            root.Add(new MinNode(root));

            Assert.AreEqual(1, root.Children.Count);
        }

        [Test]
        public void ShouldBuildTreeWithThreeChildren()
        {
            var root = new MaxNode(_evaluator.Object, _simpleChildCalc);

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
            var root = new MaxNode(_evaluator.Object, _simpleChildCalc);

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
            var root = new MaxNode(new SimpleEvaluator(), _simpleChildCalc, 1);

            root.FindBestMove(1);

            root.Children.Count.Should().Be(3);
        }

        [Test]
        public void ShouldHaveChildrenWithTheCorrectScores()
        {
            var root = new MaxNode(new SimpleEvaluator(), _simpleChildCalc, 1);

            root.FindBestMove(1);

            int count = 0;
            root.Children.ForEach(child => child.Score.Should().Be(count++));
        }

        [Test]
        public void ShouldHaveTheCorrectRootMinMaxValueWithOnePly()
        {
            int depth = 1;
            var root = new MaxNode(new SimpleEvaluator(), _simpleChildCalc, depth);

            root.FindBestMove(depth);

            root.Score.Should().Be(2);
        }

        [Test]
        public void ShouldHaveTheCorrectRootMinMaxValueWithTwoPly()
        {
            int depth = 2;
            var root = new MaxNode(new SimpleEvaluator(), _simpleChildCalc, depth);

            root.FindBestMove(depth);

            root.Score.Should().Be(6);
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
}
