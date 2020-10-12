using System;

namespace Conversion
{
    class Program
    {
        #region Functions
        static string convertFromBase10(int nr, int bazaTinta) // convertim in baza 10 prin impartiri repetate, concatenand restul
        {
            if (nr != 0)
            {
                if (nr % bazaTinta > 9)
                    return convertFromBase10(nr / bazaTinta, bazaTinta) + Convert.ToString(Convert.ToChar((nr % bazaTinta) % 10 + 'A')); // convertirea in litere
                return convertFromBase10(nr / bazaTinta, bazaTinta) + nr % bazaTinta;                                                   // convertirea in numere
            }
            return "";
        }

        static int convertToBase10(string nr, int bazaOriginala, int index = 0) // convertim din baza 10 prin formula numar[index] * bazaTinta ^ index
        {
            if (index < nr.Length)
            {
                if (nr[nr.Length - index - 1] >= '0' && nr[nr.Length - index - 1] <= '9')
                    return convertToBase10(nr, bazaOriginala, index + 1) + (nr[nr.Length - index - 1] - '0') * (int)Math.Pow(bazaOriginala, index); // convertirea din cifre
                return convertToBase10(nr, bazaOriginala, index + 1) + (nr[nr.Length - index - 1] - 'A' + 10) * (int)Math.Pow(bazaOriginala, index); // convertirea din litere
            }
            return 0;
        }

        #endregion
        static void Main(string[] args)
        {
            #region Intro

            Console.WriteLine("\n\n\n");
            Console.WriteLine("      +----------------------------------------+");
            Console.WriteLine("      |                                        |");
            Console.WriteLine("      |   Converteste un numar in orice baza   |");
            Console.WriteLine("      |                                        |");
            Console.WriteLine("      +----------------------------------------+");
            Console.WriteLine();


            #endregion

            //TODO Numere reale

            #region Driver Code
            while (true)
            {
                try
                {
                    Console.Write("\n      Introduceti numarul: ");
                    string nr = Console.ReadLine().ToUpper();

                    Console.Write("      Introduceti baza numarului: ");
                    int bazaOriginala = int.Parse(Console.ReadLine());

                    Console.Write("      Introduceti baza tinta: ");
                    int bazaTinta = int.Parse(Console.ReadLine());

                    #region Custom Exception Handling

                    foreach (char c in nr)
                        if (!((c >= '0' && c <= Convert.ToChar(bazaOriginala)) || (c >= 'A' && c <= 'A' + Convert.ToChar(bazaOriginala) - 10))) // verificam daca numarul este conform bazei originale
                            throw new Exception("Numerele pot avea doar cifre sau litere mai mici ca baza originala");

                    if ((bazaOriginala < 2 || bazaOriginala > 16) || (bazaTinta < 2 || bazaTinta > 16))
                        throw new Exception("Baza poate fi doar un numar intre 2 si 16");

                    #endregion

                    //convertim numarul in baza 10 apoi in din baza 10 in baza tinta
                    Console.WriteLine($"\n      Numarul {nr} din baza {bazaOriginala} este egal cu {convertFromBase10(convertToBase10(nr, bazaOriginala), bazaTinta)} in baza {bazaTinta}.\n");
                }

                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }

                finally
                {
                    Console.WriteLine("\n      ----Incearca din nou----");
                }
            }
            #endregion
        }
    }
}
