using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PersistentDataStructures.BinarySearch;
using PersistentDataStructures.Persistency;
using PersistentDataStructures.PersistentList;

namespace PersistentDataStructures
{
    public class PersistentDictionary<TK, TV> : BasePersistentCollection<BinaryTree<TK, PersistentNode<TV>>>,
        IEnumerable<KeyValuePair<TK, TV>>, IUndoRedo<PersistentDictionary<TK, TV>>
    {
        public PersistentDictionary()
        {
            nodes = new PersistentContent<BinaryTree<TK, PersistentNode<TV>>>(
                new BinaryTree<TK, PersistentNode<TV>>(),
                new ModificationCount(modificationCount)
            );
        }

        internal PersistentDictionary(PersistentContent<BinaryTree<TK, PersistentNode<TV>>> nodes, int count,
            int modificationCount) :
            base(nodes, count, modificationCount)
        {
        }

        internal PersistentDictionary(PersistentContent<BinaryTree<TK, PersistentNode<TV>>> nodes, int count,
            int modificationCount, int start) :
            base(nodes, count, modificationCount, start)
        {
        }

        public TV this[TK key]
        {
            get
            {
                var node = nodes.content.Get(key);
                return node == null ? default : node.GetValue(modificationCount);
            }
        }

        public ICollection<TK> keys =>
            nodes.content.ToList()
                .Where(k =>
                    k.Value.modifications.ToList().Any(m => m.Key <= modificationCount))
                .Select(k => k.Key)
                .ToList();

        public ICollection<TV> values =>
            nodes.content.ToList()
                .Where(k =>
                    k.Value.modifications.ToList().Any(m => m.Key <= modificationCount))
                .Select(k => k.Value.GetValue(modificationCount))
                .ToList();

        public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator()
        {
            return nodes.content
                .Where(k =>
                    k.Value.modifications.ToList().Any(m => m.Key <= modificationCount))
                .Select(k =>
                    new KeyValuePair<TK, TV>(k.Key, k.Value.GetValue(modificationCount)))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public PersistentDictionary<TK, TV> Undo()
        {
            return modificationCount == startModificationCount
                ? this
                : new PersistentDictionary<TK, TV>(nodes,
                    RecalculateCount(modificationCount - 1), modificationCount - 1);
        }

        public PersistentDictionary<TK, TV> Redo()
        {
            return modificationCount == nodes.maxModification
                ? this
                : new PersistentDictionary<TK, TV>(nodes,
                    RecalculateCount(modificationCount + 1), modificationCount + 1);
        }

        protected override PersistentContent<BinaryTree<TK, PersistentNode<TV>>> ReassembleNodes()
        {
            var newContent = new PersistentContent<BinaryTree<TK, PersistentNode<TV>>>(
                new BinaryTree<TK, PersistentNode<TV>>(),
                new ModificationCount(modificationCount));
            var allModifications = new List<KeyValuePair<TK, KeyValuePair<int, TV>>>();
            foreach (var (nodeKey, persistentNode) in nodes.content)
            {
                var neededModifications = persistentNode.modifications.ToList().Where(m => m.Key <= modificationCount)
                    .Select(m => new KeyValuePair<TK, KeyValuePair<int, TV>>(nodeKey, m)).OrderBy(m => m.Value.Key)
                    .ToList();

                allModifications.AddRange(neededModifications);
            }

            foreach (var (nodeKey, (step, nodeValue)) in allModifications)
                newContent.Update(c =>
                {
                    var node = c.Get(nodeKey);
                    if (node == null)
                        c.Insert(nodeKey, new PersistentNode<TV>(step, nodeValue));
                    else
                        node.Update(step, nodeValue);
                });

            return newContent;
        }

        private static void _Add(PersistentContent<BinaryTree<TK, PersistentNode<TV>>> nodes, int modificationCount,
            TK key, TV value)
        {
            nodes.Update(c =>
                c.Insert(key, new PersistentNode<TV>(modificationCount + 1, value)));
        }

        private static void _Remove(PersistentContent<BinaryTree<TK, PersistentNode<TV>>> nodes, int modificationCount,
            TK key)
        {
            nodes.Update(c =>
                c.Get(key).Update(modificationCount + 1, default));
        }

        private static void _Clear(PersistentContent<BinaryTree<TK, PersistentNode<TV>>> nodes, int modificationCount)
        {
            nodes.Update(c =>
            {
                foreach (var keyValuePair in c.ToList()) keyValuePair.Value.Update(modificationCount + 1, default);
            });
        }

        private static void _Replace(PersistentContent<BinaryTree<TK, PersistentNode<TV>>> nodes, int modificationCount,
            TK key, TV value)
        {
            nodes.Update(c =>
                c.Get(key).Update(modificationCount + 1, value));
        }

        public PersistentDictionary<TK, TV> Add(TK key, TV value)
        {
            var tryNode = nodes.content.Get(key);
            if (tryNode != null && tryNode.modifications.ToList().Any(m => m.Key <= modificationCount))
                throw new ArgumentException("Such a key is already presented in the dictionary");

            if (nodes.maxModification > modificationCount)
            {
                var res = ReassembleNodes();
                _Add(res, modificationCount, key, value);

                return new PersistentDictionary<TK, TV>(res, count + 1, modificationCount + 1);
            }

            _Add(nodes, modificationCount, key, value);

            return new PersistentDictionary<TK, TV>(nodes, count + 1, modificationCount + 1);
        }

        public PersistentDictionary<TK, TV> Remove(TK key)
        {
            var tryNode = nodes.content.Get(key);
            if (tryNode == null || tryNode.modifications.ToList().All(m => m.Key > modificationCount)) return this;

            if (nodes.maxModification > modificationCount)
            {
                var res = ReassembleNodes();
                _Remove(res, modificationCount, key);

                return new PersistentDictionary<TK, TV>(res, count - 1, modificationCount + 1);
            }

            _Remove(nodes, modificationCount, key);

            return new PersistentDictionary<TK, TV>(nodes, count - 1, modificationCount + 1);
        }

        public PersistentDictionary<TK, TV> Clear()
        {
            if (nodes.maxModification > modificationCount)
            {
                var res = ReassembleNodes();
                _Clear(res, modificationCount);

                return new PersistentDictionary<TK, TV>(res, 0, modificationCount + 1);
            }

            _Clear(nodes, modificationCount);

            return new PersistentDictionary<TK, TV>(nodes, 0, modificationCount + 1);
        }

        public PersistentDictionary<TK, TV> Replace(TK key, TV value)
        {
            var tryNode = nodes.content.Get(key);
            if (tryNode == null || tryNode.modifications.ToList().All(m => m.Key > modificationCount))
                throw new ArgumentException("Such a key is not presented in the dictionary");

            if (nodes.maxModification > modificationCount)
            {
                var res = ReassembleNodes();
                _Replace(res, modificationCount, key, value);

                return new PersistentDictionary<TK, TV>(res, count, modificationCount + 1);
            }

            _Replace(nodes, modificationCount, key, value);

            return new PersistentDictionary<TK, TV>(nodes, count, modificationCount + 1);
        }

        public bool ContainsKey(TK key)
        {
            return nodes.content.Contains(key);
        }

        protected override int RecalculateCount(int modificationStep)
        {
            return nodes.content.Count(k =>
                k.Value.modifications.ToList().Any(m => m.Key <= modificationStep));
        }

        public PersistentArray<TV> ToPersistentArray()
        {
            var content = new PersistentContent<List<PersistentNode<TV>>>(
                new List<PersistentNode<TV>>(), nodes.maxModification);

            var persistentNodes = nodes.content.ToList().Select(k => k.Value);
            content.content.AddRange(persistentNodes);

            return new PersistentArray<TV>(content, count, modificationCount, modificationCount);
        }

        public PersistentDoubleLinkedList<TV> ToPersistentDoubleLinkedList()
        {
            return ToPersistentArray().ToPersistentDoubleLinkedList();
        }
    }
}