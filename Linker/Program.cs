using System;
using System.IO;
using System.Collections.Generic;

namespace Linker
{
    static class LinkerClass
    {
        static string path = @"../../IO/input-6";
        static string file = File.ReadAllText(path);

        static string[] words = file.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        //<key, (moduleIdx, absoluteAdress, used?, alreadyDefined?, outsideModule?)>
        static Dictionary<string, (int, int, bool, bool, bool)> definedSymbols = new Dictionary<string, (int, int, bool, bool, bool)>();

        static class firstStep
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

            static void definitionAlreadyUsed(string symbol)
            {
                int moduleIdx = definedSymbols[symbol].Item1;
                int absoluteAddress = definedSymbols[symbol].Item2;
                bool used = definedSymbols[symbol].Item3;
                bool alreadyDefined = true;
                bool outsideModule = definedSymbols[symbol].Item5;

                //<key, (moduleIdx, absoluteAdress, used?, alreadyDefined?, outsideModule?)>
                definedSymbols[symbol] = (moduleIdx, absoluteAddress, used, alreadyDefined, outsideModule);
            }

            static void defineSymbols(int moduleIdx, List<string> currentSymbolsInModule)
            {
                int definitionsCount = int.Parse(words[currentIndex++]);

                for (int i = 0; i < definitionsCount; i++)
                {
                    string symbol = words[currentIndex];
                    int absoluteAddress = int.Parse(words[currentIndex + 1]) + absoluteIndex;

                    if (definedSymbols.ContainsKey(symbol))
                        definitionAlreadyUsed(symbol);
                    else
                    {
                        //<key, (moduleIdx, absoluteAdress, used?, alreadyDefined?, outsideModule)>
                        definedSymbols.Add(symbol, (moduleIdx, absoluteAddress, false, false, false));

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

            static void setRelativeAddress0(string symbol, int absoluteIndex)
            {
                int moduleIdx = definedSymbols[symbol].Item1;
                int absoluteAddress = absoluteIndex;
                bool used = definedSymbols[symbol].Item3;
                bool alreadyDefined = definedSymbols[symbol].Item4;
                bool outsideModule = true;

                //<key, (moduleIdx, absoluteAdress, used?, alreadyDefined?, outsideModule?)>
                definedSymbols[symbol] = (moduleIdx, absoluteAddress, used, alreadyDefined, outsideModule);
            }

            static void iterateAddresses(List<string> currentSymbolsInModule)
            {
                int addressesCount = int.Parse(words[currentIndex++]);


                foreach (var symbol in currentSymbolsInModule)
                    if (definedSymbols[symbol].Item2 - absoluteIndex >= addressesCount)
                        setRelativeAddress0(symbol, absoluteIndex);

                absoluteIndex += addressesCount;

                for (int i = 0; i < addressesCount; i++)
                    currentIndex++;
            }

            static string checkIfAlreadyDefined(bool alreadyDefined)
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
                    Console.WriteLine($"{symbol.Key}={symbol.Value.Item2}{checkIfAlreadyDefined(symbol.Value.Item4)}{checkIfOutsideModule(symbol.Key, symbol.Value.Item5)}");

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

                    // <key, (relativeAddress, defined?)>
                    Dictionary<string, (int, bool)> importedSymbols = new Dictionary<string, (int, bool)>();
                    readImports(ref importedSymbols);

                    handleAddresses(importedSymbols);
                }

                checkUsedDefinitions();

                Console.WriteLine();

            }

            static void iterateDefinitions()
            {
                int symbolsCount = int.Parse(words[currentIndex++]);

                for (int i = 0; i < symbolsCount; i++)
                    currentIndex += 2;
            }
            static void usedSymbol(string symbol)
            {
                int moduleIdx = definedSymbols[symbol].Item1;
                int absoluteAdress = definedSymbols[symbol].Item2;
                bool alreadyDefined = definedSymbols[symbol].Item4;
                bool outsideModule = definedSymbols[symbol].Item5;

                definedSymbols[symbol] = (moduleIdx, absoluteAdress, true, alreadyDefined, outsideModule);
            }
            static void readImports(ref Dictionary<string, (int, bool)> importedSymbols)
            {
                int importsCount = int.Parse(words[currentIndex++]);

                for (int i = 0; i < importsCount; i++)
                {
                    string symbol = words[currentIndex];
                    int relativeAddress = int.Parse(words[currentIndex + 1]);
                    bool defined = definedSymbols.ContainsKey(symbol);

                    importedSymbols.Add(symbol, (relativeAddress, defined));

                    if (defined)
                        usedSymbol(symbol);

                    currentIndex += 2;
                }
            }
            static void handleAddresses(Dictionary<string, (int, bool)> importedSymbols)
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

            static void handleAddressType4(Dictionary<string, (int, bool)> importedSymbols, string[] addresses)
            {
                // make a copy of the original array
                string[] originalAddresses = new string[addresses.Length];
                Array.Copy(addresses, originalAddresses, addresses.Length);

                foreach (var importedSymbol in importedSymbols)
                {
                    int pointer = importedSymbol.Value.Item1;

                    while (pointer != 777)
                    {
                        string aux = originalAddresses[pointer];
                        char firstDigit = originalAddresses[pointer][0];
                        int newAddress = definedSymbols[importedSymbol.Key].Item2;

                        // if not processed already
                        if (addresses[pointer].Length == 5)
                            if (importedSymbol.Value.Item2) // if import is defined
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
                Console.WriteLine();
                foreach (var symbol in definedSymbols)
                    if (symbol.Value.Item3 == false)
                        Console.WriteLine($"Warning: {symbol.Key} was defined in module {symbol.Value.Item1} but never used.");
            }

        }

        static void Main(string[] args)
        {
            firstStep.begin();
            secondStep.begin();
        }
    }
}
