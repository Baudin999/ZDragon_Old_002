using System;
namespace Compiler
{
    public class Token : ICloneable
    {
        public int StartIndex { get; set; }
        public int StartColumn { get; internal set; }
        public int StartLine { get; set; }

        public int EndIndex { get; set; }
        public int EndColumn { get; internal set; }
        public int EndLine { get; set; }

        public string Value { get; set; } = "";

        public TokenType TokenType { get; set; }


        public override string ToString()
        {
            return String.Format($"({StartColumn}, {StartLine}) ({EndColumn}, {EndLine}) {TokenType} |{Value}|");
        }

        public Token Normalize()
        {
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

        public object Clone()
        {
            var token = new Token
            {
                Value = (string)this.Value.Clone(),
                // Performing a lookup will ensure copy by value
                TokenType = (TokenType)this.TokenType,

                // int's are never copied by reference, no need to clone
                StartColumn = this.StartColumn,
                StartIndex = this.StartIndex,
                StartLine = this.StartLine,
                EndColumn = this.EndColumn,
                EndIndex = this.EndIndex,
                EndLine = this.EndLine
            };

            return token;
        }

        public static Token Empty()
        {
            return new Token
            {
                StartColumn = 0,
                EndColumn = 0,
                StartLine = 0,
                EndLine = 0,
                TokenType = TokenType.Other,
                Value = ""
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
        KW_Importing,
        GroupOpen,
        GroupClosed,
        ListSeparator,
        PluckedField,
        KW_Pluck,
        Operator,
        Op_Next,
        Op_Def,
        KW_Compose,
        KW_Loop,
        QualifiedIdentifier
    }
}
