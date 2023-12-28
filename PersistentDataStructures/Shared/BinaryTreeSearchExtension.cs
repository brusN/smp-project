using PersistentDataStructures.BinarySearch;

namespace PersistentDataStructures.Shared
{
    public static class BinaryTreeSearchExtension
    {
        public static TV FindNearestLess<TK, TV>(this BinaryTree<TK, TV> tree, TK key)
        {
            var hashedKey = key.GetHashCode();
            var node = tree.root;
            BinaryTree<TK, TV>.Node optimalNode = null;
            do
            {
                if (node == null) break;

                if (node.hash <= hashedKey &&
                    (optimalNode == null || hashedKey - optimalNode.hash > hashedKey - node.hash))
                    optimalNode = node;

                node = node.hash > hashedKey ? node.left : node.right;
            } while (true);

            return optimalNode == null ? default : optimalNode.data;
        }
    }
}