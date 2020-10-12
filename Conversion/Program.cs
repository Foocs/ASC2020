using System;

namespace Conversion
{
    class Program
    {
        static string conversionFromBase10(int nr, int baza)
        {
            if (nr != 0)
            {
                if (nr%baza > 9)
                    return conversionFromBase10(nr / baza, baza)+ Convert.ToString(Convert.ToChar((nr % baza) % 10 + 'A'));
                return conversionFromBase10(nr / baza, baza)+ nr % baza;
            }
            return "";
        }
        static void Main(string[] args)
        {
            Console.WriteLine("+---------------------------------------------------------+");
            Console.WriteLine("|---Converteste un numar din baza 10 in orice alta baza---|");
            Console.WriteLine("+---------------------------------------------------------+");

            while (true)
            {
                try
                {
                    Console.Write("\nIntroduceti numarul in baza 10: ");
                    int nr = int.Parse(Console.ReadLine());

                    Console.Write("\nIntroduceti baza tinta (intre 2 si 16): ");
                    int baza = int.Parse(Console.ReadLine());
                    if (baza < 2 || baza > 16)
                        throw new IndexOutOfRangeException();

                    Console.WriteLine($"\nNumarul {nr} din baza 10 este egal cu {conversionFromBase10(nr, baza)} in baza {baza}.\n");
                }

                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }

                finally
                {
                    Console.WriteLine("\n----Incearca din nou----");
                }
            }
        }
    }
}
