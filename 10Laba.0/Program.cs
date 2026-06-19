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

                Console.WriteLine("\nРасшифровка кодов лексем:");
                Console.WriteLine("-------------------------");
                Console.WriteLine("122 = program, 2 = идентификатор, 14 = ;, 105 = var");
                Console.WriteLine("200 = integer, 201 = real, 202 = boolean");
                Console.WriteLine("113 = begin, 51 = :=, 70 = +, 56 = if, 66 = >");
                Console.WriteLine("52 = then, 32 = else, 31 = case, 101 = of, 104 = end");
                Console.WriteLine("114 = while, 65 = <, 54 = do, 15 = число, 9 = (, 4 = )");

                SyntaxAnalyzer syntaxAnalyzer = new SyntaxAnalyzer(lexicalAnalyzer);
                bool success = syntaxAnalyzer.Parse();

                if (success)
                {
                    Console.WriteLine("\n✓ Программа синтаксически верна!");
                }
                else
                {
                    Console.WriteLine("\n✗ Обнаружены синтаксические ошибки!");
                }

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