using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.AST;

namespace Compiler
{
    public class Verificator
    {
        public List<IASTNode> ParseTree { get; }

        public Verificator(IEnumerable<IASTNode> parseTree)
        {
            this.ParseTree = parseTree.ToList();
        }

        public IEnumerable<ASTError> Verify()
        {
            var errors = FindDoubles().Concat(FindUnknownTypes());
            return errors;
        }

        private IEnumerable<ASTError> FindDoubles()
        {
            var errors = new List<ASTError>();
            var elements = new List<string>();
            ParseTree.ToList().ForEach(n =>
            {
                if (n is INamable namable && !(n is ASTImport))
                {
                    if (elements.Contains(namable.Name))
                    {
                        errors.Add(new ASTError($"{namable.Name} already exists in your model. Duplicate definitions are not allowed.", "Duplicate Type"));
                    }
                    else
                    {
                        elements.Add(namable.Name);
                    }
                }
            });

            return errors;
        }

        private IEnumerable<ASTError> FindUnknownTypes()
        {
            var errors = new List<ASTError>();
            var elements = new List<string>();
            ParseTree.ForEach(n =>
            {
                if (n is ASTType type)
                {
                    foreach (var field in type.Fields)
                    {
                        var (_mod, _type) = (field.Type.First().Value, field.Type.Last().Value);
                        var __type = (_mod, _type) switch
                        {
                            ("Maybe", _) => _type,
                            ("List", _) => _type,
                            (_, _) => _mod
                        };

                        if (!Parser.BaseTypes.Contains(__type) && !__type.StartsWith("'", StringComparison.CurrentCulture))
                        {
                            var foundNode = ParseTree.FirstOrDefault(n => n is INamable && ((INamable)n).Name == _type);
                            if (foundNode is null)
                            {
                                errors.Add(new ASTError($@"
Could not find type {__type} on field {field.Name} on type {type.Name}.

type {type.Name} ...
    ...
    {field.Name}: {string.Join(Environment.NewLine, field.Type.Select(t => t.Value).ToList())};

", "Unknown Type"));
                            }
                        }
                    }
                }
            });

            return errors;
        }
    }
}
