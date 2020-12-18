using System;
using System.IO;
using System.Collections.Generic;

namespace Linker
{
    static class LinkerClass
    {
        static string filePath = @"IO/input-1";
        static string file = File.ReadAllText(filePath);

        static string[] words = file.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        internal class DefinedClass
        {
            internal int ModuleIdx { get; set; }
            internal int AbsoluteAddress { get; set; }
            internal bool Used { get; set; }
            internal bool MultiplyDefined { get; set; }
            internal bool OutsideModule { get; set; }

            internal DefinedClass(int moduleIdx, int absoluteAddress, bool used, bool multiplyDefined, bool outsideModule)
            {
                ModuleIdx = moduleIdx;
                AbsoluteAddress = absoluteAddress;
                Used = used;
                MultiplyDefined = multiplyDefined;
                OutsideModule = outsideModule;
            }
        }

        static Dictionary<string, DefinedClass> definedSymbols = new Dictionary<string, DefinedClass>();

        static class FirstStep
        {
            static int currentIndex = 0;
            static int absoluteIndex = 0;

            static internal void begin()
            {
                int modulesCount = int.Parse(words[currentIndex++]);

                for (int moduleIdx = 0; moduleIdx < modulesCount; moduleIdx++)
                {
                    List<string> currentSymbolsInModule = new List<string>();

                    defineSymbols(moduleIdx, currentSymbolsInModule);

                    iterateImports();

                    iterateAddresses(currentSymbolsInModule);
                }

                printSymbolTable();
            }

            static void defineSymbols(int moduleIdx, List<string> currentSymbolsInModule)
            {
                int definitionsCount = int.Parse(words[currentIndex++]);

                for (int i = 0; i < definitionsCount; i++)
                {
                    string symbol = words[currentIndex];
                    int absoluteAddress = int.Parse(words[currentIndex + 1]) + absoluteIndex;

                    if (definedSymbols.ContainsKey(symbol))
                        definedSymbols[symbol].MultiplyDefined = true;
                    else
                    {
                        definedSymbols.Add(symbol, new DefinedClass(moduleIdx, absoluteAddress, false, false, false));

                        currentSymbolsInModule.Add(symbol);
                    }

                    currentIndex += 2;
                }
            }

            static void iterateImports()
            {
                int importsCount = int.Parse(words[currentIndex++]);

                for (int i = 0; i < importsCount; i++)
                    currentIndex += 2;
            }

            static void iterateAddresses(List<string> currentSymbolsInModule)
            {
                int addressesCount = int.Parse(words[currentIndex++]);


                foreach (var symbol in currentSymbolsInModule)
                    if (definedSymbols[symbol].AbsoluteAddress - absoluteIndex >= addressesCount)
                        definedSymbols[symbol].AbsoluteAddress = absoluteIndex;

                absoluteIndex += addressesCount;

                for (int i = 0; i < addressesCount; i++)
                    currentIndex++;
            }

            static string checkIfMultiplyDefined(bool alreadyDefined)
            {
                return alreadyDefined ? " Error: This variable is multiply defined; first value used." : "";
            }

            static string checkIfOutsideModule(string symbol, bool outsideModule)
            {
                return outsideModule ? $" Error: The definition of {symbol} is outside module 1; zero (relative) used." : "";
            }

            static void printSymbolTable()
            {
                Console.WriteLine("Symbol Table");
                foreach (var symbol in definedSymbols)
                    Console.WriteLine($"{symbol.Key}={symbol.Value.AbsoluteAddress}{checkIfMultiplyDefined(symbol.Value.MultiplyDefined)}{checkIfOutsideModule(symbol.Key, symbol.Value.OutsideModule)}");

                Console.WriteLine();
            }
        }

        static class SecondStep
        {
            static int currentIndex = 0;
            static int absoluteIndex = 0;
            static int memoryMapIdx = 0;

            class ImportedClass
            {
                internal int RelativeAddress { get; set; }
                internal bool Defined { get; set; }
                internal ImportedClass(int relativeAddress, bool defined)
                {
                    RelativeAddress = relativeAddress;
                    Defined = defined;
                }
            }

            static internal void begin()
            {
                int modulesCount = int.Parse(words[currentIndex++]);

                Console.WriteLine("Memory Map");

                for (int i = 0; i < modulesCount; i++)
                {
                    iterateDefinitions();

                    Dictionary<string, ImportedClass> importedSymbols = new Dictionary<string, ImportedClass>();
                    readImports(ref importedSymbols);

                    handleAddresses(importedSymbols);
                }

                Console.WriteLine();

                checkUsedDefinitions();
            }

            static void iterateDefinitions()
            {
                int symbolsCount = int.Parse(words[currentIndex++]);

                for (int i = 0; i < symbolsCount; i++)
                    currentIndex += 2;
            }

            static void readImports(ref Dictionary<string, ImportedClass> importedSymbols)
            {
                int importsCount = int.Parse(words[currentIndex++]);

                for (int i = 0; i < importsCount; i++)
                {
                    string symbol = words[currentIndex];
                    int relativeAddress = int.Parse(words[currentIndex + 1]);
                    bool defined = definedSymbols.ContainsKey(symbol);

                    importedSymbols.Add(symbol, new ImportedClass(relativeAddress, defined));

                    if (defined)
                        definedSymbols[symbol].Used = true;
                    currentIndex += 2;
                }
            }

            static void handleAddresses(Dictionary<string, ImportedClass> importedSymbols)
            {

                int addressesCount = int.Parse(words[currentIndex++]);

                string[] addresses = new string[addressesCount];

                for (int i = 0; i < addressesCount; i++)
                    addresses[i] = words[currentIndex++];

                handleAddressType4(importedSymbols, addresses);

                handleRemainingAddresses(addresses);

                printAddresses(addresses);

                absoluteIndex += addressesCount;
            }
            static char typeOf(string address)
            {
                if (address.Length == 5)
                    return address[address.Length - 1];
                return '0';
            }
            static void handleRemainingAddresses(string[] addresses)
            {
                for (int i = 0; i < addresses.Length; i++)
                {
                    string currentAddress = addresses[i];

                    if (typeOf(currentAddress) == '1' || typeOf(currentAddress) == '2')
                        currentAddress = currentAddress.Remove(currentAddress.Length - 1, 1);

                    else if (typeOf(currentAddress) == '3')
                    {
                        if (absoluteIndex < 200)
                        {
                            int newValue = getPointerAt(currentAddress) + absoluteIndex;
                            currentAddress = string.Format($"{currentAddress[0]}{newValue:D3}");
                        }
                        else
                        {
                            int newValue = getPointerAt(currentAddress) + 199; // max nr. of addresses is 200
                            currentAddress = string.Format($"{currentAddress[0]}{newValue:D3} Error: Absolute address over the address limit; used 199");
                        }
                    }

                    else if (typeOf(currentAddress) == '4')
                    {
                        currentAddress = currentAddress.Remove(currentAddress.Length - 1, 1);
                        currentAddress = $"{currentAddress} Error: E type address not on use chain; treated as I type.";
                    }

                    addresses[i] = currentAddress;
                }
            }

            static void handleAddressType4(Dictionary<string, ImportedClass> importedSymbols, string[] addresses)
            {
                // make a copy of the original array
                string[] originalAddresses = new string[addresses.Length];
                Array.Copy(addresses, originalAddresses, addresses.Length);

                foreach (var importedSymbol in importedSymbols)
                {
                    int pointer = importedSymbol.Value.RelativeAddress;

                    while (pointer != 777)
                    {
                        string aux = originalAddresses[pointer];
                        char firstDigit = originalAddresses[pointer][0];
                        int newAddress = definedSymbols[importedSymbol.Key].AbsoluteAddress;

                        // if not processed already
                        if (addresses[pointer].Length == 5)
                            if (importedSymbol.Value.Defined) // if import is defined
                                addresses[pointer] = string.Format($"{firstDigit}{newAddress:D3}");
                            else
                                addresses[pointer] = string.Format($"{firstDigit}000 Error: {importedSymbol.Key} is not defined; zero used.");
                        // if processed already
                        else
                            addresses[pointer] += " Error: Multiple symbols used by instruction; last used.";

                        if (typeOf(aux) == '1')
                            addresses[pointer] += " Error: Immediate address on use list; treated as External.";

                        pointer = getPointerAt(aux);
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

            static void checkUsedDefinitions()
            {
                foreach (var symbol in definedSymbols)
                    if (symbol.Value.Used == false)
                        Console.WriteLine($"Warning: {symbol.Key} was defined in module {symbol.Value.ModuleIdx} but never used.");
            }

        }

        static void Main(string[] args)
        {
            FirstStep.begin();
            SecondStep.begin();
        }
    }
}
