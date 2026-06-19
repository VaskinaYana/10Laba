using _10Laba._0;
using System;
using System.Collections.Generic;

namespace _10Laba._0
{
    public class SyntaxAnalyzer
    {
        private readonly LexicalAnalyzer _lexicalAnalyzer;
        private int _currentTokenIndex;
        private readonly List<int> _tokens;
        private int _currentToken;
        private bool _hasError;
        private int _errorCount;

        public SyntaxAnalyzer(LexicalAnalyzer lexicalAnalyzer)
        {
            _lexicalAnalyzer = lexicalAnalyzer;
            _tokens = lexicalAnalyzer.Tokens;
            _currentTokenIndex = 0;
            _hasError = false;
            _errorCount = 0;
            if (_tokens.Count > 0)
            {
                _currentToken = _tokens[0];
            }
            else
            {
                _currentToken = 0;
            }
        }

        private void NextToken()
        {
            _currentTokenIndex++;
            if (_currentTokenIndex < _tokens.Count)
            {
                _currentToken = _tokens[_currentTokenIndex];
            }
            else
            {
                _currentToken = 0;
            }
        }

        private void Error(string message)
        {
            _hasError = true;
            _errorCount++;
            Console.WriteLine($"Синтаксическая ошибка #{_errorCount}: {message} на позиции {_currentTokenIndex}");
        }

        private bool Match(byte expectedToken)
        {
            if (_currentToken == expectedToken)
            {
                NextToken();
                return true;
            }
            else
            {
                Error($"Ожидался код {expectedToken}, получен {_currentToken}");
                SkipToSynchronization();
                return false;
            }
        }

        private void SkipToSynchronization()
        {
            while (_currentToken != LexicalAnalyzer.Semicolon &&
                   _currentToken != LexicalAnalyzer.EndSy &&
                   _currentToken != LexicalAnalyzer.BeginSy &&
                   _currentToken != 0)
            {
                NextToken();
            }
            if (_currentToken == LexicalAnalyzer.Semicolon)
            {
                NextToken();
            }
        }

        public bool Parse()
        {
            Console.WriteLine("\nНачало синтаксического анализа...");
            Program();
            if (!_hasError && _currentToken == 0)
            {
                Console.WriteLine("Синтаксический анализ успешно завершен!");
            }
            else if (!_hasError && _currentToken != 0)
            {
                Console.WriteLine("Синтаксический анализ завершен с незавершенными токенами.");
            }
            else
            {
                Console.WriteLine($"Синтаксический анализ завершен с {_errorCount} ошибками.");
            }
            return !_hasError;
        }

        private void Program()
        {
            if (_currentToken == LexicalAnalyzer.ProgramSy)
            {
                Match(LexicalAnalyzer.ProgramSy);
                if (_currentToken == LexicalAnalyzer.Ident)
                {
                    Match(LexicalAnalyzer.Ident);
                }
                else
                {
                    Error("Ожидалось имя программы");
                }
                Match(LexicalAnalyzer.Semicolon);
            }
            else
            {
                Error("Ожидалось ключевое слово 'program'");
            }

            Block();
            Match(LexicalAnalyzer.Point);
        }

        private void Block()
        {
            if (_currentToken == LexicalAnalyzer.VarSy)
            {
                Match(LexicalAnalyzer.VarSy);
                VariableDeclarations();
            }

            if (_currentToken == LexicalAnalyzer.ConstSy)
            {
                Match(LexicalAnalyzer.ConstSy);
                ConstDeclarations();
            }

            if (_currentToken == LexicalAnalyzer.BeginSy)
            {
                CompoundStatement();
            }
            else
            {
                Error("Ожидалось ключевое слово 'begin'");
            }
        }

        private void VariableDeclarations()
        {
            while (_currentToken == LexicalAnalyzer.Ident)
            {
                VariableDeclaration();
                if (_currentToken == LexicalAnalyzer.Semicolon)
                {
                    Match(LexicalAnalyzer.Semicolon);
                }
                else
                {
                    Error("Ожидалась точка с запятой после объявления переменной");
                    break;
                }
            }
        }

        private void VariableDeclaration()
        {
            IdentifierList();
            Match(LexicalAnalyzer.Colon);
            TypeDeclaration();
        }

        private void TypeDeclaration()
        {
            if (_currentToken == LexicalAnalyzer.IntegerType ||
                _currentToken == LexicalAnalyzer.RealType ||
                _currentToken == LexicalAnalyzer.BooleanType ||
                _currentToken == LexicalAnalyzer.CharType ||
                _currentToken == LexicalAnalyzer.StringType)
            {
                Match((byte)_currentToken);
            }
            else if (_currentToken == LexicalAnalyzer.ArraySy)
            {
                ArrayType();
            }
            else if (_currentToken == LexicalAnalyzer.Ident)
            {
                Match(LexicalAnalyzer.Ident);
            }
            else
            {
                Error("Ожидалось имя типа или ключевое слово 'array'");
                SkipToSynchronization();
            }
        }

        private void ArrayType()
        {
            Match(LexicalAnalyzer.ArraySy);
            Match(LexicalAnalyzer.Lbracket);

            if (_currentToken == LexicalAnalyzer.IntConst || _currentToken == LexicalAnalyzer.Ident)
            {
                Match((byte)_currentToken);
            }
            else
            {
                Error("Ожидалась нижняя граница массива");
            }

            Match(LexicalAnalyzer.TwoPoints);

            if (_currentToken == LexicalAnalyzer.IntConst || _currentToken == LexicalAnalyzer.Ident)
            {
                Match((byte)_currentToken);
            }
            else
            {
                Error("Ожидалась верхняя граница массива");
            }

            Match(LexicalAnalyzer.Rbracket);
            Match(LexicalAnalyzer.OfSy);

            if (_currentToken == LexicalAnalyzer.IntegerType ||
                _currentToken == LexicalAnalyzer.RealType ||
                _currentToken == LexicalAnalyzer.BooleanType ||
                _currentToken == LexicalAnalyzer.CharType ||
                _currentToken == LexicalAnalyzer.StringType ||
                _currentToken == LexicalAnalyzer.Ident)
            {
                Match((byte)_currentToken);
            }
            else
            {
                Error("Ожидался тип элементов массива");
                SkipToSynchronization();
            }
        }

        private void IdentifierList()
        {
            Match(LexicalAnalyzer.Ident);
            while (_currentToken == LexicalAnalyzer.Comma)
            {
                Match(LexicalAnalyzer.Comma);
                Match(LexicalAnalyzer.Ident);
            }
        }

        private void ConstDeclarations()
        {
            while (_currentToken == LexicalAnalyzer.Ident)
            {
                Match(LexicalAnalyzer.Ident);
                Match(LexicalAnalyzer.Equal);

                if (_currentToken == LexicalAnalyzer.IntConst ||
                    _currentToken == LexicalAnalyzer.FloatConst ||
                    _currentToken == LexicalAnalyzer.Ident)
                {
                    NextToken();
                }
                else
                {
                    Error("Ожидалась константа");
                }

                if (_currentToken == LexicalAnalyzer.Semicolon)
                {
                    Match(LexicalAnalyzer.Semicolon);
                }
                else
                {
                    Error("Ожидалась точка с запятой после объявления константы");
                    break;
                }
            }
        }

        private void CompoundStatement()
        {
            if (_currentToken == LexicalAnalyzer.BeginSy)
            {
                Match(LexicalAnalyzer.BeginSy);
                StatementList();
                Match(LexicalAnalyzer.EndSy);
            }
            else
            {
                Error("Ожидалось ключевое слово 'begin'");
            }
        }

        private void StatementList()
        {
            if (IsStatementStart())
            {
                Statement();
                while (_currentToken == LexicalAnalyzer.Semicolon)
                {
                    Match(LexicalAnalyzer.Semicolon);
                    if (IsStatementStart())
                    {
                        Statement();
                    }
                }
            }
        }

        private bool IsStatementStart()
        {
            return _currentToken == LexicalAnalyzer.Ident ||
                   _currentToken == LexicalAnalyzer.BeginSy ||
                   _currentToken == LexicalAnalyzer.IfSy ||
                   _currentToken == LexicalAnalyzer.CaseSy ||
                   _currentToken == LexicalAnalyzer.WhileSy ||
                   _currentToken == LexicalAnalyzer.ForSy ||
                   _currentToken == LexicalAnalyzer.RepeatSy;
        }

        private void Statement()
        {
            if (_currentToken == LexicalAnalyzer.Ident)
            {
                AssignmentStatement();
            }
            else if (_currentToken == LexicalAnalyzer.BeginSy)
            {
                CompoundStatement();
            }
            else if (_currentToken == LexicalAnalyzer.IfSy)
            {
                IfStatement();
            }
            else if (_currentToken == LexicalAnalyzer.CaseSy)
            {
                CaseStatement();
            }
            else if (_currentToken == LexicalAnalyzer.WhileSy)
            {
                WhileStatement();
            }
            else if (_currentToken == LexicalAnalyzer.ForSy)
            {
                ForStatement();
            }
            else if (_currentToken == LexicalAnalyzer.RepeatSy)
            {
                RepeatStatement();
            }
            else
            {
                Error("Ожидался оператор");
                SkipToSynchronization();
            }
        }

        private void AssignmentStatement()
        {
            if (_currentToken == LexicalAnalyzer.Ident)
            {
                Match(LexicalAnalyzer.Ident);

                if (_currentToken == LexicalAnalyzer.Lbracket)
                {
                    Match(LexicalAnalyzer.Lbracket);
                    Expression();
                    Match(LexicalAnalyzer.Rbracket);
                }

                if (_currentToken == LexicalAnalyzer.Assign)
                {
                    Match(LexicalAnalyzer.Assign);
                    Expression();
                }
                else
                {
                    Error("Ожидался оператор присваивания ':='");
                }
            }
            else
            {
                Error("Ожидался идентификатор в операторе присваивания");
            }
        }

        private void IfStatement()
        {
            Match(LexicalAnalyzer.IfSy);
            Expression();
            if (_currentToken == LexicalAnalyzer.ThenSy)
            {
                Match(LexicalAnalyzer.ThenSy);
                Statement();
                if (_currentToken == LexicalAnalyzer.ElseSy)
                {
                    Match(LexicalAnalyzer.ElseSy);
                    Statement();
                }
            }
            else
            {
                Error("Ожидалось ключевое слово 'then'");
            }
        }

        private void CaseStatement()
        {
            Match(LexicalAnalyzer.CaseSy);
            Expression();
            if (_currentToken == LexicalAnalyzer.OfSy)
            {
                Match(LexicalAnalyzer.OfSy);
                CaseList();
                if (_currentToken == LexicalAnalyzer.EndSy)
                {
                    Match(LexicalAnalyzer.EndSy);
                }
                else
                {
                    Error("Ожидалось ключевое слово 'end' для case");
                }
            }
            else
            {
                Error("Ожидалось ключевое слово 'of'");
            }
        }

        private void CaseList()
        {
            while (_currentToken == LexicalAnalyzer.IntConst ||
                   _currentToken == LexicalAnalyzer.FloatConst ||
                   _currentToken == LexicalAnalyzer.Ident)
            {
                NextToken();

                if (_currentToken == LexicalAnalyzer.Colon)
                {
                    Match(LexicalAnalyzer.Colon);

                    if (_currentToken == LexicalAnalyzer.Ident)
                    {
                        AssignmentStatement();
                    }
                    else if (_currentToken == LexicalAnalyzer.BeginSy)
                    {
                        CompoundStatement();
                    }
                    else
                    {
                        Error("Ожидался оператор после ':' в case");
                        SkipToSynchronization();
                    }

                    if (_currentToken == LexicalAnalyzer.Semicolon)
                    {
                        Match(LexicalAnalyzer.Semicolon);
                    }
                    else if (_currentToken == LexicalAnalyzer.EndSy)
                    {
                        break;
                    }
                }
                else
                {
                    Error("Ожидалось ':' после константы выбора");
                    break;
                }
            }
        }

        private void WhileStatement()
        {
            Match(LexicalAnalyzer.WhileSy);
            Expression();
            if (_currentToken == LexicalAnalyzer.DoSy)
            {
                Match(LexicalAnalyzer.DoSy);
                Statement();
            }
            else
            {
                Error("Ожидалось ключевое слово 'do'");
            }
        }

        private void ForStatement()
        {
            Match(LexicalAnalyzer.ForSy);
            if (_currentToken == LexicalAnalyzer.Ident)
            {
                Match(LexicalAnalyzer.Ident);
                Match(LexicalAnalyzer.Assign);
                Expression();
                if (_currentToken == LexicalAnalyzer.ToSy || _currentToken == LexicalAnalyzer.DownToSy)
                {
                    NextToken();
                    Expression();
                    if (_currentToken == LexicalAnalyzer.DoSy)
                    {
                        Match(LexicalAnalyzer.DoSy);
                        Statement();
                    }
                    else
                    {
                        Error("Ожидалось ключевое слово 'do'");
                    }
                }
                else
                {
                    Error("Ожидалось 'to' или 'downto'");
                }
            }
            else
            {
                Error("Ожидался идентификатор в for");
            }
        }

        private void RepeatStatement()
        {
            Match(LexicalAnalyzer.RepeatSy);
            StatementList();
            if (_currentToken == LexicalAnalyzer.UntilSy)
            {
                Match(LexicalAnalyzer.UntilSy);
                Expression();
            }
            else
            {
                Error("Ожидалось ключевое слово 'until'");
            }
        }

        private void Expression()
        {
            SimpleExpression();
            if (_currentToken == LexicalAnalyzer.Equal ||
                _currentToken == LexicalAnalyzer.Later ||
                _currentToken == LexicalAnalyzer.Greater ||
                _currentToken == LexicalAnalyzer.LaterEqual ||
                _currentToken == LexicalAnalyzer.GreaterEqual ||
                _currentToken == LexicalAnalyzer.LaterGreater)
            {
                NextToken();
                SimpleExpression();
            }
        }

        private void SimpleExpression()
        {
            if (_currentToken == LexicalAnalyzer.Plus || _currentToken == LexicalAnalyzer.Minus)
            {
                NextToken();
            }

            Term();
            while (_currentToken == LexicalAnalyzer.Plus ||
                   _currentToken == LexicalAnalyzer.Minus ||
                   _currentToken == LexicalAnalyzer.OrSy)
            {
                NextToken();
                Term();
            }
        }

        private void Term()
        {
            Factor();
            while (_currentToken == LexicalAnalyzer.Star ||
                   _currentToken == LexicalAnalyzer.Slash ||
                   _currentToken == LexicalAnalyzer.DivSy ||
                   _currentToken == LexicalAnalyzer.ModSy ||
                   _currentToken == LexicalAnalyzer.AndSy)
            {
                NextToken();
                Factor();
            }
        }

        private void Factor()
        {
            if (_currentToken == LexicalAnalyzer.Ident)
            {
                Match(LexicalAnalyzer.Ident);
                if (_currentToken == LexicalAnalyzer.Lbracket)
                {
                    Match(LexicalAnalyzer.Lbracket);
                    Expression();
                    Match(LexicalAnalyzer.Rbracket);
                }
            }
            else if (_currentToken == LexicalAnalyzer.IntConst ||
                     _currentToken == LexicalAnalyzer.FloatConst)
            {
                NextToken();
            }
            else if (_currentToken == LexicalAnalyzer.LeftPar)
            {
                Match(LexicalAnalyzer.LeftPar);
                Expression();
                Match(LexicalAnalyzer.RightPar);
            }
            else if (_currentToken == LexicalAnalyzer.NotSy)
            {
                Match(LexicalAnalyzer.NotSy);
                Factor();
            }
            else
            {
                Error("Ожидался операнд в выражении");
                if (_currentToken == LexicalAnalyzer.Ident)
                {
                    Match(LexicalAnalyzer.Ident);
                }
                else
                {
                    NextToken();
                }
            }
        }
    }
}