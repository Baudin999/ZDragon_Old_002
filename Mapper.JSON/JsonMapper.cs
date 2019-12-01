using System;
using System.Collections.Generic;
using System.Linq;
using Compiler;
using Compiler.AST;
using Newtonsoft.Json.Schema;

namespace Mapper.JSON
{
    public class JsonMapper
    {
        public ASTGenerator Generator { get; }
        public Dictionary<string, JSchema> Schemas { get; } = new Dictionary<string, JSchema>();

        public JsonMapper(ASTGenerator generator)
        {
            this.Generator = generator;
        }

        public void Start()
        {
            foreach (var node in this.Generator.AST)
            {
                if (node is ASTType && ((ASTType)node).Directives.Any())
                {
                    var astType = (ASTType)node;
                    var apiDirective = astType.Directives.FirstOrDefault(d => d.Key == "api");
                    if (!(apiDirective is null))
                    {
                        this.Schemas.Add($"{astType.Name}.schema.json",
                            new ASTTypeToJSchema().Create(astType, this.Generator.AST));
                    }
                }
                else if (node is ASTData && ((ASTData)node).Directives.Any())
                {
                    var astType = (ASTData)node;
                    var apiDirective = astType.Directives.FirstOrDefault(d => d.Key == "api");
                    if (!(apiDirective is null))
                    {
                        this.Schemas.Add($"{astType.Name}.schema.json",
                            new ASTTypeToJSchema().Create(astType, this.Generator.AST));
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
