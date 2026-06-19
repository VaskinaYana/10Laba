using System;
using System.Collections.Generic;

namespace _10Laba._0
{
    public struct TextPosition
    {
        public uint LineNumber;
        public byte CharNumber;

        public TextPosition(uint lineNumber = 0, byte charNumber = 0)
        {
            this.LineNumber = lineNumber;
            this.CharNumber = charNumber;
        }
    }

    public struct Err
    {
        public TextPosition ErrorPosition;
        public byte ErrorCode;

        public Err(TextPosition errorPosition, byte errorCode)
        {
            this.ErrorPosition = errorPosition;
            this.ErrorCode = errorCode;
        }
    }

    public static class InputOutput
    {
        private const byte ErrMax = 9;
        private static uint _errorCount = 0;
        private static string _currentLine = string.Empty;
        private static byte _lastInLine = 0;
        private static string[] _sourceLines;
        private static int _currentLineIndex = 0;

        public static char Ch { get; private set; }
        public static TextPosition PositionNow { get; private set; }
        public static List<Err> Errors { get; private set; }

        public static void Initialize(string[] sourceLines)
        {
            _sourceLines = sourceLines;
            _currentLineIndex = 0;
            PositionNow = new TextPosition(1, 0);
            Errors = new List<Err>();
            _errorCount = 0;

            if (_sourceLines.Length > 0 && _sourceLines[0].Length > 0)
            {
                _currentLine = _sourceLines[0];
                _lastInLine = (byte)(_currentLine.Length - 1);
                Ch = _currentLine[0];
            }
            else
            {
                _currentLine = string.Empty;
                _lastInLine = 0;
                Ch = '\0';
            }
        }

        public static char PeekNextCh()
        {
            if (_currentLineIndex >= _sourceLines.Length)
            {
                return '\0';
            }

            int nextPos = PositionNow.CharNumber + 1;
            if (nextPos >= _currentLine.Length)
            {
                // Следующий символ в следующей строке
                if (_currentLineIndex + 1 < _sourceLines.Length)
                {
                    string nextLine = _sourceLines[_currentLineIndex + 1];
                    if (nextLine.Length > 0)
                    {
                        return nextLine[0];
                    }
                }
                return '\0';
            }
            return _currentLine[nextPos];
        }

        public static void NextCh()
        {
            if (_currentLineIndex >= _sourceLines.Length)
            {
                Ch = '\0';
                return;
            }

            if (PositionNow.CharNumber == _lastInLine)
            {
                PrintCurrentLine();
                PrintErrorsForCurrentLine();

                _currentLineIndex++;

                if (_currentLineIndex >= _sourceLines.Length)
                {
                    Ch = '\0';
                    return;
                }

                _currentLine = _sourceLines[_currentLineIndex];
                _lastInLine = (byte)(_currentLine.Length > 0 ? _currentLine.Length - 1 : 0);
                PositionNow = new TextPosition(PositionNow.LineNumber + 1, 0);
                Errors = new List<Err>();

                if (_currentLine.Length > 0)
                {
                    Ch = _currentLine[0];
                }
                else
                {
                    Ch = '\0';
                }
            }
            else
            {
                PositionNow = new TextPosition(PositionNow.LineNumber, (byte)(PositionNow.CharNumber + 1));
                Ch = _currentLine[PositionNow.CharNumber];
            }
        }

        public static void Error(byte errorCode, TextPosition position)
        {
            if (Errors.Count <= ErrMax)
            {
                Errors.Add(new Err(position, errorCode));
            }
        }

        public static void PrintSummary()
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"Компиляция завершена: ошибок — {_errorCount}!");
        }

        private static void PrintCurrentLine()
        {
            Console.WriteLine($"{PositionNow.LineNumber,2}  {_currentLine}");
        }

        private static void PrintErrorsForCurrentLine()
        {
            int positionStringLength = 6 - $"{PositionNow.LineNumber} ".Length;

            foreach (Err error in Errors)
            {
                if (error.ErrorPosition.LineNumber != PositionNow.LineNumber)
                {
                    continue;
                }

                _errorCount++;

                string errorPrefix = "**";
                if (_errorCount < 10)
                {
                    errorPrefix += "0";
                }
                errorPrefix += $"{_errorCount}**";

                while (errorPrefix.Length - 1 < positionStringLength + error.ErrorPosition.CharNumber)
                {
                    errorPrefix += " ";
                }

                errorPrefix += $"^ ошибка код {error.ErrorCode}";

                Console.WriteLine(errorPrefix);

                if (ErrorTable.Errors.TryGetValue(error.ErrorCode, out string message))
                {
                    Console.WriteLine($"   {message}");
                }

                Console.WriteLine();
            }
        }
    }

    public static class ErrorTable
    {
        public static readonly Dictionary<byte, string> Errors = new Dictionary<byte, string>()
        {
            { 100, "Ожидался идентификатор" },
            { 101, "Повторное описание идентификатора" },
            { 102, "Неописанный идентификатор" },
            { 103, "Ожидался символ ';'" },
            { 104, "Ожидался символ ','" },
            { 105, "Ожидался символ ')'" },
            { 106, "Ожидался символ '('" },
            { 107, "Ожидался символ ']'" },
            { 108, "Ожидался символ '['" },
            { 109, "Недопустимый символ" },
            { 110, "Ошибка описания массива" },
            { 111, "Ошибка описания переменной" },
            { 112, "Ошибка в выражении" },
            { 113, "Несовместимые типы" },
            { 114, "Ошибка оператора присваивания" },
            { 115, "Ошибка условного оператора" },
            { 116, "Ошибка оператора выбора" },
            { 117, "Ошибка составного оператора" },
            { 118, "Неожиданный конец файла" },
            { 203, "Целая константа превышает допустимый диапазон" }
        };
    }
}