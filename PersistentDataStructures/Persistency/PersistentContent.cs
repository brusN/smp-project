using System;

namespace PersistentDataStructures.Persistency
{
    public class ModificationCount
    {
        public int value;

        public ModificationCount(int value)
        {
            this.value = value;
        }

        public static implicit operator int(ModificationCount modificationCount)
        {
            return modificationCount.value;
        }
    }

    public class PersistentContent<T>
    {
        public PersistentContent(T content, ModificationCount step)
        {
            this.content = content;
            maxModification = step;
        }

        public T content { get; }
        public ModificationCount maxModification { get; }

        public void Update(Action<T> contentUpdater)
        {
            contentUpdater(content);
            maxModification.value++;
        }
    }
}