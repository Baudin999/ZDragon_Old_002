﻿using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.AST;

namespace Compiler
{
    public class VisitorSource
        : VisitorBase<string>
    {
        
        public VisitorSource(ASTGenerator generator) : base(generator)
        {
            
        }

        public override string VisitASTAlias(ASTAlias astAlias)
        {
            var parts = new List<string>();
            parts.AddRange(astAlias.Annotations.Select(Visit));
            parts.AddRange(astAlias.Directives.Select(Visit));

            var typeDef = string.Join(" ", astAlias.Types.Select(Visit));

            if (!astAlias.Restrictions.Any())
            {
                parts.Add($@"alias {astAlias.Name} = {typeDef};");
            } else
            {
                var restrictions = String.Join(Environment.NewLine, astAlias.Restrictions.Select(Visit));
                parts.Add($"alias {astAlias.Name} = {typeDef}\n{restrictions};");
            }
            return String.Join(Environment.NewLine, parts.ToArray());
        }

        public override string VisitASTAnnotation(ASTAnnotation astAnnotation)
        {
            return $"@ {astAnnotation.Value}";
        }

        public override string VisitASTChapter(ASTChapter astChapter)
        {
            return astChapter.Content;
        }

        public override string VisitASTChoice(ASTChoice astChoice)
        {
            return "";
        }

        public override string VisitASTData(ASTData astData)
        {
            return "";
        }

        public override string VisitASTDirective(ASTDirective astDirective)
        {
            return $"% {astDirective.Key}: {astDirective.Value}";
        }

        public override string VisitASTFlow(ASTFlow astFlow)
        {
            return "";
        }

        public override string VisitASTOption(ASTOption astOption)
        {
            return "";
        }

        public override string VisitASTParagraph(ASTParagraph astParagraph)
        {
            return astParagraph.Content;
        }

        public override string VisitASTRestriction(ASTRestriction astRestriction)
        {
            var indent = astRestriction.Depth == 1 ? "    " : "        ";
            var annotations = astRestriction
                    .Annotations
                    .Select(Visit)
                    .Select(a => indent + a);
            var a = String.Join(Environment.NewLine, annotations);

            if (annotations.Any())
            {
                return $"\n{a}\n{indent}& {astRestriction.Key} {astRestriction.Value}";
            }
            else
            {
                return $"{indent}& {astRestriction.Key} {astRestriction.Value}";
            }
        }

        public override string VisitASTType(ASTType astType)
        {
            var parts = new List<string>();
            parts.AddRange(astType.Annotations.Select(Visit));
            parts.AddRange(astType.Directives.Select(Visit));

            if (!astType.Fields.Any())
            {
                parts.Add($"type {astType.Name}");
            }
            else
            {
                parts.Add($"type {astType.Name} =");
                parts.AddRange(astType.Fields.Select(Visit));
            }

            return String.Join(Environment.NewLine, parts.ToArray());
        }

        public override string VisitASTTypeDefinition(ASTTypeDefinition astTypeDefinition)
        {
            return astTypeDefinition.Value;
        }

        public override string VisitASTTypeField(ASTTypeField astTypeField)
        {
            var typeDef = string.Join(" ", astTypeField.Types.Select(Visit));
            var restrictions = String.Join(Environment.NewLine, astTypeField.Restrictions.Select(Visit));

            if (astTypeField.Restrictions.Any())
            {
                return $"    {astTypeField.Name}: {typeDef}\n{restrictions}\n    ;";
            } else
            {
                return $"    {astTypeField.Name}: {typeDef};";
            }
        }

        public override string VisitASTView(ASTView astView) => throw new NotImplementedException();

        public override string VisitASTImport(ASTImport astImport)
        {
            if (astImport.Imports.Any())
            {
                return $"open {astImport} importing ({String.Join(", ", astImport.Imports)})";
            }
            else
            {
                return $"open {astImport}";
            }
        }

        public override string VisitDefault(IASTNode node)
        {
            return "";
        }
    }
}
