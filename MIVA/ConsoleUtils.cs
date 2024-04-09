using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVA
{
    internal static class ConsoleUtils
    {
        public static string ReadInput(string prompt, string defaultValue)
        {
            Console.Write($"{prompt} ({defaultValue}): ");
            var input = Console.ReadLine();
            if(input == string.Empty)
            {
                return defaultValue;
            } else
            {
                return input;
            }
        }

        public static double ReadInput(string prompt, double defaultValue)
        {
            Console.Write($"{prompt} ({defaultValue}): ");

            double input;
            bool isDouble = double.TryParse(Console.ReadLine(), out input);

            if (isDouble)
            {
                return input;
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
