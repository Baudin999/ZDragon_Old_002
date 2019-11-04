using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.AST;

namespace Compiler
{
    public class VisitorSource
        : VisitorBase<string>
    {
        List<string> value = new List<string>();
        public VisitorSource(IEnumerable<IASTNode> nodeTree) : base(nodeTree)
        {
            
        }

        public override string VisitASTAlias(ASTAlias astAlias)
        {
            List<string> parts = new List<string>();
            parts.AddRange(astAlias.Annotations.Select(Visit));
            parts.AddRange(astAlias.Directives.Select(Visit));

            string typeDef = string.Join(" ", astAlias.Type.Select(Visit));

            if (astAlias.Restrictions.Count() == 0)
            {
                parts.Add($@"alias {astAlias.Name} = {typeDef}");
            } else
            {
                string restrictions = string.Join("\n", astAlias.Restrictions.Select(Visit));
                parts.Add($"alias {astAlias.Name} = {typeDef}\n{restrictions}");
            }
            return string.Join("\n", parts.ToArray());
        }

        public override string VisitASTAnnotation(ASTAnnotation astAnnotation)
        {
            return $"@ {astAnnotation.Value}";
        }

        public override string VisitASTChoice(ASTChoice astChoice)
        {
            return "";
        }

        public override string VisitASTDirective(ASTDirective astDirective)
        {
            return $"% {astDirective.Key}: {astDirective.Value}";
        }

        public override string VisitASTOption(ASTOption astOption)
        {
            throw new NotImplementedException();
        }

        public override string VisitASTRestriction(ASTRestriction astRestriction)
        {
            var indent = astRestriction.Depth == 1 ? "    " : "        ";
            var annotations = astRestriction
                    .Annotations
                    .Select(Visit)
                    .Select(a => indent + a);
            var a = string.Join("\n", annotations);

            if (annotations.Count() > 0)
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
            List<string> parts = new List<string>();
            parts.AddRange(astType.Annotations.Select(Visit));
            parts.AddRange(astType.Directives.Select(Visit));

            if (astType.Fields.Count() == 0)
            {
                parts.Add($"type {astType.Name}");
            }
            else
            {
                parts.Add($"type {astType.Name} =");
                parts.AddRange(astType.Fields.Select(Visit));
            }

            return string.Join("\n", parts.ToArray());
        }

        public override string VisitASTTypeDefinition(ASTTypeDefinition astTypeDefinition)
        {
            return astTypeDefinition.Value;
        }

        public override string VisitASTTypeField(ASTTypeField astTypeField)
        {
            string typeDef = string.Join(" ", astTypeField.Type.Select(Visit));
            string restrictions = string.Join("\n", astTypeField.Restrictions.Select(Visit));

            if (astTypeField.Restrictions.Count() > 0)
            {
                return $"    {astTypeField.Name}: {typeDef}\n{restrictions}\n    ;";
            } else
            {
                return $"    {astTypeField.Name}: {typeDef};";
            }
        }

        public override string VisitDefault(IASTNode node)
        {
            return "";
        }
    }
}
