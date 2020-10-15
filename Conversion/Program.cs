using System;

namespace Conversion
{
    class Program
    {
        #region Functions

        static string convertFromBase10Decimal(double nr, int bazaTinta, int nrZecimale = 0)
        {
            if (nr == Math.Floor(nr) && nrZecimale > 0 || nrZecimale > 20) // configurat sa se opreasca dupa 20 de zecimale
                return "";
            return ((int)(nr * bazaTinta)).ToString() + convertFromBase10Decimal(nr * bazaTinta - Math.Floor(nr * bazaTinta), bazaTinta, nrZecimale + 1);
        }

        static int val(string nr, int index)
        {
            return nr[nr.Length - index - 1];
        }
        static string convertFromBase10Whole(int nr, int bazaTinta) // convertim in baza 10 prin impartiri repetate, concatenand restul
        {
            if (nr != 0)
            {
                if (nr % bazaTinta > 9)
                    return convertFromBase10Whole(nr / bazaTinta, bazaTinta) +
                           Convert.ToString(Convert.ToChar((nr % bazaTinta) % 10 + 'A'));    // convertirea in litere

                return convertFromBase10Whole(nr / bazaTinta, bazaTinta) + nr % bazaTinta;   // convertirea in numere
            }
            return "";
        }

        static double convertToBase10(string nr, int bazaOriginala, int nrZecimale, int index = 0) // convertim din baza 10 prin formula numar[index] * bazaTinta ^ index
        {
            if (index < nr.Length)
            {
                if (nr[nr.Length - index - 1] >= '0' && nr[nr.Length - index - 1] <= '9')
                    return convertToBase10(nr, bazaOriginala, nrZecimale, index + 1) +
                           (val(nr, index) - '0') * Math.Pow(bazaOriginala, index - nrZecimale); // convertirea din cifre

                return convertToBase10(nr, bazaOriginala, nrZecimale, index + 1) +
                      (val(nr, index) - 'A' + 10) * Math.Pow(bazaOriginala, index - nrZecimale); // convertirea din litere
            }
            return 0;
        }

        #endregion
        static void Main(string[] args)
        {
            // TODO Find bugs
            #region Intro

            Console.WriteLine("\n");
            Console.WriteLine("      +----------------------------------------+");
            Console.WriteLine("      |                                        |");
            Console.WriteLine("      |   Converteste un numar in orice baza   |");
            Console.WriteLine("      |                                        |");
            Console.WriteLine("      +----------------------------------------+");
            Console.WriteLine();


            #endregion

            #region Driver Code

            while (true)
            {
                try
                {
                    #region Input

                    Console.Write("\n      Introduceti numarul: ");
                    string originalNumber = Console.ReadLine().ToUpper();
                    string nr = originalNumber;

                    Console.Write("      Introduceti baza numarului: ");
                    int bazaOriginala = int.Parse(Console.ReadLine());

                    Console.Write("      Introduceti baza tinta: ");
                    int bazaTinta = int.Parse(Console.ReadLine());

                    #endregion

                    #region Format for decimal/minus sign
                    bool minus = nr.IndexOf('-') == -1 ? false : true; // verificam daca numarul este negativ
                    nr = nr.Replace("-", "");                          // eliminam minusul

                    int nrZecimale = nr.IndexOf('.') == -1 ? nr.IndexOf(',') == -1 ? nr.Length - 1 : nr.IndexOf(',') : nr.IndexOf('.'); // cautam unde se gasesc cifrele zecimale
                    nrZecimale = nr.Length - 1 - nrZecimale;                                                                            // calculam cate cifre zecimale sunt

                    nr = nr.Replace(".", "");
                    nr = nr.Replace(",", "");
                    #endregion

                    #region Custom Exception Handling

                    foreach (char c in nr)
                        if (!((c >= '0' && c <= '0' + Convert.ToChar(bazaOriginala)) ||
                             ((c >= 'A' && c <= 'A' + Convert.ToChar(bazaOriginala) - 10) && (bazaOriginala > 10 && bazaOriginala <= 16)))) // verificam daca numarul este conform bazei originale (daca este corect)
                            throw new Exception("\n      ERROR: Numerele pot avea doar cifre sau litere mai mici ca baza originala");

                    if ((bazaOriginala < 2 || bazaOriginala > 16) || (bazaTinta < 2 || bazaTinta > 16))
                        throw new Exception("      Baza poate fi doar un numar intre 2 si 16");

                    #endregion

                    //convertim numarul in baza 10 apoi in din baza 10 in baza tinta
                    double nrBaza10 = convertToBase10(nr, bazaOriginala, nrZecimale); // * Math.Pow(bazaTinta, 16); // inmultim pentru a trata cifrele zecimale, afiseaza 16 cifre zecimale
                    string nrBazaNoua = (int)nrBaza10 != 0 ? convertFromBase10Whole((int)Math.Floor(nrBaza10), bazaTinta) : "0"; // convertim partea intreaga in baza noua, 
                                                                                                                                 //atasam "0" pentru ca pt [0.x] functia nu returneza nimic

                    nrBazaNoua += "." + convertFromBase10Decimal(nrBaza10 - Math.Floor(nrBaza10), bazaTinta); // adaugam zecimalele

                    nrBazaNoua = (minus ? "-" : "") + nrBazaNoua; // adaugam minusul (daca este cazul)

                    Console.WriteLine($"\n      Numarul {originalNumber} din baza {bazaOriginala} este egal cu {nrBazaNoua} in baza {bazaTinta}.\n");
                }

                catch (Exception e)
                {
                    Console.WriteLine("      " + e.Message);
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
