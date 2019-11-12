using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.AST;
using Newtonsoft.Json.Schema;

namespace Mapper.JSON
{
    public class ASTTypeToJSchema
    {
        public static JSchema Create(ASTType astType, IEnumerable<IASTNode> nodes)
        {
            JSchema schema = new JSchema
            {
                Title = astType.Name,
                Description = JsonMapper.Annotate(astType.Annotations),
                Type = JSchemaType.Object
            };
            
            astType.Fields.ToList().ForEach(field =>
            {
                var _mod = field.Type.First().Value;
                var _type = field.Type.Last().Value;
                schema.Properties.Add(field.Name, JsonMapper.ConvertToJsonType(_type));
                if (_mod != "Maybe")
                {
                    schema.Required.Add(field.Name);
                }
            });

            return schema;
        }
    }
}
