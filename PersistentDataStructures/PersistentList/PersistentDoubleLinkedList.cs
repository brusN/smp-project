using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PersistentDataStructures.Persistency;

namespace PersistentDataStructures.PersistentList
{
    public class PersistentDoubleLinkedList<T> :
        BasePersistentCollection<PersistentDoubleLinkedList<T>.DoubleLinkedContent>,
        IEnumerable<T>, IUndoRedo<PersistentDoubleLinkedList<T>>
    {
        public PersistentDoubleLinkedList()
        {
            var head = new PersistentNode<DoubleLinkedData<T>>(modificationCount - 1,
                new DoubleLinkedData<T>(null, null, new PersistentNode<T>(-1, default)));
            var tail = new PersistentNode<DoubleLinkedData<T>>(modificationCount - 1,
                new DoubleLinkedData<T>(null, null, new PersistentNode<T>(-1, default)));
            head.Update(modificationCount,
                new DoubleLinkedData<T>(tail, null, head.GetValue(modificationCount - 1).value,
                    head.GetValue(modificationCount - 1).id));
            tail.Update(modificationCount,
                new DoubleLinkedData<T>(null, head, tail.GetValue(modificationCount - 1).value,
                    tail.GetValue(modificationCount - 1).id));

            nodes = new PersistentContent<DoubleLinkedContent>(new DoubleLinkedContent(head, tail),
                new ModificationCount(modificationCount));
        }

        private PersistentDoubleLinkedList(PersistentContent<DoubleLinkedContent> nodes, int count,
            int modificationCount) :
            base(nodes, count, modificationCount)
        {
        }

        internal PersistentDoubleLinkedList(PersistentContent<DoubleLinkedContent> nodes, int count,
            int modificationCount, int start) :
            base(nodes, count, modificationCount, start)
        {
        }

        public T this[int num]
        {
            get
            {
                var node = FindNode(num);
                return node == nodes.content.pseudoTail ? default :
                    node.GetValue(modificationCount).value == null ? default :
                    node.GetValue(modificationCount).value.GetValue(modificationCount);
            }
        }

        public bool isEmpty => count == 0;

        public IEnumerator<T> GetEnumerator()
        {
            return ToList(modificationCount).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public PersistentDoubleLinkedList<T> Undo()
        {
            return modificationCount == startModificationCount
                ? this
                : new PersistentDoubleLinkedList<T>(nodes, RecalculateCount(modificationCount - 1),
                    modificationCount - 1);
        }

        public PersistentDoubleLinkedList<T> Redo()
        {
            return modificationCount == nodes.maxModification
                ? this
                : new PersistentDoubleLinkedList<T>(nodes, RecalculateCount(modificationCount + 1),
                    modificationCount + 1);
        }

        protected override PersistentContent<DoubleLinkedContent> ReassembleNodes()
        {
            var allModifications = new List<KeyValuePair<Guid, KeyValuePair<int, DoubleLinkedData<T>>>>();
            var nodeModificationCount = new Dictionary<Guid, int>();
            var current = nodes.content.pseudoHead;
            for (var i = count; i != -2; i--)
            {
                var neededModifications = current.modifications.ToList().Where(m => m.Key <= modificationCount)
                    .Select(m => new KeyValuePair<Guid, KeyValuePair<int, DoubleLinkedData<T>>>(m.Value.id, m))
                    .OrderBy(m => m.Value.Key).ToList();
                allModifications.AddRange(neededModifications);
                nodeModificationCount.Add(current.GetValue(modificationCount).id, neededModifications.Count);
                current = current.GetValue(modificationCount).next;
            }

            var orderedModifications = allModifications.GroupBy(m => m.Value.Key)
                .Select(g => g.OrderBy(m => nodeModificationCount[m.Key])).SelectMany(x => x).ToList();

            var newNodes = new Dictionary<Guid, PersistentNode<DoubleLinkedData<T>>>
            {
                {
                    orderedModifications.First().Key, new PersistentNode<DoubleLinkedData<T>>(-1,
                        new DoubleLinkedData<T>(null, null, new PersistentNode<T>(-1, default),
                            orderedModifications.First().Key))
                }
            };
            orderedModifications.RemoveAt(0);
            newNodes.Add(orderedModifications.First().Key,
                new PersistentNode<DoubleLinkedData<T>>(-1,
                    new DoubleLinkedData<T>(null, null, new PersistentNode<T>(-1, default),
                        orderedModifications.First().Key)));
            orderedModifications.RemoveAt(0);

            foreach (var (nodeKey, (step, nodeValue)) in orderedModifications)
                if (newNodes.TryGetValue(nodeKey, out var node))
                {
                    node.Update(step, new DoubleLinkedData<T>(
                        nodeValue.next == null ? null : newNodes[nodeValue.next.GetValue(step).id],
                        nodeValue.previous == null ? null : newNodes[nodeValue.previous.GetValue(step).id],
                        node.GetValue(step - 1).value.Update(step, nodeValue.value.GetValue(step)), nodeKey));
                }
                else
                {
                    var newNode = new PersistentNode<DoubleLinkedData<T>>(step,
                        new DoubleLinkedData<T>(newNodes[nodeValue.next.GetValue(step).id],
                            newNodes[nodeValue.previous.GetValue(step).id],
                            new PersistentNode<T>(step, nodeValue.value.GetValue(step)), nodeKey));
                    newNodes.Add(nodeKey, newNode);
                }

            var newHead = newNodes[nodes.content.pseudoHead.GetValue(modificationCount).id];
            var newTail = newNodes[nodes.content.pseudoTail.GetValue(modificationCount).id];
            return new PersistentContent<DoubleLinkedContent>(new DoubleLinkedContent(newHead, newTail),
                new ModificationCount(modificationCount));
        }

        public PersistentDoubleLinkedList<T> AddLast(T value)
        {
            if (nodes.maxModification > modificationCount)
            {
                var newContent = ReassembleNodes();
                return _AddLast(newContent, value);
            }

            return _AddLast(nodes, value);
        }

        private PersistentDoubleLinkedList<T> _AddLast(PersistentContent<DoubleLinkedContent> content, T value)
        {
            var oldTail = content.content.pseudoTail.GetValue(modificationCount);
            var oldNextToTail = oldTail.previous;
            var oldNextToTailValue = oldNextToTail.GetValue(modificationCount);
            var newTail = new PersistentNode<DoubleLinkedData<T>>(modificationCount + 1,
                new DoubleLinkedData<T>(content.content.pseudoTail, oldTail.previous,
                    new PersistentNode<T>(modificationCount + 1, value)));
            content.Update(m =>
            {
                oldNextToTail.Update(modificationCount + 1,
                    new DoubleLinkedData<T>(newTail, oldNextToTailValue.previous, oldNextToTailValue.value,
                        oldNextToTailValue.id));
                m.pseudoTail.Update(modificationCount + 1,
                    new DoubleLinkedData<T>(null, newTail, oldTail.value, oldTail.id));
            });
            return new PersistentDoubleLinkedList<T>(content, count + 1, modificationCount + 1);
        }

        public PersistentDoubleLinkedList<T> AddFirst(T value)
        {
            if (nodes.maxModification > modificationCount)
            {
                var newContent = ReassembleNodes();
                return _AddFirst(newContent, value);
            }

            return _AddFirst(nodes, value);
        }

        private PersistentDoubleLinkedList<T> _AddFirst(PersistentContent<DoubleLinkedContent> content, T value)
        {
            var oldHead = content.content.pseudoHead.GetValue(modificationCount);
            var oldNextToHead = oldHead.next;
            var oldNextToHeadValue = oldNextToHead.GetValue(modificationCount);
            var newHead = new PersistentNode<DoubleLinkedData<T>>(modificationCount + 1,
                new DoubleLinkedData<T>(oldHead.next, content.content.pseudoHead,
                    new PersistentNode<T>(modificationCount + 1, value)));
            content.Update(m =>
            {
                oldNextToHead.Update(modificationCount + 1,
                    new DoubleLinkedData<T>(oldNextToHeadValue.next, newHead, oldNextToHeadValue.value,
                        oldNextToHeadValue.id));
                m.pseudoHead.Update(modificationCount + 1,
                    new DoubleLinkedData<T>(newHead, null, oldHead.value, oldHead.id));
            });
            return new PersistentDoubleLinkedList<T>(content, count + 1, modificationCount + 1);
        }

        public PersistentDoubleLinkedList<T> RemoveFirst()
        {
            if (count == 0) return this;

            if (nodes.maxModification <= modificationCount) return _RemoveFirst(nodes);

            var newContent = ReassembleNodes();
            return _RemoveFirst(newContent);
        }

        private PersistentDoubleLinkedList<T> _RemoveFirst(PersistentContent<DoubleLinkedContent> content)
        {
            var oldHead = content.content.pseudoHead;
            var oldHeadValue = oldHead.GetValue(modificationCount);
            var oldNextToNextToHead = oldHeadValue.next.GetValue(modificationCount).next;
            var oldNextToNextToHeadValue = oldNextToNextToHead.GetValue(modificationCount);
            content.Update(_ =>
            {
                oldNextToNextToHead.Update(modificationCount + 1,
                    new DoubleLinkedData<T>(oldNextToNextToHeadValue.next, oldHead, oldNextToNextToHeadValue.value,
                        oldNextToNextToHeadValue.id));
                oldHead.Update(modificationCount + 1,
                    new DoubleLinkedData<T>(oldNextToNextToHead, null, oldHeadValue.value, oldHeadValue.id));
            });
            return new PersistentDoubleLinkedList<T>(content, count - 1, modificationCount + 1);
        }

        public PersistentDoubleLinkedList<T> RemoveLast()
        {
            if (count == 0) return this;

            if (nodes.maxModification > modificationCount)
            {
                var newContent = ReassembleNodes();
                return _RemoveLast(newContent);
            }

            return _RemoveLast(nodes);
        }

        private PersistentDoubleLinkedList<T> _RemoveLast(PersistentContent<DoubleLinkedContent> content)
        {
            var oldTail = content.content.pseudoTail;
            var oldTailValue = oldTail.GetValue(modificationCount);
            var oldNextToNextToTail = oldTailValue.previous.GetValue(modificationCount).previous;
            var oldNextToNextToTailValue = oldNextToNextToTail.GetValue(modificationCount);
            content.Update(_ =>
            {
                oldNextToNextToTail.Update(modificationCount + 1,
                    new DoubleLinkedData<T>(oldTail, oldNextToNextToTailValue.previous, oldNextToNextToTailValue.value,
                        oldNextToNextToTailValue.id));
                oldTail.Update(modificationCount,
                    new DoubleLinkedData<T>(null, oldNextToNextToTail, oldTailValue.value, oldTailValue.id));
            });
            return new PersistentDoubleLinkedList<T>(content, count - 1, modificationCount + 1);
        }

        public PersistentDoubleLinkedList<T> Clear()
        {
            if (count == 0) return this;

            if (nodes.maxModification > modificationCount)
            {
                var newContent = ReassembleNodes();
                newContent.Update(m =>
                {
                    m.pseudoHead.Update(modificationCount + 1,
                        new DoubleLinkedData<T>(m.pseudoTail, null, m.pseudoHead.GetValue(modificationCount).value,
                            m.pseudoHead.GetValue(modificationCount).id));
                    m.pseudoTail.Update(modificationCount + 1,
                        new DoubleLinkedData<T>(null, m.pseudoHead, m.pseudoTail.GetValue(modificationCount).value,
                            m.pseudoTail.GetValue(modificationCount).id));
                });
                return new PersistentDoubleLinkedList<T>(newContent, 0, modificationCount + 1);
            }

            nodes.Update(m =>
            {
                m.pseudoHead.Update(modificationCount + 1,
                    new DoubleLinkedData<T>(m.pseudoTail, null, m.pseudoHead.GetValue(modificationCount).value,
                        m.pseudoHead.GetValue(modificationCount).id));
                m.pseudoTail.Update(modificationCount + 1,
                    new DoubleLinkedData<T>(null, m.pseudoHead, m.pseudoTail.GetValue(modificationCount).value,
                        m.pseudoTail.GetValue(modificationCount).id));
            });
            return new PersistentDoubleLinkedList<T>(nodes, 0, modificationCount + 1);
        }

        private PersistentNode<DoubleLinkedData<T>> FindNode(int num)
        {
            var current = nodes.content.pseudoHead.GetValue(modificationCount).next;
            for (var i = num; i != 0; i--) current = current.GetValue(modificationCount).next;

            return current;
        }

        public PersistentDoubleLinkedList<T> Replace(int num, T value)
        {
            if (num > count) return this;
            if (nodes.maxModification > modificationCount)
            {
                var newContent = ReassembleNodes();
                return _Replace(newContent, num, value);
            }

            return _Replace(nodes, num, value);
        }

        private PersistentDoubleLinkedList<T> _Replace(PersistentContent<DoubleLinkedContent> content, int num, T value)
        {
            var node = FindNode(num);
            var nodeValue = node.GetValue(modificationCount);
            content.Update(_ =>
                node.Update(modificationCount + 1,
                    new DoubleLinkedData<T>(nodeValue.next, nodeValue.previous,
                        nodeValue.value.Update(modificationCount + 1, value), nodeValue.id)));
            return new PersistentDoubleLinkedList<T>(content, count, modificationCount + 1);
        }

        public bool Contains(T item)
        {
            var current = nodes.content.pseudoHead.GetValue(modificationCount).next;
            for (var i = count; i != 0; i--)
            {
                if (current.GetValue(modificationCount).value.Equals(item)) return true;
                current = current.GetValue(modificationCount).next;
            }

            return false;
        }

        private List<T> ToList(int modificationStep)
        {
            var newList = new List<T>();
            var current = nodes.content.pseudoHead.GetValue(modificationStep).next;
            for (var i = count; i != 0; i--)
            {
                newList.Add(current.GetValue(modificationStep).value.GetValue(modificationStep));
                current = current.GetValue(modificationStep).next;
            }

            return newList;
        }

        protected override int RecalculateCount(int modificationStep)
        {
            return ToList(modificationStep).Count;
        }

        public PersistentArray<T> ToPersistentArray()
        {
            var content = new PersistentContent<List<PersistentNode<T>>>(
                new List<PersistentNode<T>>(), nodes.maxModification);

            var current = nodes.content.pseudoHead.GetValue(modificationCount).next;
            for (var i = count; i != 0; i--)
            {
                var currentValue = current.GetValue(modificationCount);
                content.content.Add(currentValue.value);
                current = currentValue.next;
            }

            return new PersistentArray<T>(content, count, modificationCount, modificationCount);
        }

        public PersistentDictionary<int, T> ToPersistentDictionary()
        {
            return ToPersistentArray().ToPersistentDictionary();
        }

        public class DoubleLinkedContent
        {
            public readonly PersistentNode<DoubleLinkedData<T>> pseudoHead, pseudoTail;

            public DoubleLinkedContent(PersistentNode<DoubleLinkedData<T>> pseudoHead,
                PersistentNode<DoubleLinkedData<T>> pseudoTail)
            {
                this.pseudoHead = pseudoHead;
                this.pseudoTail = pseudoTail;
            }
        }
    }
}