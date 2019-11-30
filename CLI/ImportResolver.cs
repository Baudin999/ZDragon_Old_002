using System;
using System.Collections.Generic;
using System.Linq;
using Compiler;
using Compiler.AST;

namespace CLI
{
    public static class ImportResolver
    {
        public static List<IASTNode> ResolveImports(ASTGenerator generator)
        {

            var imports = new List<IASTNode>();
            var _imports = generator.AST.FindAll(n => n is ASTImport).ToList();
            _imports.ForEach(node =>
            {
                var import = (ASTImport)node;
                var ast = Project.Current?.GetAstForModule(import.Name);
                if (!import.Imports.Any())
                {
                    var copies = ast?
                        .FindAll(a => a is ASTType || a is ASTAlias || a is ASTData || a is ASTChoice)
                        .Select(a =>
                        {
                            return a switch
                            {
                                ASTType t => t.Clone() as IASTNode,
                                ASTAlias t => t.Clone() as IASTNode,
                                ASTData t => t.Clone() as IASTNode,
                                ASTChoice t => t.Clone() as IASTNode,
                                _ => null
                            };
                        })
                        .ToList()
                        .Where(n => n != null)
                        .OfType<IASTNode>()
                        .ToList() ?? Enumerable.Empty<IASTNode>();

                    // For some reason the compiler cannot find that I filter out all of the
                    // null's from the list and so I will only have a list of reference types
                    // of type IASTNode...

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                    imports.AddRange(copies);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                }
                else
                {
                    var importedNodes = import.Imports.Select(import =>
                    {
                        return ast.FirstOrDefault(a =>
                        {
                            return a switch
                            {
                                ASTType n => n.Name == import,
                                ASTAlias n => n.Name == import,
                                ASTData n => n.Name == import,
                                ASTChoice n => n.Name == import,
                                _ => false
                            };
                        });
                    })
                    .ToList()
                    .FindAll(n => !(n is null));
                    imports.AddRange(importedNodes);
                }
            });

            return imports;
        }
    }
}
