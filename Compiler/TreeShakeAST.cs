using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.AST;

namespace Compiler
{
    public class TreeShakeAST
    {
        public IEnumerable<IASTNode> Imports { get; }
        public List<IASTNode> CombinedNodes { get; }
        public IEnumerable<IASTNode> ParseTree { get; }

        public TreeShakeAST(IEnumerable<IASTNode> tree, IEnumerable<IASTNode> imports)
        {
            this.Imports = imports;
            this.CombinedNodes = new List<IASTNode>();
            this.ParseTree = tree;
        }

        public List<IASTNode> Shake()
        {
            Action<IASTNode> parseType = (n) => { };
            Action<string> resolveNode = (name) =>
            {
                IASTNode? import = FindNode(name);
                if (import is null)
                {
                    import = FindImportedNode(name);
                    if (!(import is null))
                    {
                        parseType(import);
                    }
                }

                if (import != null)
                {
                    CombinedNodes.Add(import);
                }
            };

            parseType = (IASTNode node) =>
            {
                if (node is ASTData dataNode)
                {
                    CombinedNodes.Add(dataNode);
                    foreach (var option in dataNode.Options)
                    {
                        var _type = option.Name;
                        resolveNode(_type);
                    }
                }
                else if (node is ASTAlias alias)
                {
                    CombinedNodes.Add(alias);
                    var _type = alias.Types.Last().Value;
                    resolveNode(_type);
                }
                else if (node is ASTType t)
                {
                    CombinedNodes.Add(t);
                    foreach (var extension in t.Extensions)
                    {
                        resolveNode(extension);
                    }
                    foreach (var field in t.Fields)
                    {
                        var name = field.OfType(false);
                        if (name != null) resolveNode(name);
                    }
                }
                else if (node is ASTView view)
                {
                    CombinedNodes.Add(view);
                    foreach (var viewNode in view.Nodes)
                    {
                        resolveNode(viewNode);
                    }
                }
                else if (node is ASTChoice choice)
                {
                    CombinedNodes.Add(choice);
                }
                else
                {
                    CombinedNodes.Add(node);
                }
            };

            var list = this.ParseTree.Select(n => n is ICloneable ? (IASTNode)((ICloneable)n).Clone() : n).ToList();
            foreach (var node in list)
            {
                parseType(node);
            }

            return this.CombinedNodes.Distinct().ToList();
        }

        private IASTNode? FindNode(string name)
        {
            return CombinedNodes.FirstOrDefault(n =>
            {
                // Oh my dotnet, what have you done!!
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                return n != null && !(n is ASTImport) && n is INamable && (n as INamable).Name == name;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            });
        }

        private IASTNode? FindImportedNode(string name)
        {
            return this.Imports.FirstOrDefault(n =>
            {
                // Oh my dotnet, what have you done!!
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                return n != null && !(n is ASTImport) && n is INamable && (n as INamable).Name == name;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            });
        }
    }
}
