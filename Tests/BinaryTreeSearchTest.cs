using PersistentDataStructures.BinarySearch;
using PersistentDataStructures.Shared;
using Xunit;

namespace Tests
{
    public class BinaryTreeSearchTest
    {
        private readonly BinaryTree<int, int> _tree;

        public BinaryTreeSearchTest()
        {
            _tree = new BinaryTree<int, int>();
            _tree.Insert(10, 10);
            _tree.Insert(5, 5);
            _tree.Insert(15, 15);
            _tree.Insert(3, 3);
            _tree.Insert(7, 7);
            _tree.Insert(12, 12);
            _tree.Insert(18, 18);
            _tree.Insert(1, 1);
            _tree.Insert(4, 4);
            _tree.Insert(6, 6);
            _tree.Insert(9, 9);
            _tree.Insert(14, 14);
            _tree.Insert(17, 17);
            _tree.Insert(20, 20);
        }

        [Theory]
        [InlineData(8, 7)]
        [InlineData(2, 1)]
        [InlineData(13, 12)]
        [InlineData(19, 18)]
        [InlineData(30, 20)]
        [InlineData(1, 1)]
        [InlineData(6, 6)]
        [InlineData(20, 20)]
        [InlineData(0, default(int))]
        [InlineData(-10, default(int))]
        public void SearchTest(int value, int expected)
        {
            var result = _tree.FindNearestLess(value);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToListTest()
        {
            var list = _tree.ToList();
            Assert.Equal(1, list[0].Key);
            Assert.Equal(3, list[1].Key);
            Assert.Equal(4, list[2].Key);
            Assert.Equal(5, list[3].Key);
            Assert.Equal(6, list[4].Key);
            Assert.Equal(7, list[5].Key);
            Assert.Equal(9, list[6].Key);
            Assert.Equal(10, list[7].Key);
            Assert.Equal(12, list[8].Key);
            Assert.Equal(14, list[9].Key);
            Assert.Equal(15, list[10].Key);
            Assert.Equal(17, list[11].Key);
            Assert.Equal(18, list[12].Key);
            Assert.Equal(20, list[13].Key);
        }
    }
}