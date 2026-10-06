using _10Laba._0;
using System;

namespace _10Laba._0
{
    public static class Program
    {
        public static void Main()
        {
            string[] sourceLines = new string[]
            {
                "program test;",
                "var",
                "    a, b, c : integer;",
                "    arr : array[1..10] of integer;",
                "    x : real;",
                "    flag : boolean;",
                "const",
                "    MAX = 100;",
                "    MIN = 1;",
                "begin",
                "    a := b + c;",
                "    arr[1] := 5;",
                "    if a > 10 then",
                "        b := 1",
                "    else",
                "        b := 2;",
                "    case a of",
                "        1: x := 1.5;",
                "        2: x := 2.5;",
                "        3: x := 3.5",
                "    end;",
                "    while a < MAX do",
                "        begin",
                "            a := a + 1;",
                "            b := b * 2",
                "        end;",
                "    c := (a + b) * 2;",
                "    c := a + b * 3;",
                "    c := (a + b) * (c - 3);",
                "end."
            };

            try
            {
                InputOutput.Initialize(sourceLines);
                LexicalAnalyzer lexicalAnalyzer = new LexicalAnalyzer();
                Console.WriteLine("\nИсходный код:");
                Console.WriteLine("-------------");
                lexicalAnalyzer.Analyze();
                Console.WriteLine("\nКоды лексем:");
                Console.WriteLine("-------------");
                lexicalAnalyzer.PrintTokens();

                SyntaxAnalyzer syntaxAnalyzer = new SyntaxAnalyzer(lexicalAnalyzer);
                bool success = syntaxAnalyzer.Parse();
                Console.WriteLine(success ? "\n✓ Программа синтаксически верна!" : "\n✗ Обнаружены синтаксические ошибки!");
                InputOutput.PrintSummary();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}