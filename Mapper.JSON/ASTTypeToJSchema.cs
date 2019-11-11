using System;
using Compiler.AST;
using Newtonsoft.Json.Schema;

namespace Mapper.JSON
{
    public class ASTTypeToJSchema
    {
        public static JSchema Create(ASTType astType)
        {
            JSchema schema = new JSchema
            {
                Title = astType.Name,
                Description = JsonMapper.Annotate(astType.Annotations),
                Type = JSchemaType.Object
            };
            JSchema properties = new JSchema { };

            return schema;
        }
    }
}
