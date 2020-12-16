using System;
using System.IO;

namespace ARC_Assembler
{
    static class Program
    {
        // searching for ".begin"
        static int searchBeginning(this string[] lines)
        {
            int currentLine = 0;

            while (currentLine < lines.Length && lines[currentLine].Trim() != ".begin")
                currentLine++;

            return currentLine;
        }

        // searching for ".org"
        static int searchStartingAdress(this string[] lines, ref int currentLine)
        {
            for (currentLine++; currentLine < lines.Length; currentLine++)
            {
                string[] words = lines[currentLine].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                words[0] = words[0].Trim();

                if (words[0] == ".org")
                    return int.Parse(words[1]);
            }

            return 0;
        }

        static void Main(string[] args)
        {
            string filePath = @"..\..\ARC documentation\addTwoIntegers.asm";

            string[] lines = File.ReadAllLines(filePath);

            int currentLine = lines.searchBeginning();

            int currentMemoryAdress = lines.searchStartingAdress(ref currentLine);

            Console.WriteLine(currentLine + " " + currentMemoryAdress);
        }
    }
}
