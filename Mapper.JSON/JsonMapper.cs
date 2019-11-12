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
                            ASTTypeToJSchema.Create(astType, this.Nodes));
                    }
                }
            }
        }

        public Dictionary<string, string> ToFileNameAndContentDict()
        {
            var d = new Dictionary<string, string>();
            this.Schemas.ToList().ForEach(s => {
                Console.WriteLine(s.Key);
                d.Add(s.Key, s.Value.ToString());
            });
            return d;
        }
        

        internal static JSchema ConvertToJsonType(string sourceType)
        {
            return sourceType switch
            {
                "String" => new JSchema { Type = JSchemaType.String },
                "Number" => new JSchema { Type = JSchemaType.Number },
                _ => new JSchema { Type = JSchemaType.String }
            };
        }

        internal static string Annotate(IEnumerable<ASTAnnotation> annotations)
        {
            return string.Join("\n", annotations.Select(a => a.Value).ToList());
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
