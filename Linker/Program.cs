using System;
using System.IO;
using System.Collections.Generic;

namespace Linker
{
    static class LinkerClass
    {
        static string path = @"../../IO/input-2";
        static string file = File.ReadAllText(path);

        static string[] words = file.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        static Dictionary<string, int> definedSymbols = new Dictionary<string, int>();

        static class firstStep
        {
            static int currentIndex = 0;
            static int absoluteIndex = 0;

            static internal void begin()
            {
                int modulesCount = int.Parse(words[currentIndex++]);

                for (int i = 0; i < modulesCount; i++)
                {
                    defineSymbols();

                    iterateImports();

                    iterateAddresses();
                }

                printSymbolTable();
            }

            static void defineSymbols()
            {
                int definitionsCount = int.Parse(words[currentIndex++]);

                for (int i = 0; i < definitionsCount; i++)
                {
                    definedSymbols.Add(words[currentIndex], int.Parse(words[currentIndex + 1]) + absoluteIndex);
                    currentIndex += 2;
                }
            }

            static void iterateImports()
            {
                int importsCount = int.Parse(words[currentIndex++]);

                for (int i = 0; i < importsCount; i++)
                    currentIndex += 2;
            }

            static void iterateAddresses()
            {
                int addressesCount = int.Parse(words[currentIndex++]);

                absoluteIndex += addressesCount;

                for (int i = 0; i < addressesCount; i++)
                    currentIndex++;
            }

            static void printSymbolTable()
            {
                Console.WriteLine("Symbol Table");
                foreach (var symbol in definedSymbols)
                    Console.WriteLine($"{symbol.Key}={symbol.Value}");

                Console.WriteLine();
            }
        }

        static class secondStep
        {
            static int currentIndex = 0;
            static int absoluteIndex = 0;
            static int memoryMapIdx = 0;

            static internal void begin()
            {
                int modulesCount = int.Parse(words[currentIndex++]);

                Console.WriteLine("Memory Map");

                for (int i = 0; i < modulesCount; i++)
                {
                    iterateDefinitions();

                    Dictionary<string, int> importedSymbols = new Dictionary<string, int>();
                    readImports(ref importedSymbols);

                    handleAddresses(importedSymbols);
                }

                Console.WriteLine();
            }

            static void iterateDefinitions()
            {
                int symbolsCount = int.Parse(words[currentIndex++]);

                for (int i = 0; i < symbolsCount; i++)
                    currentIndex += 2;
            }

            static void readImports(ref Dictionary<string, int> importedSymbols)
            {
                int importsCount = int.Parse(words[currentIndex++]);

                for (int i = 0; i < importsCount; i++)
                {
                    importedSymbols.Add(words[currentIndex], int.Parse(words[currentIndex + 1]));
                    currentIndex += 2;
                }
            }
            static void handleAddresses(Dictionary<string, int> importedSymbols)
            {

                int addressesCount = int.Parse(words[currentIndex++]);

                string[] addresses = new string[addressesCount];

                for (int i = 0; i < addressesCount; i++)
                    addresses[i] = words[currentIndex++];

                handleAddressType1and2and3(addresses);

                handleAddressType4(importedSymbols, addresses);

                printAddresses(addresses);

                absoluteIndex += addressesCount;
            }


            static char typeOf(string address)
            {
                if (address.Length == 5)
                    return address[address.Length - 1];
                return '0';
            }
            static void handleAddressType1and2and3(string[] addresses)
            {
                for (int i = 0; i < addresses.Length; i++)
                {

                    if (typeOf(addresses[i]) == '1' || typeOf(addresses[i]) == '2')
                        addresses[i] = addresses[i].Remove(addresses[i].Length - 1, 1);

                    if (typeOf(addresses[i]) == '3')
                    {
                        int newValue = getPointerAt(addresses[i]) + absoluteIndex;
                        addresses[i] = string.Format($"{addresses[i][0]}{newValue:D3}");
                    }    
                }
            }

            static void handleAddressType4(Dictionary<string, int> importedSymbols, string[] addresses)
            {


                foreach (var importedSymbol in importedSymbols)
                {
                    int pointer = importedSymbol.Value;

                    while (pointer != 777)
                    {
                        int aux = getPointerAt(addresses[pointer]);

                        addresses[pointer] = string.Format($"{addresses[pointer][0]}{definedSymbols[importedSymbol.Key]:D3}");

                        pointer = aux;
                    }
                }
            }

            static void printAddresses(string[] addresses)
            {
                foreach (var address in addresses)
                    Console.WriteLine($"{$"{memoryMapIdx++}:",-4}{address}");
            }

            static int getPointerAt(string value)
            {
                return int.Parse(value.Substring(1, 3));
            }

        }

        static void Main(string[] args)
        {
            firstStep.begin();
            secondStep.begin();
        }
    }
}
