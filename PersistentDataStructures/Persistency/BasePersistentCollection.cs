namespace PersistentDataStructures.Persistency
{
    public abstract class BasePersistentCollection<T>
    {
        protected readonly int modificationCount, startModificationCount;
        protected PersistentContent<T> nodes;

        protected BasePersistentCollection()
        {
        }

        protected BasePersistentCollection(PersistentContent<T> nodes, int count, int modificationCount,
            int startModificationCount = 0)
        {
            this.nodes = nodes;
            this.modificationCount = modificationCount;
            this.startModificationCount = startModificationCount;
            this.count = count;
        }

        public int count { get; }

        protected abstract int RecalculateCount(int modificationStep);
        protected abstract PersistentContent<T> ReassembleNodes();
    }
}