using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Linq;

namespace ConsoleApp3
{
    public class Node 
    {
        public int Num { get; set; }
        public virtual Node Parent { get; set; } = null!;
        public List<Node> Childs { get; set; } = new List<Node>();

        public Node(Node parent, int num)
        {
            Num = num;
            Parent = parent;
            Parent.AddNode(this);
        }
        public void AddNode(Node node) => Childs.Add(node);
        protected Node() { }
    }
    public class Tree : Node
    {
        public Tree() 
        {
            Parent = this;
        }

    }
    internal class Program
    {
        static int[] nums = new int[] { 13, 8, 15, 12, 18, 0, 9, 16, 14 };
        static void Main(string[] args)
        {
            var tree = GetTree(nums);

            ShowTree(tree);

            FindNumConsoleView(tree);

            DeleteTree(tree);

            ShowTree(tree);
        }
        
        static void FindNumConsoleView(Tree tree, int num = 13)
        {
            //num = int.Parse(Console.ReadLine() ?? "0");
            var res = FindNodeWithNum(tree, num);
            Console.WriteLine("Res is {0}", res?.Num);
        }

        static void ShowTree(Node node)
        {
            if (node == null || node.Childs.Count == 0)
            {
                return;
            }

            foreach (var child in node.Childs.Reverse<Node>())
            {
                ShowTree(child);
            }
            Console.WriteLine("______");
            foreach (var child in node.Childs)
            {
                Console.WriteLine("The CHILD {0} with PARENT {1}", child?.Num, child?.Parent?.Num);
            }
        }

        

        static Tree GetTree(params int[] nums)
        {
            var sortedNums = SortNums(nums);
            var tree = CreateTree(sortedNums);
            return tree;
        }
        #region tree creation
        static int[] SortNums(params int[] nums)
        {
            int temp;
            for (int i = 0; i < nums.Length - 1; i++)
            {
                for (int j = i + 1; j < nums.Length; j++)
                {
                    if (nums[i] > nums[j])
                    {
                        temp = nums[i];
                        nums[i] = nums[j];
                        nums[j] = temp;
                    }
                }
            }
            return nums;
        }
        static List<Node> GetNodesByLevel(int level, Tree tree, int[] nums)
        {
            var numsToAdd = GetNumsByLevel(level, nums);
            var newNodes = new List<Node>();
            int i = 0;
            if (level == 1)
            {
                var nodeToAdd = new Node(tree, numsToAdd.First());
                newNodes.Add(nodeToAdd);
                return tree.Childs;
            }
            var nodes = GetNodesByLevel(level - 1, tree, nums);

            foreach (var node in nodes)
            {
                if (i >= numsToAdd.Length)
                    break;

                var num1 = numsToAdd[i];
                var num2 = numsToAdd[i + 1];
                var nodeToAdd1 = new Node(node, num1);
                var nodeToAdd2 = new Node(node, num2);
                newNodes.Add(nodeToAdd1);
                newNodes.Add(nodeToAdd2);
                i += 2;
            }

            return newNodes;
        }

        static int[] GetNumsByLevel(int level, int[] nums)
        {
            if (level == 0)
                return Array.Empty<int>();

            if (level == 1)
                return nums.Take(CountTake(level)).ToArray();

            return nums.Skip(CountSkip(level)).Take(CountTake(level)).ToArray();
        }
        static int CountTake(int level) => Convert.ToInt32(Math.Pow(2, level - 1));


        static int CountSkip(int level)
        {
            int skip = 0;
            for (int i = 1; i < level; i++)
            {
                skip += CountTake(i);
            }
            return skip;
        }

        static int ProccesLevel(int[] nums)
        {
            if (nums.Length == 0)
                return 0;

            int level = 1;

            while (GetNumsByLevel(level, nums).Length > 0)
            {
                level++;
            }
            return level - 1;
        }

        static Tree CreateTree(params int[] sortedNums)
        {
            Tree tree = new Tree();
            GetNodesByLevel(ProccesLevel(sortedNums), tree, sortedNums);
            return tree;
        }
        #endregion


        static Node? FindNodeWithNum(Tree tree, int num)
        {
            void AddChildsToStack(Queue<Node> stack, Node node)
            {
                foreach (var child in node.Childs)
                {
                    stack.Enqueue(child);
                }
            }
            Queue<Node> nodesForSearching = new Queue<Node>();
            AddChildsToStack(nodesForSearching, tree);
            do
            {
                var node = nodesForSearching.Dequeue();
                if(node.Num == num)
                    return node;

                AddChildsToStack(nodesForSearching, node);
            } 
            while (nodesForSearching.Count > 0);
            return null;
        }
        
        static void DeleteTree(Node tree)
        {
            if(tree.Childs.Count <= 0)
            {
                return;
            }
            foreach (var child in tree.Childs)
            {
                DeleteTree(child);
            }
            foreach (var child in tree.Childs)
            {
                child.Childs.Clear();
                child.Parent = null;
            }
        }
    }
}