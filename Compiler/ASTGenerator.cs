using System.Collections.Generic;
using System.Linq;
using Compiler;
using Compiler.AST;

namespace Compiler
{
    public class ASTGenerator
    {
        public string ModuleName { get; }
        public string Code { get; }
        public IEnumerable<Token> Tokens { get; }
        public Parser Parser { get; }
        public List<IASTNode> ParseTree { get; }
        public List<IASTError> Errors { get; }
        public List<IASTNode> AST { get; }

        public ASTGenerator(string code, string moduleName = "")
        {
            this.ModuleName = moduleName;
            this.Code = code; 
            this.Tokens = new Lexer().Lex(code);
            this.Parser = new Parser(this.Tokens);
            this.ParseTree = this.Parser.Parse().ToList();
            var (errors, nodes) = new Resolver(this.ParseTree).Resolve();
            this.AST = nodes.ToList();
            this.Errors = this.Parser.Errors.Concat(errors).ToList();

        }

    }
}
