using PersistentDataStructures.BinarySearch;
using PersistentDataStructures.Shared;

namespace PersistentDataStructures.Persistency
{
    public class PersistentNode<TV>
    {
        public PersistentNode(int creationStep, TV initialValue)
        {
            Update(creationStep, initialValue);
        }

        public BinaryTree<int, TV> modifications { get; } = new();

        public TV GetValue(int accessStep)
        {
            return modifications.FindNearestLess(accessStep);
        }

        public PersistentNode<TV> Update(int accessStep, TV value)
        {
            modifications[accessStep] = value;
            return this;
        }
    }
}