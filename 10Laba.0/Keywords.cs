using System;
using System.Collections.Generic;

namespace _10Laba._0
{
    public class Keywords
    {
        private readonly Dictionary<string, byte> _keywordTable;

        public Keywords()
        {
            _keywordTable = new Dictionary<string, byte>(StringComparer.OrdinalIgnoreCase)
            {
                { "do", LexicalAnalyzer.DoSy },
                { "if", LexicalAnalyzer.IfSy },
                { "in", LexicalAnalyzer.InSy },
                { "of", LexicalAnalyzer.OfSy },
                { "or", LexicalAnalyzer.OrSy },
                { "to", LexicalAnalyzer.ToSy },
                { "end", LexicalAnalyzer.EndSy },
                { "var", LexicalAnalyzer.VarSy },
                { "div", LexicalAnalyzer.DivSy },
                { "and", LexicalAnalyzer.AndSy },
                { "not", LexicalAnalyzer.NotSy },
                { "for", LexicalAnalyzer.ForSy },
                { "mod", LexicalAnalyzer.ModSy },
                { "nil", LexicalAnalyzer.NilSy },
                { "set", LexicalAnalyzer.SetSy },
                { "then", LexicalAnalyzer.ThenSy },
                { "else", LexicalAnalyzer.ElseSy },
                { "case", LexicalAnalyzer.CaseSy },
                { "file", LexicalAnalyzer.FileSy },
                { "goto", LexicalAnalyzer.GotoSy },
                { "type", LexicalAnalyzer.TypeSy },
                { "with", LexicalAnalyzer.WithSy },
                { "begin", LexicalAnalyzer.BeginSy },
                { "while", LexicalAnalyzer.WhileSy },
                { "array", LexicalAnalyzer.ArraySy },
                { "const", LexicalAnalyzer.ConstSy },
                { "label", LexicalAnalyzer.LabelSy },
                { "until", LexicalAnalyzer.UntilSy },
                { "downto", LexicalAnalyzer.DownToSy },
                { "packed", LexicalAnalyzer.PackedSy },
                { "record", LexicalAnalyzer.RecordSy },
                { "repeat", LexicalAnalyzer.RepeatSy },
                { "program", LexicalAnalyzer.ProgramSy },
                { "function", LexicalAnalyzer.FunctionSy },
                { "procedure", LexicalAnalyzer.ProcedureSy }
            };
        }

        public byte? GetKeywordCode(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            return _keywordTable.TryGetValue(name, out byte code) ? code : null;
        }

        public bool IsKeyword(string name)
        {
            return GetKeywordCode(name).HasValue;
        }
    }
}