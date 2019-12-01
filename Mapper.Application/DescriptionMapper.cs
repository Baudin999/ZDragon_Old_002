using System;
using System.Collections.Generic;
using System.Linq;
using Compiler;
using Compiler.AST;

namespace Mapper.Application
{
    public class DescriptionMapper : VisitorDefault<IEnumerable<Descriptor>>
    {
        private string ModuleName;
        public DescriptionMapper(ASTGenerator generator) : base(generator)
        {
        }

        public DescriptionMapper(ASTGenerator generator, string moduleName) : base(generator)
        {
            this.ModuleName = moduleName;
        }

        public override IEnumerable<Descriptor> VisitASTType(ASTType astType) {
            yield return new Descriptor
            {
                Module = ModuleName,
                Name = astType.Name,
                Description = MapAnnotations(astType.Annotations),
                DescriptorType = DescriptorType.Type.ToString("g")
            };


            //foreach (var field in astType.Fields)
            //{
            //    yield return new Descriptor
            //    {
            //        Module = ModuleName,
            //        Name = field.Name,
            //        Description = MapAnnotations(field.Annotations),
            //        Parent = astType.Name,
            //        Type = MapTypes(field.Type),
            //        DescriptorType = DescriptorType.Field.ToString("g")
            //    };
            //}
        }

        public override IEnumerable<Descriptor> VisitASTAlias(ASTAlias astAlias)
        {
            yield return new Descriptor
            {
                Module = ModuleName,
                Name = astAlias.Name,
                Description = MapAnnotations(astAlias.Annotations),
                Type = MapTypes(astAlias.Type),
                DescriptorType = DescriptorType.Alias.ToString("g")
            };
        }

        public override IEnumerable<Descriptor> VisitASTData(ASTData astData)
        {
            yield return new Descriptor
            {
                Module = ModuleName,
                Name = astData.Name,
                Description = MapAnnotations(astData.Annotations),
                DescriptorType = DescriptorType.Data.ToString("g")
            };

            //foreach (var option in astData.Options)
            //{
            //    yield return new Descriptor
            //    {
            //        Module = ModuleName,
            //        Name = option.Name,
            //        Parent = astData.Name,
            //        Description = MapAnnotations(option.Annotations),
            //        DescriptorType = DescriptorType.DataOption.ToString("g")
            //    };
            //}
        }

        public override IEnumerable<Descriptor> VisitASTChoice(ASTChoice astChoice)
        {
            yield return new Descriptor
            {
                Module = ModuleName,
                Name = astChoice.Name,
                Description = MapAnnotations(astChoice.Annotations),
                DescriptorType = DescriptorType.Choice.ToString("g")
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
