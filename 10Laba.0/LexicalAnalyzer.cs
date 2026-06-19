using _10Laba._0;
using System;
using System.Collections.Generic;

namespace _10Laba._0
{
    public class LexicalAnalyzer
    {
        public const byte Star = 21;
        public const byte Slash = 60;
        public const byte Equal = 16;
        public const byte Comma = 20;
        public const byte Semicolon = 14;
        public const byte Colon = 5;
        public const byte Point = 61;
        public const byte Arrow = 62;
        public const byte LeftPar = 9;
        public const byte RightPar = 4;
        public const byte Lbracket = 11;
        public const byte Rbracket = 12;
        public const byte Flpar = 63;
        public const byte Frpar = 64;
        public const byte Later = 65;
        public const byte Greater = 66;
        public const byte LaterEqual = 67;
        public const byte GreaterEqual = 68;
        public const byte LaterGreater = 69;
        public const byte Plus = 70;
        public const byte Minus = 71;
        public const byte Lcomment = 72;
        public const byte Rcomment = 73;
        public const byte Assign = 51;
        public const byte TwoPoints = 74;
        public const byte Ident = 2;
        public const byte FloatConst = 82;
        public const byte IntConst = 15;

        public const byte CaseSy = 31;
        public const byte ElseSy = 32;
        public const byte FileSy = 57;
        public const byte GotoSy = 33;
        public const byte ThenSy = 52;
        public const byte TypeSy = 34;
        public const byte UntilSy = 53;
        public const byte DoSy = 54;
        public const byte WithSy = 37;
        public const byte IfSy = 56;
        public const byte InSy = 100;
        public const byte OfSy = 101;
        public const byte OrSy = 102;
        public const byte ToSy = 103;
        public const byte EndSy = 104;
        public const byte VarSy = 105;
        public const byte DivSy = 106;
        public const byte AndSy = 107;
        public const byte NotSy = 108;
        public const byte ForSy = 109;
        public const byte ModSy = 110;
        public const byte NilSy = 111;
        public const byte SetSy = 112;
        public const byte BeginSy = 113;
        public const byte WhileSy = 114;
        public const byte ArraySy = 115;
        public const byte ConstSy = 116;
        public const byte LabelSy = 117;
        public const byte DownToSy = 118;
        public const byte PackedSy = 119;
        public const byte RecordSy = 120;
        public const byte RepeatSy = 121;
        public const byte ProgramSy = 122;
        public const byte FunctionSy = 123;
        public const byte ProcedureSy = 124;

        public const byte IntegerType = 200;
        public const byte RealType = 201;
        public const byte BooleanType = 202;
        public const byte CharType = 203;
        public const byte StringType = 204;

        public List<int> Tokens { get; } = new List<int>();

        private byte _symbol;
        private string _identifierName = string.Empty;
        private int _intValue;
        private float _floatValue;
        private readonly Keywords _keywords;

        public LexicalAnalyzer()
        {
            _keywords = new Keywords();
        }

        public byte NextSym()
        {
            while (InputOutput.Ch == ' ' || InputOutput.Ch == '\t')
            {
                InputOutput.NextCh();
            }

            TextPosition currentPosition = new TextPosition(
                InputOutput.PositionNow.LineNumber,
                InputOutput.PositionNow.CharNumber);

            char ch = InputOutput.Ch;

            if (IsLetter(ch))
            {
                _identifierName = string.Empty;

                while (IsLetter(InputOutput.Ch) || IsDigit(InputOutput.Ch))
                {
                    _identifierName += InputOutput.Ch;
                    InputOutput.NextCh();
                }

                string lowerName = _identifierName.ToLower();
                switch (lowerName)
                {
                    case "integer": _symbol = IntegerType; Tokens.Add(_symbol); return _symbol;
                    case "real": _symbol = RealType; Tokens.Add(_symbol); return _symbol;
                    case "boolean": _symbol = BooleanType; Tokens.Add(_symbol); return _symbol;
                    case "char": _symbol = CharType; Tokens.Add(_symbol); return _symbol;
                    case "string": _symbol = StringType; Tokens.Add(_symbol); return _symbol;
                    default:
                        byte? keywordCode = _keywords.GetKeywordCode(_identifierName);
                        _symbol = keywordCode ?? Ident;
                        Tokens.Add(_symbol);
                        return _symbol;
                }
            }

            if (IsDigit(ch))
            {
                _intValue = 0;
                int maxInt = short.MaxValue;

                while (IsDigit(InputOutput.Ch))
                {
                    byte digit = (byte)(InputOutput.Ch - '0');

                    if (_intValue < maxInt / 10 || (_intValue == maxInt / 10 && digit <= maxInt % 10))
                    {
                        _intValue = 10 * _intValue + digit;
                    }
                    else
                    {
                        InputOutput.Error(203, InputOutput.PositionNow);
                        _intValue = 0;
                        while (IsDigit(InputOutput.Ch))
                        {
                            InputOutput.NextCh();
                        }
                        _symbol = IntConst;
                        Tokens.Add(_symbol);
                        return _symbol;
                    }

                    InputOutput.NextCh();
                }

                // Проверка на вещественное число
                if (InputOutput.Ch == '.')
                {
                    char nextCh = InputOutput.PeekNextCh();

                    // Если следующий символ тоже точка - это оператор ".."
                    if (nextCh == '.')
                    {
                        _symbol = IntConst;
                        Tokens.Add(_symbol);
                        return _symbol;
                    }

                    InputOutput.NextCh();

                    float floatValue = _intValue;
                    float floatDiv = 10;

                    while (IsDigit(InputOutput.Ch))
                    {
                        byte digit = (byte)(InputOutput.Ch - '0');
                        floatValue = floatValue + digit / floatDiv;
                        floatDiv *= 10;
                        InputOutput.NextCh();
                    }

                    _symbol = FloatConst;
                    Tokens.Add(_symbol);
                    return _symbol;
                }

                _symbol = IntConst;
                Tokens.Add(_symbol);
                return _symbol;
            }

            switch (ch)
            {
                case '<':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=')
                    {
                        _symbol = LaterEqual;
                        InputOutput.NextCh();
                    }
                    else if (InputOutput.Ch == '>')
                    {
                        _symbol = LaterGreater;
                        InputOutput.NextCh();
                    }
                    else
                    {
                        _symbol = Later;
                    }
                    Tokens.Add(_symbol);
                    return _symbol;

                case '>':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=')
                    {
                        _symbol = GreaterEqual;
                        InputOutput.NextCh();
                    }
                    else
                    {
                        _symbol = Greater;
                    }
                    Tokens.Add(_symbol);
                    return _symbol;

                case ':':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=')
                    {
                        _symbol = Assign;
                        InputOutput.NextCh();
                    }
                    else
                    {
                        _symbol = Colon;
                    }
                    Tokens.Add(_symbol);
                    return _symbol;

                case ';':
                    _symbol = Semicolon;
                    InputOutput.NextCh();
                    Tokens.Add(_symbol);
                    return _symbol;

                case '.':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '.')
                    {
                        _symbol = TwoPoints;
                        InputOutput.NextCh();
                    }
                    else
                    {
                        _symbol = Point;
                    }
                    Tokens.Add(_symbol);
                    return _symbol;

                case ',':
                    _symbol = Comma;
                    InputOutput.NextCh();
                    Tokens.Add(_symbol);
                    return _symbol;

                case '=':
                    _symbol = Equal;
                    InputOutput.NextCh();
                    Tokens.Add(_symbol);
                    return _symbol;

                case '+':
                    _symbol = Plus;
                    InputOutput.NextCh();
                    Tokens.Add(_symbol);
                    return _symbol;

                case '-':
                    _symbol = Minus;
                    InputOutput.NextCh();
                    Tokens.Add(_symbol);
                    return _symbol;

                case '*':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == ')')
                    {
                        _symbol = Rcomment;
                        InputOutput.NextCh();
                    }
                    else
                    {
                        _symbol = Star;
                    }
                    Tokens.Add(_symbol);
                    return _symbol;

                case '/':
                    _symbol = Slash;
                    InputOutput.NextCh();
                    Tokens.Add(_symbol);
                    return _symbol;

                case '(':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '*')
                    {
                        _symbol = Lcomment;
                        InputOutput.NextCh();
                    }
                    else
                    {
                        _symbol = LeftPar;
                    }
                    Tokens.Add(_symbol);
                    return _symbol;

                case ')':
                    _symbol = RightPar;
                    InputOutput.NextCh();
                    Tokens.Add(_symbol);
                    return _symbol;

                case '[':
                    _symbol = Lbracket;
                    InputOutput.NextCh();
                    Tokens.Add(_symbol);
                    return _symbol;

                case ']':
                    _symbol = Rbracket;
                    InputOutput.NextCh();
                    Tokens.Add(_symbol);
                    return _symbol;

                case '{':
                    _symbol = Flpar;
                    InputOutput.NextCh();
                    Tokens.Add(_symbol);
                    return _symbol;

                case '}':
                    _symbol = Frpar;
                    InputOutput.NextCh();
                    Tokens.Add(_symbol);
                    return _symbol;

                case '^':
                    _symbol = Arrow;
                    InputOutput.NextCh();
                    Tokens.Add(_symbol);
                    return _symbol;

                case '\0':
                    _symbol = 0;
                    Tokens.Add(0);
                    return 0;

                default:
                    InputOutput.Error(109, currentPosition);
                    InputOutput.NextCh();
                    _symbol = 0;
                    Tokens.Add(0);
                    return 0;
            }
        }

        private static bool IsLetter(char ch)
        {
            return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z');
        }

        private static bool IsDigit(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        public void Analyze()
        {
            Tokens.Clear();

            while (true)
            {
                byte token = NextSym();
                if (token == 0 && InputOutput.Ch == '\0')
                {
                    break;
                }
            }
        }

        public void PrintTokens()
        {
            Console.WriteLine("Коды лексем:");
            string result = "";
            int count = 0;
            foreach (int token in Tokens)
            {
                result += token + " ";
                count++;
                if (count % 20 == 0)
                {
                    result += "\n";
                }
            }
            Console.WriteLine(result);
        }
    }
}