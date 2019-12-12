﻿using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.AST;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Mapper.JSON
{
    public class ASTTypeToJSchema
    {
        private List<string> NodeNames = new List<string>();
        private JObject References = new JObject();
        public List<IASTNode> Nodes { get; private set; } = new List<IASTNode>();

        public JSchema? Create(ASTType astType, IEnumerable<IASTNode> nodes)
        {
            this.Nodes = nodes.ToList();
            var schema = MapAstNode(astType.Name);
            if (schema != null)
            {
                schema.SchemaVersion = new Uri("http://json-schema.org/draft-07/schema#");
                schema.Title = astType.Name;
                schema.Description = JsonMapper.Annotate(astType.Annotations);
                schema.ExtensionData.Add("references", References);
            }
            return schema;
        }

        public JSchema? Create(ASTData astData, IEnumerable<IASTNode> nodes)
        {
            this.Nodes = nodes.ToList();
            var schema = MapAstNode(astData.Name);
            if (schema != null)
            {
                schema.SchemaVersion = new Uri("http://json-schema.org/draft-07/schema#");
                schema.Title = astData.Name;
                schema.Description = JsonMapper.Annotate(astData.Annotations);
                schema.ExtensionData.Add("references", References);
            }
            return schema;
        }

        private JSchema? MapTypeField(ASTTypeField field)
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

        private JSchema MapASTType(ASTType astType)
        {
            var schema = new JSchema
            {
                Description = JsonMapper.Annotate(astType.Annotations),
                Type = JSchemaType.Object,
                Title = astType.Name
            };

            AddReference(astType.Name, schema);

            astType.Fields.ToList().ForEach(field =>
            {
                schema.Properties.Add(field.Name, MapTypeField(field));

                if (field.Type.First().Value != "Maybe")
                {
                    schema.Required.Add(field.Name);
                }
            });
            return schema;
        }

        private JSchema? MapASTAlias(ASTAlias astAlias)
        {
            var _mod = astAlias.Type.First().Value;
            var _type = astAlias.Type.Last().Value;
            var result = MapDefinition(_mod, _type);
            if (result != null)
            {
                result.Title = astAlias.Name;
            } 
            return result;
        }

        private JSchema MapASTChoice(ASTChoice astChoice)
        {
            var result = new JSchema
            {
                Title = astChoice.Name
            };
            astChoice.Options.ToList().ForEach(option =>
            {
                result.Enum.Add(option.Value);
            });
            result.Title = astChoice.Name;
            return result;
        }

        private JSchema MapASTData(ASTData astData)
        {
            var result = new JSchema
            {
                Title = astData.Name
            };
            astData.Options.ToList().ForEach(option =>
            {
                var temp = MapAstNode(option.Name);
                result.AnyOf.Add(temp);
            });
            return result;
        }

        private void AddReference(string name, JSchema jSchema)
        {
            var success = References.TryAdd(name, jSchema);
            if (success) NodeNames.Add(name);
        }

        private JSchema? MapDefinition(string _mod, string _type)
        {
            var isBasicType = JsonMapper.IsBasicType(_type);
            JSchema? result;

            if (_mod == "List" && isBasicType)
            {
                return MapBasicArray(_type);
            }
            else if (_mod == "List" && !isBasicType)
            {
                var temp = MapAstNode(_type);
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
                result = MapAstNode(_type);
            }
            return result;
        }

        private JSchema? MapAstNode(string name)
        {
            if (NodeNames.Contains(name))
            {
                return (JSchema)References.GetValue(name);
            }
            var refNode = JsonMapper.Find(Nodes, name);
            var result = refNode switch
            {
                ASTType n => MapASTType(n),
                ASTAlias n => MapASTAlias(n),
                ASTChoice n => MapASTChoice(n),
                ASTData n => MapASTData(n),
                _ => throw new NotImplementedException("Not implemented.")
            };

            if (result != null)
            {
                AddReference(name, result);
            }
            return result;
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
    }
}
