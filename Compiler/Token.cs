using System;
namespace Compiler
{
    public class Token
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public string Value { get; set;  }
        public TokenType TokenType { get; set; }


        public Token()
        {
        }

        public override string ToString()
        {
            return string.Format($"({StartIndex}, {EndIndex}): {TokenType}::{Value}");
        }
    }

    public enum TokenType
    {
        Word,
        WhiteSpace,
        Other,
        NewLine
    }
}
