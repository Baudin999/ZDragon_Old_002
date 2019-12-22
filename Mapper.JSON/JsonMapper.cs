using System;
using System.Collections.Generic;
using System.Linq;
using Compiler;
using Compiler.AST;
using Configuration;
using Newtonsoft.Json.Schema;

namespace Mapper.JSON
{
    public class JsonMapper
    {
        public ASTGenerator Generator { get; }
        public CarConfig CarConfig { get; }
        public Dictionary<string, JSchema> Schemas { get; } = new Dictionary<string, JSchema>();

        public JsonMapper(ASTGenerator generator, CarConfig carConfig)
        {
            this.Generator = generator;
            this.CarConfig = carConfig;
        }

        public void Start()
        {
            foreach (var node in this.Generator.AST)
            {
                if (node is ASTType _type && _type != null && _type.Directives.Any())
                {
                    var apiDirective = _type.Directives.FirstOrDefault(d => d.Key == "api");
                    if (!(apiDirective is null) && _type != null)
                    {
                        var schema = new ASTTypeToJSchema(this.CarConfig).Create(_type, this.Generator.AST);
                        if (schema != null) this.Schemas.Add($"{_type.Name}.schema.json", schema);
                    }
                }
                else if (node is ASTData _data && _data != null && _data.Directives.Any())
                {
                    var apiDirective = _data.Directives.FirstOrDefault(d => d.Key == "api");
                    if (!(apiDirective is null) && _data != null)
                    {
                        var schema = new ASTTypeToJSchema(this.CarConfig).Create(_data, this.Generator.AST);
                        if (schema != null) this.Schemas.Add($"{_data.Name}.schema.json", schema);
                    }
                }
                else if (node is ASTAlias alias && alias != null && alias.Directives.Any())
                {
                    var apiDirective = alias.Directives.FirstOrDefault(d => d.Key == "api");
                    if (!(apiDirective is null))
                    {
                        var schema = new ASTAliasToJSchema(this.CarConfig).Create(alias, this.Generator);
                        if (schema != null) this.Schemas.Add($"{alias.Name}.schema.json", schema);
                    }
                }
            }
        }

        public Dictionary<string, string> ToFileNameAndContentDict()
        {
            var d = new Dictionary<string, string>();
            this.Schemas.ToList().ForEach(s =>
            {
                d.Add(s.Key, s.Value.ToString());
            });
            return d;
        }


        internal static string Annotate(IEnumerable<ASTAnnotation> annotations)
        {
            return String.Join(Environment.NewLine, annotations.Select(a => a.Value).ToList());
        }

        internal static bool IsBasicType(string t)
        {
            return Parser.BaseTypes.Contains(t);
        }


        internal static IASTNode? Find(IEnumerable<IASTNode> nodes, string name)
        {
            return nodes.FirstOrDefault(n =>
            {
                return n switch
                {
                    ASTType node => node.Name == name,
                    ASTAlias node => node.Name == name,
                    ASTData node => node.Name == name,
                    ASTChoice node => node.Name == name,
                    _ => false
                };
            });
        }

        internal static JSchema MapBasicType(string _type, IRestrictable restrictable, CarConfig carConfig)
        {


            var min = restrictable.Restrictions.FirstOrDefault(r => r.Key == "min");
            var max = restrictable.Restrictions.FirstOrDefault(r => r.Key == "max");
            var pattern = restrictable.Restrictions.FirstOrDefault(r => r.Key == "pattern");
            var decimals = restrictable.Restrictions.FirstOrDefault(r => r.Key == "decimals");

            var multipleOf = int.Parse(decimals?.Value ?? "2");
            var d = multipleOf switch
            {
                0 => 1d,
                1 => 0.1,
                2 => 0.01,
                3 => 0.001,
                4 => 0.0001,
                5 => 0.00001,
                6 => 0.000001,
                7 => 0.0000001,
                8 => 0.00000001,
                9 => 0.000000001,
                10 => 0.0000000001,
                _ => 0.00000000001
            };

            return _type switch
            {
                "String" => new JSchema
                {
                    Type = JSchemaType.String,
                    MinimumLength = min != null ? int.Parse(min.Value) : carConfig.DefaultRestrictions.StringRestrictions.Min,
                    MaximumLength = max != null ? int.Parse(max.Value) : carConfig.DefaultRestrictions.StringRestrictions.Max,
                },
                "Number" => new JSchema
                {
                    Type = JSchemaType.Integer,
                    Minimum = min != null ? int.Parse(min.Value) : carConfig.DefaultRestrictions.NumberRestrictions.Min,
                    Maximum = max != null ? int.Parse(max.Value) : carConfig.DefaultRestrictions.NumberRestrictions.Max,
                    MultipleOf = 1
                },
                "Decimal" => new JSchema
                {
                    Type = JSchemaType.Number,
                    Minimum = min != null ? int.Parse(min.Value) : carConfig.DefaultRestrictions.DecimalRestrictions.Min,
                    Maximum = max != null ? int.Parse(max.Value) : carConfig.DefaultRestrictions.DecimalRestrictions.Max,
                    MultipleOf = d
                },
                "Boolean" => new JSchema { Type = JSchemaType.Boolean },
                "Date" => new JSchema { Type = JSchemaType.String, Format = "date" },
                "Time" => new JSchema { Type = JSchemaType.String, Format = "time" },
                "DateTime" => new JSchema { Type = JSchemaType.String, Format = "date-time" },
                _ => new JSchema { Type = JSchemaType.String }
            };
        }

        internal static string ToSnakeCase(string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

    }
}
