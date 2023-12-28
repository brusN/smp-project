using System;
using PersistentDataStructures;
using Xunit;

namespace Tests
{
    public class PersistentDictionaryTest
    {
        [Fact]
        public void AddTest()
        {
            var d0 = new PersistentDictionary<int, int>();
            var d1 = d0.Add(3, 3);
            var d2 = d1.Add(4, 4);
            var d4 = d1.Add(5, 5);
            var d3 = d1.Add(6, 6);

            Assert.Equal(default, d0[3]);
            Assert.Equal(default, d0[4]);
            Assert.Equal(default, d0[5]);
            Assert.Equal(default, d0[6]);

            Assert.Equal(3, d1[3]);
            Assert.Equal(default, d1[4]);
            Assert.Equal(default, d1[5]);
            Assert.Equal(default, d1[6]);

            Assert.Equal(3, d2[3]);
            Assert.Equal(4, d2[4]);
            Assert.Equal(default, d2[5]);
            Assert.Equal(default, d2[6]);

            Assert.Equal(3, d3[3]);
            Assert.Equal(default, d3[4]);
            Assert.Equal(default, d3[5]);
            Assert.Equal(6, d3[6]);

            Assert.Equal(3, d4[3]);
            Assert.Equal(default, d4[4]);
            Assert.Equal(5, d4[5]);
            Assert.Equal(default, d4[6]);

            Assert.Throws<ArgumentException>(() => d1.Add(3, 5));
        }

        [Fact]
        public void RemoveTest()
        {
            var d0 = new PersistentDictionary<int, int>();
            var d1 = d0.Add(3, 3);
            var d2 = d1.Remove(3);
            var d3 = d1.Add(5, 5);
            var d4 = d3.Remove(3);

            Assert.Equal(3, d1[3]);
            Assert.Equal(default, d1[5]);

            Assert.Equal(default, d2[3]);
            Assert.Equal(default, d2[5]);

            Assert.Equal(3, d3[3]);
            Assert.Equal(5, d3[5]);

            Assert.Equal(default, d4[3]);
            Assert.Equal(5, d4[5]);
        }

        [Fact]
        public void ClearTest()
        {
            var d0 = new PersistentDictionary<int, int>();
            var d1 = d0.Add(3, 3);
            var d2 = d1.Add(4, 4);
            var d3 = d2.Add(5, 5);
            var d4 = d2.Clear();
            var d5 = d3.Clear();

            Assert.Equal(3, d1[3]);
            Assert.Equal(default, d1[4]);
            Assert.Equal(default, d1[5]);

            Assert.Equal(3, d2[3]);
            Assert.Equal(4, d2[4]);
            Assert.Equal(default, d2[5]);

            Assert.Equal(3, d3[3]);
            Assert.Equal(4, d3[4]);
            Assert.Equal(5, d3[5]);

            Assert.Equal(default, d4[3]);
            Assert.Equal(default, d4[4]);
            Assert.Equal(default, d4[5]);

            Assert.Equal(default, d5[3]);
            Assert.Equal(default, d5[4]);
            Assert.Equal(default, d5[5]);
        }

        [Fact]
        public void ReplaceTest()
        {
            var d0 = new PersistentDictionary<int, int>();
            var d1 = d0.Add(3, 3);
            var d2 = d1.Add(4, 4);
            var d3 = d2.Replace(3, 5);
            var d4 = d2.Replace(4, 6);

            Assert.Equal(3, d1[3]);
            Assert.Equal(default, d1[4]);

            Assert.Equal(3, d2[3]);
            Assert.Equal(4, d2[4]);

            Assert.Equal(5, d3[3]);
            Assert.Equal(4, d3[4]);

            Assert.Equal(3, d4[3]);
            Assert.Equal(6, d4[4]);

            Assert.Throws<ArgumentException>(() => d1.Replace(2, 8));
        }

        [Fact]
        public void UndoTest()
        {
            var d0 = new PersistentDictionary<int, int>();
            var d1 = d0.Add(3, 3);
            var d2 = d1.Add(4, 4);
            var d3 = d2.Replace(3, 5);
            var d4 = d3.Remove(4);
            var d5 = d3.Clear();

            var d6 = d0.Undo();
            var d7 = d2.Undo();
            var d8 = d3.Undo();
            var d9 = d4.Undo();
            var da = d5.Undo();

            Assert.Equal(d0, d6);

            Assert.Equal(d1[3], d7[3]);
            Assert.Equal(d1[4], d7[4]);

            Assert.Equal(d2[3], d8[3]);
            Assert.Equal(d2[4], d8[4]);

            Assert.Equal(d3[3], d9[3]);
            Assert.Equal(d3[4], d9[4]);

            Assert.Equal(d3[3], da[3]);
            Assert.Equal(d3[4], da[4]);
        }

        [Fact]
        public void RedoTest()
        {
            var d0 = new PersistentDictionary<int, int>();
            var d1 = d0.Add(3, 3);
            var d2 = d1.Add(4, 4);
            var d3 = d2.Replace(3, 5);
            var d4 = d3.Remove(4);
            var d5 = d3.Clear();

            var d6 = d5.Redo();
            var d7 = d0.Redo();
            var d8 = d1.Redo();
            var d9 = d2.Redo();
            var da = d3.Redo();

            Assert.Equal(d5, d6);

            Assert.Equal(d1[3], d7[3]);
            Assert.Equal(d1[4], d7[4]);

            Assert.Equal(d2[3], d8[3]);
            Assert.Equal(d2[4], d8[4]);

            Assert.Equal(d3[3], d9[3]);
            Assert.Equal(d3[4], d9[4]);

            Assert.Equal(d4[3], da[3]);
            Assert.Equal(d4[4], da[4]);
        }

        [Fact]
        public void ToPersistentArrayTest()
        {
            var d0 = new PersistentDictionary<int, int>();
            var d1 = d0.Add(3, 3);
            var d2 = d1.Add(4, 4);
            var d3 = d2.Add(5, 5);
            var d4 = d3.Add(6, 6);

            var arr = d4.ToPersistentArray();

            Assert.Equal(3, arr[0]);
            Assert.Equal(4, arr[1]);
            Assert.Equal(5, arr[2]);
            Assert.Equal(6, arr[3]);
        }
    }
}