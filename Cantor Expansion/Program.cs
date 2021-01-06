using System;
using System.Collections.Generic;

namespace Cantor_Expansion
{
    static class CantorExpansion
    {
        static string ToCantor(this int y)
        {
            var cantor = new Stack<int>();
            for (int n = 1; y != 0; n++)
            {
                cantor.Push(y % (n + 1));
                y = (y - cantor.Peek()) / (n + 1);
            }

            int i = 0;
            string result = "";

            foreach (var item in cantor)
                if (item != 0)
                    result += $"{item} * {cantor.Count - i++}! + ";

            return result.Remove(result.Length - 2);
        }

        static int ToDecimal(this string cantor)
        {
            int[] cantorArr = Array.ConvertAll(cantor.Split(new char[] { '!', '*', '+', ' ' }, StringSplitOptions.RemoveEmptyEntries), int.Parse);

            int result = 0;

            for (int i = 0; i < cantorArr.Length; i += 2)
                result = cantorArr[i + 1] * (result + cantorArr[i]);

            return result;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Enter a base 10 number to convert to cantor expansion: ");
            int number = int.Parse(Console.ReadLine());
            Console.WriteLine(number.ToCantor());
            Console.WriteLine();


            Console.WriteLine("Enter a cantor expansion to convert to base 10: ");
            string cantor = Console.ReadLine();
            Console.WriteLine(cantor.ToDecimal());
            Console.WriteLine();

        }
    }
}
