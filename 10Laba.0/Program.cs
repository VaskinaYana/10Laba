using System;
using System.Collections.Generic;

namespace CompilerLab
{
    public class Position
    {
        public int LineNumber { get; set; }

        public int CharNumber { get; set; }
    }

    public class CompilerError
    {
        public int Code { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }

        public string Message { get; set; } = string.Empty;
    }

    public static class ErrorTable
    {
        public static readonly Dictionary<int, string> Errors = new()
        {
            {100, "Ожидался идентификатор"},
            {101, "Повторное описание идентификатора"},
            {102, "Неописанный идентификатор"},
            {103, "Ожидался символ ';'"},
            {104, "Ожидался символ ','"},
            {105, "Ожидался символ ')'"},
            {106, "Ожидался символ '('"},
            {107, "Ожидался символ ']'"},
            {108, "Ожидался символ '['"},
            {109, "Недопустимый символ"},
            {110, "Ошибка описания массива"},
            {111, "Ошибка описания переменной"},
            {112, "Ошибка в выражении"},
            {113, "Несовместимые типы"},
            {114, "Ошибка оператора присваивания"},
            {115, "Ошибка условного оператора"},
            {116, "Ошибка оператора выбора"},
            {117, "Ошибка составного оператора"},
            {118, "Неожиданный конец файла"}
        };
    }

    public class InputOutputModule
    {
        private readonly string[] _lines =
        {
            "var a, b, c : integer;",
            "var arr : array[1..10] of integer;",
            "a := b + c;",
            "a := b + * c;",
            "@",
            "begin",
            "    a := 1;",
            "end;"
        };

        private readonly List<CompilerError> _errors = new();

        private int _currentLineIndex;

        private string _currentLine;

        public char Ch { get; private set; }

        public Position PositionNow { get; } = new();

        public InputOutputModule()
        {
            _currentLineIndex = 0;
            _currentLine = _lines[0];

            PositionNow.LineNumber = 1;
            PositionNow.CharNumber = 0;

            Ch = _currentLine[0];
        }

        public void NextCh()
        {
            if (_currentLineIndex >= _lines.Length)
            {
                Ch = '\0';
                return;
            }

            if (PositionNow.CharNumber >= _currentLine.Length - 1)
            {
                PrintCurrentLine();
                PrintErrorsForCurrentLine();

                _currentLineIndex++;

                if (_currentLineIndex >= _lines.Length)
                {
                    Ch = '\0';
                    return;
                }

                _currentLine = _lines[_currentLineIndex];

                PositionNow.LineNumber++;
                PositionNow.CharNumber = 0;
            }
            else
            {
                PositionNow.CharNumber++;
            }

            Ch = _currentLine.Length > 0
                ? _currentLine[PositionNow.CharNumber]
                : '\0';
        }

        public void AddError(int code)
        {
            _errors.Add(new CompilerError
            {
                Code = code,
                Line = PositionNow.LineNumber,
                Column = PositionNow.CharNumber + 1,
                Message = ErrorTable.Errors[code]
            });
        }

        private void PrintCurrentLine()
        {
            Console.WriteLine(
                $"{PositionNow.LineNumber:D2}  {_currentLine}");
        }

        private void PrintErrorsForCurrentLine()
        {
            foreach (CompilerError error in _errors)
            {
                if (error.Line == PositionNow.LineNumber)
                {
                    Console.Write("    ");

                    for (int i = 1; i < error.Column; i++)
                    {
                        Console.Write(' ');
                    }

                    Console.WriteLine("^");
                    Console.WriteLine($"Ошибка { error.Code}");
                    Console.WriteLine(error.Message);
                    Console.WriteLine();
                }
            }
        }

        public void PrintSummary()
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine(
                $"Компиляция окончена: ошибок - {_errors.Count}");
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            InputOutputModule io = new();

            while (io.Ch != '\0')
            {
                if (io.Ch == '*')
                {
                    io.AddError(112);
                }

                if (io.Ch == '@')
                {
                    io.AddError(109);
                }

                io.NextCh();
            }

            io.PrintSummary();

            Console.ReadKey();
        }
    }
}