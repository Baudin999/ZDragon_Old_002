using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Compiler.AST
{
    public class ASTAlias : IASTNode, IRestrictable, IElement, ITypeble, INamable, IRootNode, ICloneable
    {
        public ASTName ASTName { get; }
        public string Name { get { return this.ASTName.Name; } }
        public Token? Token { get; } = Token.Empty();
        public string Module { get; }
        public IEnumerable<ASTTypeDefinition> Types { get; } = Enumerable.Empty<ASTTypeDefinition>();
        public IEnumerable<ASTRestriction> Restrictions { get; } = Enumerable.Empty<ASTRestriction>();
        public IEnumerable<ASTAnnotation> Annotations { get; } = Enumerable.Empty<ASTAnnotation>();
        public IEnumerable<ASTDirective> Directives { get; } = Enumerable.Empty<ASTDirective>();

        public ASTAlias(
            ASTName astName,
            string module,
            IEnumerable<ASTTypeDefinition> types,
            IEnumerable<ASTRestriction> restrictions,
            IEnumerable<ASTAnnotation> annotations,
            IEnumerable<ASTDirective> directives) {
            this.ASTName = astName;
            this.Types = types;
            this.Restrictions = restrictions;
            this.Annotations = annotations;
            this.Directives = directives;
            this.Module = module;
        }

        public static (List<ASTError>, ASTAlias) Parse(
                IParser parser,
                IEnumerable<ASTAnnotation> annotations,
                IEnumerable<ASTDirective> directives,
                string module = "")
        {
            var errors = new List<ASTError>();
            
            if (parser.HasNext()) parser.Next();
            var name = ASTName.Parse(parser);//parser.Consume(TokenType.Identifier);
            parser.Consume(TokenType.Equal);
            var types = ASTTypeDefinition.Parse(parser, module).ToList();
            var restrictions = ASTRestriction.CreateRestrictions(parser, TokenType.KW_Alias).ToList();
            parser.TryConsume(TokenType.EndStatement);
            parser.Consume(TokenType.ContextEnded);

            var result = new ASTAlias(
                name,
                module,
                types,
                restrictions,
                annotations,
                directives);

            return (errors, result);
        }

        public object Clone()
        {
            return new ASTAlias(
                this.ASTName.Clone<ASTName>(),
                (string)this.Module.Clone(),
                ObjectCloner.CloneList(this.Types.ToList()),
                ObjectCloner.CloneList(this.Restrictions.ToList()),
                ObjectCloner.CloneList(this.Annotations.ToList()),
                ObjectCloner.CloneList(this.Directives.ToList())
            );
        }
    }
}
