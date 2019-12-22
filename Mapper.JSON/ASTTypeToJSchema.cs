using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.AST;
using Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Mapper.JSON
{
    public class ASTTypeToJSchema
    {
        private List<string> NodeNames = new List<string>();
        private JObject References = new JObject();
        public List<IASTNode> Nodes { get; private set; } = new List<IASTNode>();
        private CarConfig CarConfig { get; }

        public ASTTypeToJSchema(CarConfig carConfig)
        {
            this.CarConfig = carConfig;
        }

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
            var _mod = field.Types.First().Value;
            var _type = field.Types.Last().Value;

            return MapDefinition(_mod, _type, field);
        }

        private JSchema MapBasicArray(string _type, IRestrictable restrictable)
        {
            var schema = new JSchema
            {
                Type = JSchemaType.Array
            };
            schema.Items.Add(JsonMapper.MapBasicType(_type, restrictable, this.CarConfig));
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
                var fieldName = JsonMapper.ToSnakeCase(field.Name);
                schema.Properties.Add(fieldName, MapTypeField(field));

                if (field.Types.First().Value != "Maybe")
                {
                    schema.Required.Add(fieldName);
                }
            });
            return schema;
        }

        private JSchema? MapASTAlias(ASTAlias astAlias)
        {
            var _mod = astAlias.Types.First().Value;
            var _type = astAlias.Types.Last().Value;
            var result = MapDefinition(_mod, _type, astAlias);
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

        private JSchema? MapDefinition(string _mod, string _type, IRestrictable restrictable)
        {
            var isBasicType = JsonMapper.IsBasicType(_type);
            var min = restrictable.Restrictions.FirstOrDefault(r => r.Key == "min");
            var max = restrictable.Restrictions.FirstOrDefault(r => r.Key == "max");
            var pattern = restrictable.Restrictions.FirstOrDefault(r => r.Key == "pattern");
            var decimals = restrictable.Restrictions.FirstOrDefault(r => r.Key == "decimals");

            JSchema? result;

            if (_mod == "List" && isBasicType)
            {
                result = MapBasicArray(_type, restrictable);
                result.MinimumItems = int.Parse(min?.Value ?? this.CarConfig.DefaultRestrictions.ListRestrictions.Min.ToString());
                result.MaximumItems = int.Parse(min?.Value ?? this.CarConfig.DefaultRestrictions.ListRestrictions.Max.ToString());
            }
            else if (_mod == "List" && !isBasicType)
            {
                var temp = MapAstNode(_type);
                result = new JSchema
                {
                    Type = JSchemaType.Array,
                    MinimumItems = int.Parse(min?.Value ?? this.CarConfig.DefaultRestrictions.ListRestrictions.Min.ToString()),
                    MaximumItems = int.Parse(min?.Value ?? this.CarConfig.DefaultRestrictions.ListRestrictions.Max.ToString())
                };
                result.Items.Add(temp);
            }
            else if (isBasicType)
            {
                result = JsonMapper.MapBasicType(_type, restrictable, this.CarConfig);
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


    }
}
