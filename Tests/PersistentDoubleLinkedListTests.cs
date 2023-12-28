using System;
using PersistentDataStructures.PersistentList;
using Xunit;

namespace Tests
{
    public class PersistentDoubleLinkedListTests
    {
        [Fact]
        public void AddFirstTest()
        {
            var l0 = new PersistentDoubleLinkedList<int>();
            var l1 = l0.AddFirst(2);
            var l2 = l1.AddFirst(4);
            var l3 = l1.AddFirst(5);
            var l4 = l2.AddFirst(6);

            Assert.Equal(2, l1[0]);
            Assert.Equal(default, l1[1]);

            Assert.Equal(4, l2[0]);
            Assert.Equal(2, l2[1]);
            Assert.Equal(default, l2[2]);

            Assert.Equal(5, l3[0]);
            Assert.Equal(2, l3[1]);
            Assert.Equal(default, l3[2]);

            Assert.Equal(6, l4[0]);
            Assert.Equal(4, l4[1]);
            Assert.Equal(2, l4[2]);
            Assert.Equal(default, l4[3]);
        }

        [Fact]
        public void AddLastTest()
        {
            var l0 = new PersistentDoubleLinkedList<int>();
            var l1 = l0.AddLast(2);
            var l2 = l1.AddLast(4);
            var l3 = l1.AddLast(5);
            var l4 = l2.AddLast(6);

            Assert.Equal(2, l1[0]);
            Assert.Equal(default, l1[1]);

            Assert.Equal(2, l2[0]);
            Assert.Equal(4, l2[1]);
            Assert.Equal(default, l2[2]);

            Assert.Equal(2, l3[0]);
            Assert.Equal(5, l3[1]);
            Assert.Equal(default, l3[2]);

            Assert.Equal(2, l4[0]);
            Assert.Equal(4, l4[1]);
            Assert.Equal(6, l4[2]);
            Assert.Equal(default, l4[3]);
        }

        [Fact]
        public void RemoveFirstTest()
        {
            var l0 = new PersistentDoubleLinkedList<int>();
            var l1 = l0.AddFirst(2);
            var l2 = l1.AddFirst(4);
            var l3 = l2.AddFirst(6);

            var l4 = l2.RemoveFirst();
            var l5 = l3.RemoveFirst();

            var l6 = l5.AddFirst(8);
            var l7 = l6.RemoveFirst();

            var l8 = l0.RemoveFirst();

            Assert.Equal(2, l4[0]);
            Assert.Equal(default, l4[1]);

            Assert.Equal(4, l5[0]);
            Assert.Equal(2, l5[1]);
            Assert.Equal(default, l5[2]);

            Assert.Equal(4, l7[0]);
            Assert.Equal(2, l7[1]);
            Assert.Equal(default, l7[2]);

            Assert.Equal(0, l8.count);
        }

        [Fact]
        public void RemoveLastTest()
        {
            var l0 = new PersistentDoubleLinkedList<int>();
            var l1 = l0.AddLast(2);
            var l2 = l1.AddLast(4);
            var l3 = l2.AddLast(6);

            var l4 = l2.RemoveLast();
            var l5 = l3.RemoveLast();

            var l6 = l5.AddLast(8);
            var l7 = l6.RemoveLast();

            var l8 = l0.RemoveLast();

            Assert.Equal(2, l4[0]);
            Assert.Equal(default, l4[1]);

            Assert.Equal(2, l5[0]);
            Assert.Equal(4, l5[1]);
            Assert.Equal(default, l5[2]);

            Assert.Equal(2, l7[0]);
            Assert.Equal(4, l7[1]);
            Assert.Equal(default, l7[2]);

            Assert.Equal(0, l8.count);
        }

        [Fact]
        public void ClearTest()
        {
            var l0 = new PersistentDoubleLinkedList<int>();
            var l1 = l0.AddLast(2);
            var l2 = l1.AddLast(4);

            var l3 = l1.Clear();
            var l4 = l2.Clear();

            Assert.Equal(0, l3.count);
            Assert.Equal(0, l4.count);

            Assert.Equal(default, l3[0]);
            Assert.Equal(default, l4[0]);
        }

        [Fact]
        public void ReplaceTest()
        {
            var l0 = new PersistentDoubleLinkedList<int>();
            var l1 = l0.AddLast(2);
            var l2 = l1.Replace(0, 4);
            var l3 = l2.Replace(0, 6);

            var l4 = l2.AddLast(8);
            var l5 = l4.Replace(1, 10);

            Assert.Equal(2, l1[0]);
            Assert.Equal(default, l1[1]);

            Assert.Equal(4, l2[0]);
            Assert.Equal(default, l2[1]);

            Assert.Equal(6, l3[0]);
            Assert.Equal(default, l3[1]);

            Assert.Equal(4, l4[0]);
            Assert.Equal(8, l4[1]);
            Assert.Equal(default, l4[2]);

            Assert.Equal(4, l5[0]);
            Assert.Equal(10, l5[1]);
            Assert.Equal(default, l5[2]);
        }

        [Fact]
        public void UndoTest()
        {
            var l0 = new PersistentDoubleLinkedList<int>();
            var l1 = l0.AddLast(2);
            var l2 = l1.Replace(0, 4);
            var l3 = l2.Replace(0, 6);
            var l4 = l3.AddLast(8);
            var l5 = l4.RemoveLast();

            Assert.Equal(2, l1[0]);
            Assert.Equal(default, l1[1]);

            Assert.Equal(4, l2[0]);
            Assert.Equal(default, l2[1]);

            Assert.Equal(6, l3[0]);
            Assert.Equal(default, l3[1]);

            var l6 = l2.Undo();
            var l7 = l3.Undo();
            var l8 = l5.Undo();

            Assert.Equal(2, l6[0]);
            Assert.Equal(default, l6[1]);

            Assert.Equal(4, l7[0]);
            Assert.Equal(default, l7[1]);

            Assert.Equal(6, l8[0]);
            Assert.Equal(8, l8[1]);
            Assert.Equal(default, l8[2]);
        }

        [Fact]
        public void RedoTest()
        {
            var l0 = new PersistentDoubleLinkedList<int>();
            var l1 = l0.AddLast(2);
            var l2 = l1.Replace(0, 4);
            var l3 = l2.Replace(0, 6);
            var l4 = l3.AddLast(8);
            var l5 = l4.RemoveLast();

            Assert.Equal(2, l1[0]);
            Assert.Equal(default, l1[1]);

            Assert.Equal(4, l2[0]);
            Assert.Equal(default, l2[1]);

            Assert.Equal(6, l3[0]);
            Assert.Equal(default, l3[1]);

            var l6 = l2.Undo();
            var l7 = l3.Undo();
            var l8 = l5.Undo();

            var l9 = l6.Redo();
            var l10 = l7.Redo();
            var l11 = l8.Redo();

            Assert.Equal(4, l9[0]);
            Assert.Equal(default, l9[1]);

            Assert.Equal(6, l10[0]);
            Assert.Equal(default, l10[1]);

            Assert.Equal(6, l11[0]);
            Assert.Equal(default, l11[1]);
        }

        [Fact]
        public void ToPersistentArray()
        {
            var l0 = new PersistentDoubleLinkedList<int>();
            var l1 = l0.AddFirst(2);
            var l2 = l1.AddFirst(4);
            var l3 = l2.AddFirst(5);

            var arr0 = l3.ToPersistentArray();

            Assert.Equal(5, arr0[0]);
            Assert.Equal(4, arr0[1]);
            Assert.Equal(2, arr0[2]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr0[3]);
        }
    }
}