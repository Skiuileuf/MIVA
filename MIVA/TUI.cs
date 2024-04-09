using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVA
{
    public class TUI
    {
        public MenuNode RootNode;

        private MenuNode CurrentNode;
        private Stack<MenuNode> Parents = new Stack<MenuNode>();

        private int width;
        private int height;

        public TUI(string title, int width, int height )
        {
            Console.CursorVisible = false;
            Console.Title = title;
            this.width = width;
            this.height = height;


            Console.SetWindowSize(width, height);
            Console.SetBufferSize(width, Console.BufferHeight);
        }

        public void Run(MenuNode RootNode)
        {
            this.RootNode = RootNode;

            CurrentNode = RootNode;


            bool canQuit = false;
            while (!canQuit)
            {
                //Print out the menu node
                Console.Clear();
                Console.SetCursorPosition(0, 0);

                SetConsoleColors(ConsoleColor.White, ConsoleColor.Magenta);
                if(Parents.Count <= 1)
                {
                    Console.Write($"Root > {CurrentNode.Title}".PadRight(this.width, ' '));
                } else
                {
                    Console.Write($"Root > {Parents.Count - 1} > {CurrentNode.Title}".PadRight(this.width, ' '));
                }
                //Console.SetCursorPosition(0, Console.CursorTop + 1);
                SetConsoleColors(ConsoleColor.White, ConsoleColor.Black);
                Console.WriteLine(CurrentNode.Title);
                SetConsoleColors(ConsoleColor.Black, ConsoleColor.White);
                Console.WriteLine(CurrentNode.Message + "\n");

                //Execute the node's action
                CurrentNode.OnActivate?.Invoke();

                //Print out the childrens' titles
                for (int i = 0; i < CurrentNode.Children.Count; i++)
                {
                    Console.WriteLine($"{i + 1} > {CurrentNode.Children[i].Title}");
                }

                //Print out the quit/back option
                if (Parents.Count == 0) //Current node is the root node
                {
                    Console.WriteLine($"q > Quit");
                }
                else //Current node has a parent
                {
                    Console.WriteLine($"q > Back");
                }

                //Read the console and decide what to do with the input
                while (true)
                {
                    char input = Console.ReadKey(true).KeyChar;

                    if (char.IsDigit(input))
                    {
                        int inpchr = int.Parse(input.ToString()); //input - '0'
                        if (inpchr > 0 && inpchr <= CurrentNode.Children.Count)
                        {
                            Parents.Push(CurrentNode);
                            CurrentNode = CurrentNode.Children[inpchr - 1];
                            break;
                        }
                    }

                    if (input == 'q')
                    {
                        BackOrQuit();
                        break;
                    }
                }

            }
        }

        /// <summary>
        /// Goes back one menu or exits the application if the current node is the root node.
        /// </summary>
        /// <returns>Boolean value indicating if it's a back or a quit</returns>
        public bool BackOrQuit()
        {
            //Determine if it's the root node
            if (Parents.Count == 0)
            {
                Environment.Exit(0);
                return false;
            }
            else
            {
                CurrentNode = Parents.Pop();
                return true;
            }
        }

        public class MenuNode
        {
            public List<MenuNode> Children = new List<MenuNode>();
            public string Title;
            public string Message;
            public delegate void OnActivateDelegate();

            public OnActivateDelegate OnActivate;

            public MenuNode(string Title, string Message, OnActivateDelegate OnActivate, params MenuNode[] Children)
            {
                this.Title = Title;
                this.Message = Message;
                this.OnActivate = OnActivate;
                this.Children = new List<MenuNode>(Children);
            }
        }

        public static void SetConsoleColors(ConsoleColor background, ConsoleColor foreground)
        {
            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;
        }
    }
}
