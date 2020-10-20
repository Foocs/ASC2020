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

        static int valAtIndex(string nr, int index)
        {
            return nr[nr.Length - index - 1];
        }

        static double convertToBase10(string nr, int bazaOriginala, int nrZecimale, int index = 0) // convertim din baza 10 prin formula numar[index] * bazaTinta ^ index
        {
            if (index < nr.Length)
            {
                if (valAtIndex(nr, index) >= '0' && valAtIndex(nr, index) <= '9')
                    return convertToBase10(nr, bazaOriginala, nrZecimale, index + 1) +
                           (valAtIndex(nr, index) - '0') * Math.Pow(bazaOriginala, index - nrZecimale); // convertirea din cifre

                return convertToBase10(nr, bazaOriginala, nrZecimale, index + 1) +
                      (valAtIndex(nr, index) - 'A' + 10) * Math.Pow(bazaOriginala, index - nrZecimale); // convertirea din litere
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

                    #region Formatting

                    bool minus = nr.IndexOf('-') == -1 ? false : true; // verificam daca numarul este negativ
                    nr = nr.Replace("-", "");                          // eliminam minusul

                    nr = nr.Replace(",", ".");                 // facem ca ambele semne ( "," si ".") sa functioneze
                    int nrZecimale = 0;
                    if (nr.IndexOf('.') != -1)
                        nrZecimale = nr.Split('.')[1].Length;  // impartim numarul in parte intreaga si zecimala iar apoi numaram cate nr zecimale sunt
                    nr = nr.Replace(".", "");                  //eliminam virgula pentru ca numarul sa poata fi procesat

                    #endregion

                    #region Custom Exception Handling

                    foreach (char c in nr)
                    {
                        if (!(char.IsDigit(c) || char.IsLetter(c)))    // verificam daca numarul este format din litere si/sau cifre
                            throw new Exception("\n      Numarul poate avea doar litere si/sau cifre.");

                        if (!((c <= '0' + Convert.ToChar(bazaOriginala)) ||
                             ((c <= 'A' + Convert.ToChar(bazaOriginala) - 10))))     // verificam daca numarul este conform bazei originale (daca este corect)
                            throw new Exception("\n      Numerele pot avea doar litere sau/si cifre mai mici ca baza originala.");
                    }

                    if ((bazaOriginala < 2 || bazaOriginala > 36) || (bazaTinta < 2 || bazaTinta > 36))
                        throw new Exception("\n      Baza poate fi doar un numar intre 2 si 36.");

                    #endregion

                    #region Conversion

                    //convertim numarul in baza 10 apoi in din baza 10 in baza tinta
                    double nrBaza10 = convertToBase10(nr, bazaOriginala, nrZecimale);
                    string nrBazaNoua = (int)nrBaza10 != 0 ? convertFromBase10Whole((int)nrBaza10, bazaTinta) : "0"; // convertim partea intreaga in baza noua, si atasam "0" in cazul in care partea intreaga e 0

                    if (nrZecimale > 0)
                        nrBazaNoua += "." + convertFromBase10Decimal(nrBaza10 - Math.Floor(nrBaza10), bazaTinta); // adaugam zecimalele

                    nrBazaNoua = (minus ? "-" : "") + nrBazaNoua; // adaugam minusul (daca este cazul)

                    #endregion

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
