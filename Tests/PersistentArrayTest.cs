using System;
using System.Linq;
using PersistentDataStructures;
using Xunit;

namespace Tests
{
    public class PersistentArrayTest
    {
        [Fact]
        public void AddTest()
        {
            var arr0 = new PersistentArray<int>();

            var arr1 = arr0.Add(3);
            var arr2 = arr1.Add(5);
            var arr3 = arr2.Add(6);
            var arr4 = arr1.Add(7);

            Assert.Throws<ArgumentOutOfRangeException>(() => arr0[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr0[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr0[2]);

            Assert.Equal(3, arr1[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr1[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr1[2]);

            Assert.Equal(3, arr2[0]);
            Assert.Equal(5, arr2[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr2[2]);

            Assert.Equal(3, arr3[0]);
            Assert.Equal(5, arr3[1]);
            Assert.Equal(6, arr3[2]);

            Assert.Equal(3, arr4[0]);
            Assert.Equal(7, arr4[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr4[2]);
        }

        [Fact]
        public void InsertTest()
        {
            var arr0 = new PersistentArray<int>();

            var arr1 = arr0.Insert(0, 3);
            var arr2 = arr1.Insert(0, 5);
            var arr3 = arr2.Insert(1, 6);
            var arr4 = arr1.Insert(1, 7);

            Assert.Throws<ArgumentOutOfRangeException>(() => arr0.Insert(1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => arr0.Insert(-1, 0));

            Assert.Throws<ArgumentOutOfRangeException>(() => arr0[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr0[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr0[2]);

            Assert.Equal(3, arr1[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr1[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr1[2]);

            Assert.Equal(5, arr2[0]);
            Assert.Equal(3, arr2[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr2[2]);

            Assert.Equal(5, arr3[0]);
            Assert.Equal(6, arr3[1]);
            Assert.Equal(3, arr3[2]);

            Assert.Equal(3, arr4[0]);
            Assert.Equal(7, arr4[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr4[2]);
        }

        [Fact]
        public void ReplaceTest()
        {
            var arr0 = new PersistentArray<int>();

            var arr1 = arr0.Add(3);
            var arr2 = arr1.Add(5);
            var arr3 = arr2.Add(6);
            var arr6 = arr1.Add(8);

            var arr4 = arr6.Replace(0, 9);
            var arr5 = arr3.Replace(1, 1);

            Assert.Throws<ArgumentOutOfRangeException>(() => arr0.Replace(1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => arr0.Replace(-1, 0));

            Assert.Equal(9, arr4[0]);
            Assert.Equal(8, arr4[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr4[2]);

            Assert.Equal(3, arr5[0]);
            Assert.Equal(1, arr5[1]);
            Assert.Equal(6, arr5[2]);
        }

        [Fact]
        public void RemoveTest()
        {
            var arr0 = new PersistentArray<int>();

            var arr1 = arr0.Add(3);
            var arr2 = arr1.Add(5);
            var arr3 = arr2.Add(6);
            var arr7 = arr1.Add(7);

            var arr4 = arr2.Remove(0);
            var arr5 = arr7.Remove(1);
            var arr6 = arr3.Remove(1);

            Assert.Equal(5, arr4[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr4[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr4[2]);

            Assert.Equal(3, arr5[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr5[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr5[2]);

            Assert.Equal(3, arr6[0]);
            Assert.Equal(6, arr6[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr6[2]);
        }

        [Fact]
        public void ClearTest()
        {
            var arr0 = new PersistentArray<int>();

            var arr1 = arr0.Add(3);
            var arr2 = arr1.Add(5);
            var arr3 = arr2.Add(6);

            var arr4 = arr2.Clear();
            var arr5 = arr3.Clear();

            Assert.Throws<ArgumentOutOfRangeException>(() => arr4[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr4[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr4[2]);

            Assert.Throws<ArgumentOutOfRangeException>(() => arr5[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr5[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr5[2]);
        }

        [Fact]
        public void UndoTest()
        {
            var arr0 = new PersistentArray<int>();

            var arr1 = arr0.Add(3);
            var arr2 = arr1.Add(5);
            var arr3 = arr2.Add(6);

            var arr4 = arr0.Undo();
            var arr5 = arr2.Undo();
            var arr6 = arr3.Undo();

            Assert.Equal(arr0, arr4);

            Assert.Equal(3, arr5[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr5[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr5[2]);

            Assert.Equal(3, arr6[0]);
            Assert.Equal(5, arr6[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr6[2]);
        }

        [Fact]
        public void RedoTest()
        {
            var arr0 = new PersistentArray<int>();

            var arr1 = arr0.Add(3);
            var arr2 = arr1.Add(5);
            var arr3 = arr2.Add(6);

            var arr4 = arr3.Redo();
            var arr5 = arr0.Redo();
            var arr6 = arr2.Redo();

            Assert.Equal(arr3, arr4);

            Assert.Equal(3, arr5[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr5[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => arr5[2]);

            Assert.Equal(3, arr6[0]);
            Assert.Equal(5, arr6[1]);
            Assert.Equal(6, arr6[2]);
        }

        [Fact]
        public void ToPersistentDictionaryTest()
        {
            var arr0 = new PersistentArray<int>();

            var arr1 = arr0.Add(3);
            var arr2 = arr1.Add(5);
            var arr3 = arr2.Add(6);
            var arr4 = arr3.Add(8);

            var d1 = arr4.ToPersistentDictionary();

            Assert.Equal(3, d1[0]);
            Assert.Equal(5, d1[1]);
            Assert.Equal(6, d1[2]);
            Assert.Equal(8, d1[3]);
        }

        [Fact]
        public void ToPersistentDoubleLinkedListTest()
        {
            var arr0 = new PersistentArray<int>();

            var arr1 = arr0.Add(3);
            var arr2 = arr1.Add(5);
            var arr3 = arr2.Add(6);
            var arr4 = arr3.Add(8);

            var l1 = arr4.ToPersistentDoubleLinkedList();
            var list = l1.ToList();

            Assert.Equal(3, list[0]);
            Assert.Equal(5, list[1]);
            Assert.Equal(6, list[2]);
            Assert.Equal(8, list[3]);
        }
    }
}