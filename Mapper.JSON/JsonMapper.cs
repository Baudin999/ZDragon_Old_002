using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.AST;
using Newtonsoft.Json.Schema;

namespace Mapper.JSON
{
    public class JsonMapper
    {
        public IEnumerable<IASTNode> Nodes { get; }
        public Dictionary<string, JSchema> Schemas { get; } = new Dictionary<string, JSchema>();

        public JsonMapper(IEnumerable<IASTNode> nodes)
        {
            this.Nodes = nodes;
        }

        public void Start()
        {
            foreach (var node in this.Nodes)
            {
                if (node is ASTType && ((ASTType)node).Directives.Any())
                {
                    var astType = (ASTType)node;
                    var apiDirective = astType.Directives.FirstOrDefault(d => d.Key == "api");
                    if (!(apiDirective is null))
                    {
                        this.Schemas.Add($"{astType.Name}.schema.json",
                            new ASTTypeToJSchema().Create(astType, this.Nodes));
                    }
                }
                else if (node is ASTData && ((ASTData)node).Directives.Any())
                {
                    var astType = (ASTData)node;
                    var apiDirective = astType.Directives.FirstOrDefault(d => d.Key == "api");
                    if (!(apiDirective is null))
                    {
                        this.Schemas.Add($"{astType.Name}.schema.json",
                            new ASTTypeToJSchema().Create(astType, this.Nodes));
                    }
                }
            }
        }

        public Dictionary<string, string> ToFileNameAndContentDict()
        {
            var d = new Dictionary<string, string>();
            this.Schemas.ToList().ForEach(s => {
                d.Add(s.Key, s.Value.ToString());
            });
            return d;
        }
        

        //internal static JSchema ConvertToJsonType(string sourceType)
        //{
        //    return sourceType switch
        //    {
        //        "String" => new JSchema { Type = JSchemaType.String },
        //        "Number" => new JSchema { Type = JSchemaType.Number },
        //        "Boolean" => new JSchema {  Type = JSchemaType.Boolean },
        //        "Date" => new JSchema {  Type = JSchemaType.String, Format = "date" },
        //        "Time" => new JSchema {  Type = JSchemaType.String, Format = "time" },
        //        "DateTime" => new JSchema {  Type = JSchemaType.String, Format = "date-time" },
        //        _ => new JSchema { Type = JSchemaType.String }
        //    };
        //}

        internal static string Annotate(IEnumerable<ASTAnnotation> annotations)
        {
            return string.Join("\n", annotations.Select(a => a.Value).ToList());
        }

        internal static bool IsBasicType(string t)
        {
            return t == "String" || t == "Number" || t == "Boolean" || t == "Date" || t == "Time" || t == "DateTime";
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
    }
}

/*
$schema": "http://json-schema.org/draft-04/schema#",
   "title": "Product",
   "description": "A product from Acme's catalog",
   "type": "object",
	
   "properties": {
	
      "id": {
         "description": "The unique identifier for a product",
         "type": "integer"
      },
		
      "name": {
         "description": "Name of the product",
         "type": "string"
      },
		
      "price": {
         "type": "number",
         "minimum": 0,
         "exclusiveMinimum": true
      }
   },
	
   "required": ["id", "name", "price"]
*/
