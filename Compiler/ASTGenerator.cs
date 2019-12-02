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

        /// <summary>
        /// All of the parsed nodes, without the imported nodes.
        /// </summary>
        public List<IASTNode> ParseTree { get; }

        public List<IASTError> Errors { get; private set; }

        /// <summary>
        /// All of the nodes including the imported nodes after Resolve
        /// has been called.
        /// </summary>
        public List<IASTNode> AST { get; private set; }

        public ASTGenerator(IEnumerable<IASTNode> nodes)
        {
            this.ModuleName = "";
            this.AST = nodes.ToList();
            this.Code = "";
            this.ParseTree = nodes.ToList();
            this.Errors = new List<IASTError>();
            this.Tokens = Enumerable.Empty<Token>();
            this.Parser = new Parser(this.Tokens);
        }

        public ASTGenerator(string name, IEnumerable<IASTNode> nodes)
        {
            this.ModuleName = name;
            this.Code = "";
            this.AST = nodes.ToList();
            this.Errors = new List<IASTError>();
            this.ParseTree = nodes.ToList();
            this.Tokens = Enumerable.Empty<Token>();
            this.Parser = new Parser(this.Tokens);
        }

        public ASTGenerator(string code, string moduleName = "")
        {
            this.ModuleName = moduleName;
            this.Code = code; 
            this.Tokens = new Lexer().Lex(code);
            this.Parser = new Parser(this.Tokens);
            this.ParseTree = this.Parser.Parse().ToList();
            var (errors, nodes) = new Resolver(this.ParseTree).Resolve();
            var verificationErrors = new Verificator(nodes).Verify();
            this.AST = nodes.ToList();
            this.Errors = this.Parser.Errors.Concat(errors).Concat(verificationErrors).ToList();
        }

        public IASTNode Find(string name)
        {
            return this.AST.FirstOrDefault(n => !(n is null) && n is INamable && ((INamable)n).Name == name);
        }

        /// <summary>
        /// Re-resolve the types in the AST based extra nodes. Usually used after
        /// resolving some imported types.
        /// This is a second phase of the resolver.
        /// </summary>
        /// <param name="imports"></param>
        /// <returns></returns>
        public (List<IASTError> Errors, List<IASTNode> AST) Resolve(IEnumerable<IASTNode> imports)
        {
            var (errors, nodes) = new Resolver(this.ParseTree.Concat(imports)).Resolve();
            var verificationErrors = new Verificator(nodes).Verify();
            this.AST = nodes.ToList();
            this.Errors = this.Parser.Errors.Concat(errors).Concat(verificationErrors).ToList();

            return (this.Errors, this.AST);
        }

    }
}
