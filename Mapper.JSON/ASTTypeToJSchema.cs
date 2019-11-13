using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.AST;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Mapper.JSON
{
    public class ASTTypeToJSchema
    {
        private JObject References = new JObject();
        public List<IASTNode> Nodes { get; private set; } = new List<IASTNode>();

        public JSchema Create(ASTType astType, IEnumerable<IASTNode> nodes)
        {
            this.Nodes = nodes.ToList();
            JSchema schema = new JSchema
            {
                SchemaVersion = new Uri("http://json-schema.org/draft-07/schema#"),
                Title = astType.Name,
                Description = JsonMapper.Annotate(astType.Annotations),
                Type = JSchemaType.Object
            };

            astType.Fields.ToList().ForEach(field =>
            {
                schema.Properties.Add(field.Name, MapTypeField(field));

                if (field.Type.First().Value != "Maybe")
                {
                    schema.Required.Add(field.Name);
                }
            });

            schema.ExtensionData.Add("references", References);

            return schema;
        }

        private JSchema MapTypeField(ASTTypeField field)
        {
            var _mod = field.Type.First().Value;
            var _type = field.Type.Last().Value;

            return MapDefinition(_mod, _type);
        }

        private JSchema MapBasicArray(string _type)
        {
            var schema = new JSchema
            {
                Type = JSchemaType.Array
            };
            schema.Items.Add(MapBasicType(_type));
            return schema;
        }

        private JSchema MapBasicType(string _type)
        {
            return _type switch
            {
                "String" => new JSchema { Type = JSchemaType.String },
                "Number" => new JSchema { Type = JSchemaType.Number },
                "Boolean" => new JSchema { Type = JSchemaType.Boolean },
                "Date" => new JSchema { Type = JSchemaType.String, Format = "date" },
                "Time" => new JSchema { Type = JSchemaType.String, Format = "time" },
                "DateTime" => new JSchema { Type = JSchemaType.String, Format = "date-time" },
                _ => new JSchema { Type = JSchemaType.String }
            };
        }

        private JSchema MapASTType(ASTType astType)
        {

            // TODO: Check if the type already exists on the references list.

            JSchema schema = new JSchema
            {
                Description = JsonMapper.Annotate(astType.Annotations),
                Type = JSchemaType.Object
            };

            astType.Fields.ToList().ForEach(field =>
            {
                schema.Properties.Add(field.Name, MapTypeField(field));

                if (field.Type.First().Value != "Maybe")
                {
                    schema.Required.Add(field.Name);
                }
            });
            AddReference(astType.Name, schema);
            return schema;
        }

        private JSchema MapASTAlias(ASTAlias astAlias)
        {
            var _mod = astAlias.Type.First().Value;
            var _type = astAlias.Type.Last().Value;
            var result = MapDefinition(_mod, _type);
            AddReference(astAlias.Name, result);
            return result;
        }

        private void AddReference(string name, JSchema jSchema)
        {
            References.TryAdd(name, jSchema);
        }

        private JSchema MapDefinition(string _mod, string _type)
        {
            var isBasicType = JsonMapper.IsBasicType(_type);
            JSchema result;

            if (_mod == "List" && isBasicType)
            {
                return MapBasicArray(_type);
            }
            else if (_mod == "List" && !isBasicType)
            {
                var refNode = JsonMapper.Find(Nodes, _type);
                var temp = refNode switch
                {
                    ASTType n => MapASTType(n),
                    ASTAlias n => MapASTAlias(n),
                    _ => throw new NotImplementedException("Not implemented.")
                };

                result = new JSchema
                {
                    Type = JSchemaType.Array,
                };
                result.Items.Add(temp);
            }
            else if (isBasicType)
            {
                return MapBasicType(_type);
            }
            else
            {
                var refNode = JsonMapper.Find(Nodes, _type);
                result = refNode switch
                {
                    ASTType n => MapASTType(n),
                    ASTAlias n => MapASTAlias(n),
                    _ => throw new NotImplementedException("Not implemented.")
                };
            }
            return result;
        }
    }
}


/*
astType.Fields.ToList().ForEach(field =>
            {
                var _mod = field.Type.First().Value;
                var _type = field.Type.Last().Value;

                if (_mod == "List")
                {
                    if (JsonMapper.IsBasicType(_type))
                    {
                        var subSchema = new JSchema
                        {
                            Title = astType.Name,
                            Type = JSchemaType.Array
                        };
                        subSchema.Items.Add(JsonMapper.ConvertToJsonType(_type));
                        schema.Properties.Add(field.Name, subSchema);
                    } else
                    {
                        var referenceNode = JsonMapper.Find(nodes, _type);
                        if (!(referenceNode is null))
                        {
                            if (referenceNode is ASTType)
                            {
                                var refJson = ASTTypeToJSchema.Create((ASTType)referenceNode, nodes);
                                var subSchema = new JSchema
                                {
                                    Title = field.Name,
                                    Type = JSchemaType.Array
                                };
                                subSchema.Items.Add(refJson);
                                schema.Properties.Add(_type, subSchema);
                                references.Add(_type, subSchema);
                            }
                            else if (referenceNode is ASTAlias)
                            {
                                var refJson = ASTAliasToJSchema.Create((ASTAlias)referenceNode, nodes);
                                schema.Properties.Add(_type, refJson);
                                references.Add(_type, refJson);
                            }
                        }
                    }
                }
                else
                {
                    if (JsonMapper.IsBasicType(_type))
                    {
                        schema.Properties.Add(field.Name, JsonMapper.ConvertToJsonType(_type));
                    }
                    else
                    {
                        var referenceNode = JsonMapper.Find(nodes, _type);
                        if (!(referenceNode is null))
                        {
                            if (referenceNode is ASTType)
                            {
                                var refJson = ASTTypeToJSchema.Create((ASTType)referenceNode, nodes);
                                schema.Properties.Add(_type, refJson);
                                references.Add(_type, refJson);
                            }
                            else if (referenceNode is ASTAlias)
                            {
                                var refJson = ASTAliasToJSchema.Create((ASTAlias)referenceNode, nodes);
                                schema.Properties.Add(_type, refJson);
                                references.Add(_type, refJson);
                            }
                        }
                    }
                }


                if (_mod != "Maybe")
                {
                    schema.Required.Add(field.Name);
                }
            });

    */
