using System;
using System.Collections.Generic;
using System.Linq;
using Compiler;
using Compiler.AST;
using Configuration;
using Newtonsoft.Json.Schema;

namespace Mapper.JSON
{
    public class ASTAliasToJSchema
    {
        private CarConfig CarConfig { get; }
        public ASTAliasToJSchema(CarConfig carConfig)
        {
            this.CarConfig = carConfig;
        }
        public JSchema? Create(ASTAlias astAlias, ASTGenerator generator)
        {
            var schema = new JSchema();
            if (schema != null)
            {
                schema.SchemaVersion = new Uri("http://json-schema.org/draft-07/schema#");
                schema.Title = astAlias.Name;
                schema.Description = JsonMapper.Annotate(astAlias.Annotations);

                var _mod = astAlias.Types.First().Value;
                var _type = astAlias.Types.Last().Value;

                if (_mod == "List")
                {
                    schema.Type = JSchemaType.Array;
                    if (Parser.BaseTypes.Contains(_mod))
                    {
                        schema.Items.Add(JsonMapper.MapBasicType(_type, astAlias, this.CarConfig));
                        return schema;
                    } else
                    {
                        var typeDef = generator.Find<ASTType>(_type);
                        if (typeDef != null)
                        {
                            var typeSchema = new ASTTypeToJSchema(this.CarConfig).Create(typeDef, generator.AST);
                            schema.Items.Add(typeSchema);
                        }

                    }
                } else if (Parser.BaseTypes.Contains(_type))
                {
                    var mappedType = JsonMapper.MapBasicType(_type, astAlias, this.CarConfig);
                    schema.Properties.Clear();
                    schema.Type = mappedType.Type;
                }
                else
                {
                    schema.Type = JSchemaType.Object;
                    var typeDef = generator.Find<ASTType>(_type);
                    if (typeDef != null)
                    {
                        var typeSchema = new ASTTypeToJSchema(this.CarConfig).Create(typeDef, generator.AST);
                        schema.Items.Add(typeSchema);
                    }
                }

            }
            return schema;
        }
    }
}
