using System;
using System.Linq;

namespace BigOperations
{
    public static class BigNumbers
    {
        static string Addition(string a, string b)
        {
            string result = "";

            int ai = a.Length - 1;
            int bi = b.Length - 1;

            int sum = 0;

            while (ai >= 0 && bi >= 0)
            {
                sum += a[ai--] - '0' + b[bi--] - '0';
                result = sum % 10 + result;
                sum /= 10;
            }

            while (ai >= 0)
            {
                sum += a[ai--] - '0';
                result = sum % 10 + result;
                sum /= 10;
            }

            while (bi >= 0)
            {
                sum += b[bi--] - '0';
                result = sum % 10 + result;
                sum /= 10;
            }

            result = sum + result;

            result = result.TrimStart('0');

            return result;
        }

        static bool aIsLowerThanB(string a, string b)
        {
            if (a.Length < b.Length)
                return true;

            if (a.Length == b.Length)
                for (int i = 0; i < a.Length; i++)
                    if (a[i] < b[i])
                        return true;
                    else if (a[i] > b[i])
                        return false;

            return false;
        }

        static string Subtraction(string a, string b)
        {
            string minus = "";

            if (a == b)
                return "0";

            if (aIsLowerThanB(a, b))
            {
                (a, b) = (b, a);
                minus = "-";
            }

            string result = "";

            int ai = a.Length - 1;
            int bi = b.Length - 1;

            int diff = 0;

            while (ai >= 0 && bi >= 0)
            {
                diff += 10 + a[ai--] - '0' - (b[bi--] - '0');
                result = diff % 10 + result;
                diff = diff / 10 - 1;
            }

            while (ai >= 0)
            {
                diff += 10 + a[ai--] - '0';
                result = diff % 10 + result;
                diff = diff / 10 - 1;
            }

            while (bi >= 0)
            {
                diff += 10 + b[bi--] - '0';
                result = diff % 10 + result;
                diff = diff / 10 - 1;
            }

            result = result.TrimStart('0');

            result = minus + result;

            return result;
        }

        static string MultiplyByDigit(string a, char b)
        {
            string result = "";

            int ai = a.Length - 1;

            int sum = 0;

            while (ai >= 0)
            {
                sum += (a[ai--] - '0') * (b - '0');
                result = sum % 10 + result;
                sum /= 10;
            }

            while (sum > 0)
            {
                result = sum % 10 + result;
                sum /= 10;
            }

            return result;
        }

        static string Multiplication(string a, string b)
        {
            string result = "0";

            string zeros = "";

            for (int i = b.Length - 1; i >= 0; i--)
            {
                string multiplied = MultiplyByDigit(a, b[i]);
                multiplied += zeros;
                zeros += "0";
                result = Addition(multiplied, result);
            }

            return result;
        }

        static string Division(string a, string b)
        {
            if (a == b)
                return "1";

            string count = "0";

            while (a[0] != '-')
            {
                a = Subtraction(a, b);
                count = Addition(count, "1");
            }

            count = Subtraction(count, "1");

            return count;
        }

        static string Factorial(string x)
        {
            string count = "1";
            string result = "1";

            while (count != x)
            {
                count = Addition(count, "1");
                result = Multiplication(result, count);
            }

            return result;
        }

        static string Power(string a, string b)
        {
            string count = "0";

            string result = "1";

            while (count != b)
            {
                count = Addition(count, "1");
                result = Multiplication(result, a);
            }

            return result;
        }

        static string SquareRoot(string x)
        {
            //Babylonian Method

            string oldOldGuess = "";
            string oldGuess = "";
            string guess = "1";

            while (oldOldGuess != guess)
            {
                oldOldGuess = oldGuess;
                oldGuess = guess;
                guess = Division(Addition(guess, Division(x, guess)), "2"); //guess = (guess + x / guess) / 2
            }

            return guess;
        }

        static void PrintResults(string a, string b)
        {
            Console.WriteLine($"a = {a}");
            Console.WriteLine($"b = {b}");
            Console.WriteLine();

            Console.WriteLine($"Addition: a + b = {Addition(a, b)}");

            Console.WriteLine($"Subtraction: a - b = {Subtraction(a, b)}");

            Console.WriteLine($"Multiplication: a * b = {Multiplication(a, b)}");

            Console.WriteLine($"Division: a / b = {Division(a, b)}");

            Console.WriteLine($"Factorial a = {Factorial(a)}");
            Console.WriteLine($"Factorial b = {Factorial(b)}");

            Console.WriteLine($"Power: a ^ b = {Power(a, b)}");

            Console.WriteLine($"SquareRoot a: {SquareRoot(a)}");
            Console.WriteLine($"SquareRoot b: {SquareRoot(b)}");
        }

        static void ReadNumbers(ref string a, ref string b)
        {
            Console.Write("Number a: ");
            a = Console.ReadLine();
            Console.WriteLine();

            if (!a.isNumber())
            {
                Console.WriteLine("Wrong Input");
                ReadNumbers(ref a, ref b);
            }

            Console.Write("Number b: ");
            b = Console.ReadLine();
            Console.WriteLine();

            if (!b.isNumber())
            {
                Console.WriteLine("Wrong Input");
                ReadNumbers(ref a, ref b);
            }
        }

        static bool isNumber(this string a)
        {
            return a.All(char.IsDigit);
        }

        public static void Main()
        {
            string a = "";
            string b = "";

            ReadNumbers(ref a, ref b);
            PrintResults(a, b);
        }
    }
}