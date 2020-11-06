using System;
using System.IO;

namespace Hexviewer
{
    class Program
    {
        static void Main(string[] args)
        {
            bool done = false;

            while (!done) // cat timp un fisier nici un fisier un fisier nu a fost vizualizat (prin hexviewer)
            {
                try
                {
                    Console.Write(" Introduceti calea fisierului pentru a-l vizualiza prin HexViewer:\n ");
                    string path = Console.ReadLine();
                    Console.WriteLine();


                    Console.Write(" Introduceti cati octeti vreti sa fie pe o singura linie:\n ");
                    int nrOcteti = int.Parse(Console.ReadLine());
                    Console.WriteLine();

                    char[] caractereDeEliminat = new char[] { ' ', '"' };
                    path = path.Trim(caractereDeEliminat);  // eliminam caracterele speciale de la inceputul si sfarsitul caii (path-ului)


                    if (!File.Exists(path))
                        throw new FileNotFoundException("Path gresit.");

                    FileStream file = new FileStream(path, FileMode.Open); // variabila prin care vom putea citi din fisier

                    byte[] byteBlock = new byte[nrOcteti]; // variabila cu care vom retine textul din fisier pentru a-l prelucra
                    int idx = 0;    // index-ul pentru rand-ul afisat

                    int actual;

                    while ((actual = file.Read(byteBlock, 0, nrOcteti)) > 0)    // citim 16 caractere ca si bytes pana cand nu mai exista nimic in fisier
                    {
                        string hex = BitConverter.ToString(byteBlock, 0, actual);  // convertim secventa de bytes intr-un sir hexazecimal

                        //hex = hex.Replace("-", " ");  // bitconverter separa fiecare byte prin '-', iar noi il eliminam pentru lizibilitate

                        string text = "";           // string-ul care va contine ce caractere contine defapt textul
                        for (int i = 0; i < actual; i++)
                            text += byteBlock[i] < ' ' ? "." : ((char)byteBlock[i]).ToString();
                        // transformam fiecare byte in caracter iar caracterele speciale (ex. carriage return, newline) (daca exista) in '.'

                        Console.WriteLine($" {idx:X8} : {hex.PadRight(nrOcteti * 3 - 1)}  | {text}");     // afisam un rand
                        idx += nrOcteti;

                        //Array.Clear(byteBlock, 0, byteBlock.Length);  // golim vectorul de bytes pentru a evita output gresit la ultima linie (ar afisa o parte din linia anterioara)
                    }

                    file.Close();

                    done = true; // incetam repetitia programului

                    Console.Write("\n\n ");
                }
                catch (Exception e)
                {
                    Console.WriteLine($" {e.Message}\n\n");
                }
            }
        }
    }
}
