namespace PersistentDataStructures.Persistency
{
    public interface IUndoRedo<T>
    {
        public T Undo();
        public T Redo();
    }
}