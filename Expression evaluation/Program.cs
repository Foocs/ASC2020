using System;
using System.Collections.Generic;

// Reference: https://www.geeksforgeeks.org/expression-evaluation/
namespace Expression_evaluation
{
    static class ExpEvaluator
    {
        internal static Dictionary<char, Func<int, int, int>> operations = new Dictionary<char, Func<int, int, int>>()
        {
            {'+', (b, a) => { return a + b; } },
            {'-', (b, a) => { return a - b; } },
            {'*', (b, a) => { return a * b; } },
            {'/', (b, a) => { return b == 0 ? throw new DivideByZeroException("Cannot divide by zero") : a / b; } },
            {'%', (b, a) => { return a % b; } },
        };

        static bool IsOperator(this char operation)
        {
            foreach (char op in operations.Keys)
                if (op == operation)
                    return true;

            return false;
        }
        static bool HasPrecedence(char op1, char op2)
        {
            if (op2 == '(' || op2 == ')')
                return false;

            if ((op1 == '*' || op1 == '/' || op1 == '%') && (op2 == '+' || op2 == '-'))
                return false;

            else
                return true;
        }
        static void checkParenthesis(string expression)
        {
            int sum = 0, valMax = 0;
            for (int i = 0; i < expression.Length && sum >= 0; i++)
            {
                sum += expression[i] == '(' ? 1 : 0;
                sum += expression[i] == ')' ? -1 : 0;

                valMax = sum > valMax ? sum : valMax;
            }

            if (sum != 0)
                throw new FormatException("Invalid expression.");
        }
        static int EvaluateExpression(this string expression)
        {
            checkParenthesis(expression);

            Stack<int> numbers = new Stack<int>();
            Stack<char> operators = new Stack<char>();

            bool nextIsOperator = false;

            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == ' ')
                    continue;

                else if (char.IsDigit(expression[i]) || ((expression[i] == '-' || expression[i] == '+') && i + 1 < expression.Length && char.IsDigit(expression[i + 1])))
                {
                    if (nextIsOperator)
                        throw new FormatException("Invalid expression.");
                    nextIsOperator = true;

                    string number = "" + expression[i++];

                    while (i < expression.Length && char.IsDigit(expression[i]))
                        number += expression[i++];
                    i--;

                    numbers.Push(int.Parse(number));
                }

                else if (expression[i] == '(')
                {
                    if (nextIsOperator)
                        throw new FormatException("Invalid expression.");

                    operators.Push(expression[i]);
                }

                else if (expression[i] == ')')
                {
                    while (operators.Peek() != '(')
                        numbers.Push(operations[operators.Pop()](numbers.Pop(), numbers.Pop()));
                    operators.Pop();
                }

                else if (expression[i].IsOperator())
                {
                    if (!nextIsOperator)
                        throw new FormatException("Invalid expression.");
                    nextIsOperator = false;

                    while (operators.Count > 0 && HasPrecedence(expression[i], operators.Peek()))
                        numbers.Push(operations[operators.Pop()](numbers.Pop(), numbers.Pop()));
                    operators.Push(expression[i]);
                }
                else
                    throw new FormatException("Invalid expression.");
            }

            while (operators.Count > 0)
                numbers.Push(operations[operators.Pop()](numbers.Pop(), numbers.Pop()));

            return numbers.Pop();
        }
        static void Main(string[] args)
        {
            string expression = "-5 - (3 - (4 / 2))";

            Console.WriteLine(expression.EvaluateExpression());
        }
    }
}
