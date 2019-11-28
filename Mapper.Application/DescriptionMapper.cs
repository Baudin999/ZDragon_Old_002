using System;
using System.Collections.Generic;
using System.Linq;
using Compiler;
using Compiler.AST;

namespace Mapper.Application
{
    public class DescriptionMapper : DefaultVisitor<IEnumerable<Descriptor>>
    {
        private string ModuleName;
        public DescriptionMapper(IEnumerable<IASTNode> nodeTree) : base(nodeTree)
        {
        }

        public DescriptionMapper(IEnumerable<IASTNode> nodeTree, string moduleName) : base(nodeTree)
        {
            this.ModuleName = moduleName;
        }

        public override IEnumerable<Descriptor> VisitASTType(ASTType astType) {
            yield return new Descriptor
            {
                Module = ModuleName,
                Name = astType.Name,
                Description = MapAnnotations(astType.Annotations)
            };


            foreach (var field in astType.Fields)
            {
                yield return new Descriptor
                {
                    Module = ModuleName,
                    Name = field.Name,
                    Description = MapAnnotations(field.Annotations),
                    Type = MapTypes(field.Type)
                };
            }
        }

        public override IEnumerable<Descriptor> VisitASTAlias(ASTAlias astAlias)
        {
            yield return new Descriptor
            {
                Module = ModuleName,
                Name = astAlias.Name,
                Description = MapAnnotations(astAlias.Annotations),
                Type = MapTypes(astAlias.Type)
            };
        }

        public override IEnumerable<Descriptor> VisitASTData(ASTData astData)
        {
            yield return new Descriptor
            {
                Module = ModuleName,
                Name = astData.Name,
                Description = MapAnnotations(astData.Annotations)
            };

            foreach (var option in astData.Options)
            {
                yield return new Descriptor
                {
                    Module = ModuleName,
                    Name = option.Name,
                    Description = MapAnnotations(option.Annotations)
                };
            }
        }

        public override IEnumerable<Descriptor> VisitASTChoice(ASTChoice astChoice)
        {
            yield return new Descriptor
            {
                Module = ModuleName,
                Name = astChoice.Name,
                Description = MapAnnotations(astChoice.Annotations)
            };
        }

        private string MapAnnotations(IEnumerable<ASTAnnotation> annotations)
        {
            return string.Join(" ", annotations.Select(a => a.Value).ToList());
        }

        private string MapTypes(IEnumerable<ASTTypeDefinition> types)
        {
            return string.Join(" ", types.Select(a => a.Value).ToList());
        }
    }
}
