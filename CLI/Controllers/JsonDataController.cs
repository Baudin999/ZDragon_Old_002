using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Bogus;
using Compiler.AST;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CLI.Controllers
{
    public class JsonDataController : ControllerBase
    {
        private Module? Module;

        [HttpGet("/api/data/{module}/{type}")]
        public IActionResult GetData(string module, string type, [FromQuery]bool list)
        {

            Module = Project.Current?.Modules.First(m => m.Name == module);
            var result = list ? new Faker().Make(10, () => Generate(type)) : Generate(type);
            if (result is null)
            {
                return NotFound();
            }
            else
            {
                return Ok(JsonConvert.SerializeObject(result, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy(true, true)
                    }
                }));
            }
        }

        private dynamic Generate(string nodeName)
        {
            var node = Find(nodeName);

            if (node is ASTType)
            {
                return GenerateType((ASTType)node);
            }
            else if (node is ASTAlias)
            {
                return GenerateAlias((ASTAlias)node);
            }
            else if (node is ASTChoice)
            {
                return GenerateEnum((ASTChoice)node);
            }

            return new { };
        }

        private IASTNode? Find(string name)
        {
            return Module?.Transpiler.AST.FirstOrDefault(m => m is INamable && ((INamable)m).Name == name);
        }

        private dynamic GenerateType(ASTType node)
        {
            dynamic result = new ExpandoObject();
            foreach (var field in node.Fields)
            {
                var fakerDirective = field.Directives.FirstOrDefault(f => f.Key == "faker")?.Value;
                var (_mod, _type) = (field.Type.First().Value, field.Type.Last().Value);
                var data = (_mod, _type) switch
                {
                    ("Maybe", _) => GenerateBaseValue(_type),
                    ("List", _) => GenerateList(_type),
                    (_, _) => GenerateBaseValue(_type, fakerDirective)
                };

                AddProperty(result, field.Name, data);
            }
            return result;
        }

        private dynamic GenerateEnum(ASTChoice choice)
        {
            var faker = new Bogus.Faker();
            return faker.Random.ListItem(choice.Options.ToList()).Value;
        }

        private dynamic GenerateAlias(ASTAlias alias)
        {
            var fakerDirective = alias.Directives.FirstOrDefault(f => f.Key == "faker")?.Value;
            var (_mod, _type) = (alias.Type.First().Value, alias.Type.Last().Value);
            return (_mod, _type) switch
            {
                ("Maybe", _) => GenerateBaseValue(_type),
                ("List", _) => GenerateList(_type),
                (_, _) => GenerateBaseValue(_type, fakerDirective)
            };
        }

        private List<object> GenerateList(string _type)
        {
            var randomAmount = new Random().Next(0, 5);
            return new Faker<dynamic>().Generate(randomAmount).Select(i => this.GenerateBaseValue(_type)).ToList();
        }

        private object GenerateBaseValue(string _type, string? fakerDirective = null)
        {
            var faker = new Bogus.Faker();
            if (!(fakerDirective is null))
            {
                return faker.GetPropValue(fakerDirective) ?? "Not a valid faker directive";
            }
            return _type switch
            {
                "String" => faker.Person.FirstName,
                "Number" => faker.Random.Int(0, 100),
                "Boolean" => faker.Random.Bool().ToString(),
                "Date" => faker.Person.DateOfBirth.ToShortDateString(),
                "Time" => faker.Person.DateOfBirth.ToShortTimeString(),
                "DateTime" => faker.Person.DateOfBirth.ToString(),
                _ => Generate(_type),
            };
        }

        private static int Rnd(int min, int max)
        {
            return new Random().Next(min, max);
        }

        private static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
    }

    public static class DataHelpers
    {
        public static object? GetPropValue(this object obj, string name)
        {
            foreach (var part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                var info = type.GetProperty(part);
                if (info != null)
                {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    obj = info.GetValue(obj, null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                }
                else
                {
                    var fieldInfo = type.GetField(part);
                    if (fieldInfo != null)
                    {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                        obj = fieldInfo.GetValue(obj);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    }
                }
            }
            return obj;
        }

        public static T GetPropValue<T>(this object obj, String name)
        {
            var retval = GetPropValue(obj, name);
#pragma warning disable CS8653 // A default expression introduces a null value for a type parameter.
            if (retval == null) { return default; }
#pragma warning restore CS8653 // A default expression introduces a null value for a type parameter.

            // throws InvalidCastException if types are incompatible
            return (T)retval;
        }
    }
}
