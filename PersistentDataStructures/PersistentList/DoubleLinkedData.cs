using System;
using PersistentDataStructures.Persistency;

namespace PersistentDataStructures.PersistentList
{
    public class DoubleLinkedData<T>
    {
        public DoubleLinkedData(PersistentNode<DoubleLinkedData<T>> next, PersistentNode<DoubleLinkedData<T>> previous,
            PersistentNode<T> value)
        {
            this.next = next;
            this.previous = previous;
            this.value = value;
            id = Guid.NewGuid();
        }

        public DoubleLinkedData(PersistentNode<DoubleLinkedData<T>> next, PersistentNode<DoubleLinkedData<T>> previous,
            PersistentNode<T> value, Guid id)
        {
            this.next = next;
            this.previous = previous;
            this.value = value;
            this.id = id;
        }

        public PersistentNode<DoubleLinkedData<T>> previous { get; }
        public PersistentNode<DoubleLinkedData<T>> next { get; }
        public Guid id { get; }
        public PersistentNode<T> value { get; }
    }
}