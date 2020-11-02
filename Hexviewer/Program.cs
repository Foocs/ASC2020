using System;
using System.IO;

namespace Hexviewer
{
    class Program
    {
        static void Main(string[] args)
        {
            bool done = false;

            while (!done)
            {
                try
                {
                    Console.Write(" Introduceti calea fisierului pentru a-l vizualiza prin HexViewer:\n ");
                    string path = Console.ReadLine();
                    Console.WriteLine();


                    Console.Write(" Introduceti cati octeti vreti sa fie pe o singura linie:\n ");
                    int nrOcteti = int.Parse(Console.ReadLine());
                    Console.WriteLine();

                    path = path.Trim(new char[] { ' ', '"' });  // eliminam caracterele speciale de la inceputul si sfarsitul caii (path-ului)

                    FileStream file = new FileStream(path, FileMode.Open); // modul prin care accesam fisierul

                    byte[] byteBlock = new byte[nrOcteti];
                    int idx = 0;    // index-ul pentru rand-ul afisat

                    while (file.Read(byteBlock, 0, nrOcteti) > 0)    // citim 16 caractere ca si bytes pana cand nu mai exista nimic in fisier
                    {
                        string hex = BitConverter.ToString(byteBlock);  // convertim secventa de bytes intr-un sir hexazecimal

                        hex = hex.Replace("-", " ");  // bitconverter separa fiecare byte prin '-', iar noi eliminam '-' pentru lizibilitate

                        string text = "";
                        for (int i = 0; i < byteBlock.Length; i++)
                            text += byteBlock[i] < ' ' ? "." : ((char)byteBlock[i]).ToString();   // eliminam toate caracterele speciale precum newline, carriage return

                        Console.WriteLine($" {idx++:X7}0 : {hex}  | {text}");     // afisam un rand

                        Array.Clear(byteBlock, 0, byteBlock.Length);  // golim vectorul de bytes pentru a evita output gresit la ultima linie (ar afisa o parte din linia anterioara)
                    }

                    file.Close();
                    done = true;

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
