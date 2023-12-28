using System;
using System.Collections;
using System.Collections.Generic;

namespace PersistentDataStructures.BinarySearch
{
    public class BinaryTree<TK, TV> : IEnumerable<KeyValuePair<TK, TV>>
    {
        public enum Color
        {
            Red,
            Black
        }

        public Node root;

        public TV this[TK key]
        {
            get => Find(key).data;
            set => Insert(key, value);
        }

        public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator()
        {
            return ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(TK key)
        {
            return Find(key) != null;
        }

        /// <summary>
        ///     Left Rotate
        /// </summary>
        /// <param name="x"></param>
        /// <returns>void</returns>
        private void LeftRotate(Node x)
        {
            var y = x.right;
            x.right = y.left; //turn Y's left subtree into X's right subtree
            if (y.left != null) y.left.parent = x;

            if (y != null) y.parent = x.parent; //link X's parent to Y

            if (x.parent == null) root = y;

            if (x.parent != null && x == x.parent.left)
                x.parent.left = y;
            else if (x.parent != null) x.parent.right = y;

            y.left = x;
            x.parent = y;
        }

        /// <summary>
        ///     Rotate Right
        /// </summary>
        /// <param name="y"></param>
        /// <returns>void</returns>
        private void RightRotate(Node y)
        {
            var x = y.left;
            y.left = x.right;
            if (x.right != null) x.right.parent = y;

            if (x != null) x.parent = y.parent;

            if (y.parent == null) root = x;

            if (y.parent != null && y == y.parent.right) y.parent.right = x;

            if (y.parent != null && y == y.parent.left) y.parent.left = x;

            x.right = y;
            y.parent = x;
        }

        public TV Get(TK key)
        {
            var node = Find(key);
            return node == null ? default : node.data;
        }

        private Node Find(TK key)
        {
            var isFound = false;
            var temp = root;
            Node item = null;
            var hash = key.GetHashCode();
            while (!isFound)
            {
                if (temp == null) break;

                if (hash < temp.hash)
                    temp = temp.left;
                else if (hash > temp.hash) temp = temp.right;

                if (temp == null || hash != temp.hash) continue;

                isFound = true;
                item = temp;
            }

            return item;
        }

        public void Insert(TK key, TV item)
        {
            var node = Find(key);

            if (node != null)
            {
                node.data = item;
                return;
            }

            var newItem = new Node(key, item);
            if (root == null)
            {
                root = newItem;
                root.color = Color.Black;
                return;
            }

            Node y = null;
            var x = root;
            while (x != null)
            {
                y = x;
                x = newItem.hash < x.hash ? x.left : x.right;
            }

            newItem.parent = y;
            if (y == null)
                root = newItem;
            else if (newItem.hash < y.hash)
                y.left = newItem;
            else
                y.right = newItem;

            newItem.left = null;
            newItem.right = null;
            newItem.color = Color.Red;
            InsertFixUp(newItem);
        }

        private void InsertFixUp(Node item)
        {
            while (item != root && item.parent.color == Color.Red)
            {
                // red black tree rules violation
                if (item.parent == item.parent.parent.left)
                {
                    var y = item.parent.parent.right;
                    //Case 1: uncle is red
                    if (y is { color: Color.Red })
                    {
                        item.parent.color = Color.Black;
                        y.color = Color.Black;
                        item.parent.parent.color = Color.Red;
                        item = item.parent.parent;
                    }
                    //Case 2: uncle is black
                    else
                    {
                        if (item == item.parent.right)
                        {
                            item = item.parent;
                            LeftRotate(item);
                        }

                        //Case 3: recolour & rotate
                        item.parent.color = Color.Black;
                        item.parent.parent.color = Color.Red;
                        RightRotate(item.parent.parent);
                    }
                }
                else
                {
                    var x = item.parent.parent.left;
                    if (x is { color: Color.Black })
                    {
                        item.parent.color = Color.Red;
                        x.color = Color.Red;
                        item.parent.parent.color = Color.Black;
                        item = item.parent.parent;
                    }
                    else
                    {
                        if (item == item.parent.left)
                        {
                            item = item.parent;
                            RightRotate(item);
                        }

                        item.parent.color = Color.Black;
                        item.parent.parent.color = Color.Red;
                        LeftRotate(item.parent.parent);
                    }
                }

                root.color = Color.Black;
            }
        }

        public void Delete(TK key)
        {
            var item = Find(key);
            Node x;
            Node y;

            if (item == null)
            {
                Console.WriteLine("Nothing to delete!");
                return;
            }

            if (item.left == null || item.right == null)
                y = item;
            else
                y = TreeSuccessor(item);

            x = y.left ?? y.right;

            if (x != null) x.parent = y;

            if (y.parent == null)
                root = x;
            else if (y == y.parent.left)
                y.parent.left = x;
            else
                y.parent.left = x;

            if (y != item) item.data = y.data;

            if (y.color == Color.Black) DeleteFixUp(x);
        }

        /// <summary>
        ///     Checks the tree for any violations after deletion and performs a fix
        /// </summary>
        /// <param name="x"></param>
        private void DeleteFixUp(Node x)
        {
            while (x != null && x != root && x.color == Color.Black)
                if (x == x.parent.left)
                {
                    var parentRight = x.parent.right;
                    if (parentRight.color == Color.Red)
                    {
                        parentRight.color = Color.Black;
                        x.parent.color = Color.Red;
                        LeftRotate(x.parent);
                        parentRight = x.parent.right;
                    }

                    if (parentRight.left.color == Color.Black && parentRight.right.color == Color.Black)
                    {
                        parentRight.color = Color.Red;
                        x = x.parent;
                    }
                    else if (parentRight.right.color == Color.Black)
                    {
                        parentRight.left.color = Color.Black;
                        parentRight.color = Color.Red;
                        RightRotate(parentRight);
                        parentRight = x.parent.right;
                    }

                    parentRight.color = x.parent.color;
                    x.parent.color = Color.Black;
                    parentRight.right.color = Color.Black;
                    LeftRotate(x.parent);
                    x = root;
                }
                else
                {
                    var parentLeft = x.parent.left;
                    if (parentLeft.color == Color.Red)
                    {
                        parentLeft.color = Color.Black;
                        x.parent.color = Color.Red;
                        RightRotate(x.parent);
                        parentLeft = x.parent.left;
                    }

                    if (parentLeft.right.color == Color.Black && parentLeft.left.color == Color.Black)
                    {
                        parentLeft.color = Color.Black;
                        x = x.parent;
                    }
                    else if (parentLeft.left.color == Color.Black)
                    {
                        parentLeft.right.color = Color.Black;
                        parentLeft.color = Color.Red;
                        LeftRotate(parentLeft);
                        parentLeft = x.parent.left;
                    }

                    parentLeft.color = x.parent.color;
                    x.parent.color = Color.Black;
                    parentLeft.left.color = Color.Black;
                    RightRotate(x.parent);
                    x = root;
                }

            if (x != null)
                x.color = Color.Black;
        }

        private Node Minimum(Node x)
        {
            while (x.left.left != null) x = x.left;

            if (x.left.right != null) x = x.left.right;

            return x;
        }

        private Node TreeSuccessor(Node x)
        {
            if (x.left != null) return Minimum(x);

            var xParent = x.parent;
            while (xParent != null && x == xParent.right)
            {
                x = xParent;
                xParent = xParent.parent;
            }

            return xParent;
        }

        public IList<KeyValuePair<TK, TV>> ToList()
        {
            var res = new List<KeyValuePair<TK, TV>>();

            AddToList(res, root);

            return res;
        }

        private static void AddToList(ICollection<KeyValuePair<TK, TV>> list, Node node)
        {
            while (true)
            {
                if (node == null) return;

                AddToList(list, node.left);
                list.Add(new KeyValuePair<TK, TV>(node.key, node.data));
                node = node.right;
            }
        }

        public class Node
        {
            public readonly int hash;
            public readonly TK key;
            public Color color;
            public TV data;
            public Node left;
            public Node parent;
            public Node right;

            public Node(TK key, TV data)
            {
                this.key = key;
                this.data = data;
                hash = key.GetHashCode();
            }
        }
    }
}