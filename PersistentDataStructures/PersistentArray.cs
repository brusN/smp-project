using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PersistentDataStructures.BinarySearch;
using PersistentDataStructures.Persistency;
using PersistentDataStructures.PersistentList;

namespace PersistentDataStructures
{
    public class PersistentArray<T> : BasePersistentCollection<List<PersistentNode<T>>>,
        IEnumerable<T>, IUndoRedo<PersistentArray<T>>
    {
        public PersistentArray()
        {
            nodes = new PersistentContent<List<PersistentNode<T>>>(new List<PersistentNode<T>>(),
                new ModificationCount(modificationCount));
        }

        private PersistentArray(PersistentContent<List<PersistentNode<T>>> nodes, int count, int modificationCount) :
            base(nodes, count, modificationCount)
        {
        }

        internal PersistentArray(PersistentContent<List<PersistentNode<T>>> nodes, int count, int modificationCount,
            int start) :
            base(nodes, count, modificationCount, start)
        {
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= count) throw new ArgumentOutOfRangeException(nameof(index));

                return nodes.content[index].GetValue(modificationCount);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return nodes.content
                .Where(n =>
                    n.modifications.Any(m => m.Key <= modificationCount))
                .Select(n =>
                    n.GetValue(modificationCount))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public PersistentArray<T> Undo()
        {
            return modificationCount == startModificationCount
                ? this
                : new PersistentArray<T>(nodes,
                    RecalculateCount(modificationCount - 1), modificationCount - 1);
        }

        public PersistentArray<T> Redo()
        {
            return modificationCount == nodes.maxModification
                ? this
                : new PersistentArray<T>(nodes,
                    RecalculateCount(modificationCount + 1), modificationCount + 1);
        }

        protected override PersistentContent<List<PersistentNode<T>>> ReassembleNodes()
        {
            var newContent = new PersistentContent<List<PersistentNode<T>>>(new List<PersistentNode<T>>(),
                new ModificationCount(modificationCount));
            var allModifications = new List<KeyValuePair<int, KeyValuePair<int, T>>>();
            for (var i = 0; i < nodes.content.Count; i++)
            {
                var node = nodes.content[i];
                var neededModifications = from m in node.modifications.ToList()
                    where m.Key <= modificationCount
                    orderby m.Key
                    select new KeyValuePair<int, KeyValuePair<int, T>>(i, m);
                allModifications.AddRange(neededModifications);
            }

            allModifications.ForEach(m =>
            {
                if (m.Key >= newContent.content.Count)
                    newContent.Update(c =>
                        c.Add(new PersistentNode<T>(m.Value.Key, m.Value.Value)));
                else
                    newContent.Update(c =>
                        c[m.Key].Update(m.Value.Key, m.Value.Value));
            });

            return newContent;
        }

        private static void _Add(PersistentContent<List<PersistentNode<T>>> content, int modificationCount, T value)
        {
            content.Update(c =>
                c.Add(new PersistentNode<T>(modificationCount + 1, value)));
        }

        private static void _Insert(PersistentContent<List<PersistentNode<T>>> content, int modificationCount,
            int index, T value)
        {
            content.Update(c =>
            {
                c.Add(new PersistentNode<T>(modificationCount + 1, c[^1].GetValue(modificationCount)));
                c[index].Update(modificationCount + 1, value);
                for (var i = index + 1; i < c.Count; i++)
                    c[i].Update(modificationCount + 1, c[i - 1].GetValue(modificationCount));
            });
        }

        private static void _Replace(PersistentContent<List<PersistentNode<T>>> content, int modificationCount,
            int index, T value)
        {
            content.Update(c => { c[index].Update(modificationCount + 1, value); });
        }

        private static void _Remove(PersistentContent<List<PersistentNode<T>>> content, int modificationCount,
            int index)
        {
            content.Update(c =>
            {
                for (var i = index; i < c.Count - 1; i++)
                    c[i].Update(modificationCount + 1, c[i + 1].GetValue(modificationCount));

                c[^1].Update(modificationCount + 1, default);
            });
        }

        private static void _Clear(PersistentContent<List<PersistentNode<T>>> content, int modificationCount)
        {
            content.Update(c => { c.ForEach(n => n.Update(modificationCount + 1, default)); });
        }

        public PersistentArray<T> Add(T value)
        {
            if (nodes.maxModification > modificationCount)
            {
                var res = ReassembleNodes();
                _Add(res, modificationCount, value);

                return new PersistentArray<T>(res, count + 1, modificationCount + 1);
            }

            _Add(nodes, modificationCount, value);
            return new PersistentArray<T>(nodes, count + 1, modificationCount + 1);
        }

        public PersistentArray<T> Insert(int index, T value)
        {
            if (index < 0 || index > count) throw new ArgumentOutOfRangeException(nameof(index));

            if (index == count) return Add(value);

            if (nodes.maxModification > modificationCount)
            {
                var res = ReassembleNodes();
                _Insert(res, modificationCount, index, value);

                return new PersistentArray<T>(res, count + 1, modificationCount + 1);
            }

            _Insert(nodes, modificationCount, index, value);
            return new PersistentArray<T>(nodes, count + 1, modificationCount + 1);
        }

        public PersistentArray<T> Replace(int index, T value)
        {
            if (index < 0 || index > count) throw new ArgumentOutOfRangeException(nameof(index));

            if (nodes.maxModification > modificationCount)
            {
                var res = ReassembleNodes();
                _Replace(res, modificationCount, index, value);

                return new PersistentArray<T>(res, count, modificationCount + 1);
            }

            _Replace(nodes, modificationCount, index, value);
            return new PersistentArray<T>(nodes, count, modificationCount + 1);
        }

        public PersistentArray<T> Remove(int index)
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException(nameof(index));

            if (nodes.maxModification > modificationCount)
            {
                var res = ReassembleNodes();
                _Remove(res, modificationCount, index);

                return new PersistentArray<T>(res, count - 1, modificationCount + 1);
            }

            _Remove(nodes, modificationCount, index);
            return new PersistentArray<T>(nodes, count - 1, modificationCount + 1);
        }

        public PersistentArray<T> Clear()
        {
            if (nodes.maxModification > modificationCount)
            {
                var res = ReassembleNodes();
                _Clear(res, modificationCount);

                return new PersistentArray<T>(res, 0, modificationCount + 1);
            }

            _Clear(nodes, modificationCount);
            return new PersistentArray<T>(nodes, 0, modificationCount + 1);
        }

        public bool ContainsValue(T value)
        {
            return this.Contains(value);
        }

        protected override int RecalculateCount(int modificationStep)
        {
            return nodes.content
                .Count(n => n.modifications.Any(m => m.Key <= modificationStep));
        }

        public PersistentDictionary<int, T> ToPersistentDictionary()
        {
            var content = new PersistentContent<BinaryTree<int, PersistentNode<T>>>(
                new BinaryTree<int, PersistentNode<T>>(), nodes.maxModification);

            for (var i = 0; i < nodes.content.Count; i++) content.content.Insert(i, nodes.content[i]);

            return new PersistentDictionary<int, T>(content, count, modificationCount, modificationCount);
        }

        public PersistentDoubleLinkedList<T> ToPersistentDoubleLinkedList()
        {
            var head = new PersistentNode<DoubleLinkedData<T>>(
                modificationCount - 1,
                new DoubleLinkedData<T>(null, null,
                    new PersistentNode<T>(-1, default)));
            var tail = new PersistentNode<DoubleLinkedData<T>>(
                modificationCount - 1,
                new DoubleLinkedData<T>(null, null,
                    new PersistentNode<T>(-1, default)));
            head.Update(modificationCount, new DoubleLinkedData<T>(tail, null,
                head.GetValue(modificationCount - 1).value, head.GetValue(modificationCount - 1).id));
            tail.Update(modificationCount, new DoubleLinkedData<T>(null, head,
                tail.GetValue(modificationCount - 1).value, tail.GetValue(modificationCount - 1).id));

            var content = new PersistentContent<PersistentDoubleLinkedList<T>.DoubleLinkedContent>(
                new PersistentDoubleLinkedList<T>.DoubleLinkedContent(head, tail), nodes.maxModification);

            foreach (var t in nodes.content)
            {
                var tailValue = content.content.pseudoTail.GetValue(modificationCount);
                var prevToTail = tailValue.previous;
                var prevToTailValue = content.content.pseudoTail.GetValue(modificationCount).previous
                    .GetValue(modificationCount);
                var dnode = new DoubleLinkedData<T>(content.content.pseudoTail, prevToTail, t);
                var node = new PersistentNode<DoubleLinkedData<T>>(modificationCount - 1, dnode);
                prevToTail.Update(modificationCount,
                    new DoubleLinkedData<T>(node, prevToTailValue.previous, prevToTailValue.value, prevToTailValue.id));
                content.content.pseudoTail.Update(modificationCount,
                    new DoubleLinkedData<T>(tailValue.next, node, tailValue.value, tailValue.id));
            }

            return new PersistentDoubleLinkedList<T>(content, count, modificationCount, modificationCount);
        }
    }
}