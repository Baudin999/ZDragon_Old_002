using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTTypeField
    {
        public string Name { get; private set; }
        public List<ASTTypeDefinition> Type { get; private set; }
        public List<ASTAnnotation> Annotations { get; private set; }
        public List<ASTRestriction> Restrictions { get; private set; }


        public ASTTypeField(Parser parser)
        {
            this.Restrictions = new List<ASTRestriction>();
            this.Annotations = ASTAnnotation.Parse(parser).ToList();
            this.Name = parser.Consume(TokenType.Identifier).Value;
            Token Separator = parser.Consume(TokenType.Separator);
            this.Type = ASTTypeDefinition.ParseType(parser).ToList();
            this.Restrictions = ASTRestriction.CreateRestrictions(parser).ToList();
            parser.Consume(TokenType.EndStatement);
        }

    }
}
