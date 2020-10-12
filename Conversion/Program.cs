using System;

namespace Conversion
{
    class Program
    {
        #region Functions
        static string convertFromBase10(int nr, int bazaTinta)
        {
            if (nr != 0)
            {
                if (nr % bazaTinta > 9)
                    return convertFromBase10(nr / bazaTinta, bazaTinta) + Convert.ToString(Convert.ToChar((nr % bazaTinta) % 10 + 'A'));
                return convertFromBase10(nr / bazaTinta, bazaTinta) + nr % bazaTinta;
            }
            return "";
        }

        static int convertToBase10(string nr, int bazaOriginala, int index = 0)
        {
            if (index < nr.Length)
            {
                if (nr[nr.Length - index - 1] >= '0' && nr[nr.Length - index - 1] <= '9')
                    return convertToBase10(nr, bazaOriginala, index + 1) + (nr[nr.Length - index - 1] - '0') * (int)Math.Pow(bazaOriginala, index);
                return convertToBase10(nr, bazaOriginala, index + 1) + (nr[nr.Length - index - 1] - 'A' + 10) * (int)Math.Pow(bazaOriginala, index);
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
                        if (!((c >= '0' && c <= Convert.ToChar(bazaOriginala)) || (c >= 'A' && c <= 'F')))
                            throw new Exception("Numerele pot avea doar cifre mai mici ca baza originala sau litere de la A la F");

                    if ((bazaOriginala < 2 || bazaOriginala > 16) || (bazaTinta < 2 || bazaTinta > 16))
                        throw new Exception("Baza poate fi doar un numar intre 2 si 16");

                    #endregion

                    Console.WriteLine(convertToBase10(nr, bazaOriginala));
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
