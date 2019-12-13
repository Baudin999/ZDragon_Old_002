using System;
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

        /// <summary>
        /// Initialise an ASTGenerator with a module name and a list of nodes.
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="nodes"></param>
        public ASTGenerator(string moduleName, IEnumerable<IASTNode> nodes)
        {
            this.ModuleName = moduleName;
            this.Code = "";
            this.AST = nodes.ToList();
            this.Errors = new List<IASTError>();
            this.ParseTree = nodes.ToList();
            this.Tokens = Enumerable.Empty<Token>();
            this.Parser = new Parser(this.Tokens);

            var sourceVisitor = new VisitorSource(this);
            var parts = sourceVisitor.Start().ToList();
            this.Code = string.Join(Environment.NewLine + Environment.NewLine, parts);
        }

        public ASTGenerator(string code, string moduleName = "", IEnumerable<IASTNode>? imports = null)
        {
            this.ModuleName = moduleName;
            this.Code = code;
            if (code.Length == 0)
            {
                this.Tokens = Enumerable.Empty<Token>();
                this.Parser = Parser.Empty();
                this.ParseTree = new List<IASTNode>();
                this.AST = new List<IASTNode>();
                this.Errors = new List<IASTError>();
            }
            else
            {
                this.Tokens = new Lexer().Lex(code);
                this.Parser = new Parser(this.Tokens);
                var parseTree = this.Parser.Parse(moduleName).ToList();
                if (imports != null)
                {
                    parseTree = new TreeShakeAST(parseTree, imports ?? Enumerable.Empty<IASTNode>()).Shake();
                }
                this.ParseTree = parseTree;
                var (errors, nodes) = new Resolver(this.ParseTree).Resolve();
                var verificationErrors = new Verificator(nodes).Verify();
                this.AST = nodes.ToList();
                this.Errors = this.Parser.Errors.Concat(errors).Concat(verificationErrors).ToList();
            }
        }

        public IASTNode Find(string name)
        {
            return this.AST.FirstOrDefault(n => !(n is null) && n is INamable && ((INamable)n).Name == name);
        }

        public T Find<T>(string name) where T : IASTNode
        {
            return (T)this.AST.FirstOrDefault(n => !(n is null) && n is INamable && ((INamable)n).Name == name);
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
            var parseTree = new TreeShakeAST(this.ParseTree, imports).Shake();
            var (errors, nodes) = new Resolver(parseTree).Resolve();
            var verificationErrors = new Verificator(nodes).Verify();
            this.AST = nodes.ToList();
            this.Errors = this.Parser.Errors.Concat(errors).Concat(verificationErrors).ToList();

            return (this.Errors, this.AST);
        }

    }
}
