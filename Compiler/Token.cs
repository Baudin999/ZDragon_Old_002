using System;
namespace Compiler
{
    public class Token
    {
        public int StartIndex { get; set; }
        public int StartColumn { get; internal set; }
        public int StartLine { get; set; }

        public int EndIndex { get; set; }
        public int EndColumn { get; internal set; }
        public int EndLine { get; set; }

        public string Value { get; set; } = "";

        public TokenType TokenType { get; set; }

        public Token()
        {
        }

        public override string ToString()
        {
            return String.Format($"({StartColumn}, {StartLine}) ({EndColumn}, {EndLine}) {TokenType} |{Value}|");
        }

        public Token Normalize() {
            return new Token
            {
                StartIndex = this.StartIndex,
                StartColumn = this.StartColumn,
                StartLine = this.StartLine,

                EndIndex = this.EndIndex,
                EndColumn = this.EndColumn,
                EndLine = this.EndLine,

                TokenType = this.TokenType,

                Value = this.Value.Replace("↓", "\n").Replace("→", "    ")
            };
        }

    }

    public enum TokenType
    {
        Word,
        WhiteSpace,
        Other,
        NewLine,
        Annotation,
        Identifier,
        Directive,
        Equal,
        Or,
        Separator,
        Indent,
        EndStatement,
        KW_Type,
        KW_Alias,
        KW_Data,
        KW_Choice,
        KW_Flow,
        KW_Component,
        KW_View,
        Chapter,
        Paragraph,
        ContextEnded,
        ContextStarted,
        String,
        Number,
        Comment,
        GenericParameter,
        And,
        KW_Extends,
        Pattern,
        EndOfFile,
        KW_Open,
        KW_Importing
    }
}
