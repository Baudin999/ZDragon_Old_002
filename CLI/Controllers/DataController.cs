using System;
using System.Linq;
using System.Text.Json;
using Compiler.AST;
using Microsoft.AspNetCore.Mvc;
using Bogus;
using System.Dynamic;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Reflection;

namespace CLI.Controllers
{
    public class DataController : ControllerBase
    {
        private Module Module;

        [HttpGet("/api/data/{module}/{type}")]
        public IActionResult GetData(string module, string type)
        {
            Module = Project.Current.Modules.First(m => m.Name == module);
            var node = Find(type);

            if (!(node is null) && node is ASTType t)
            {
                var result = GenerateData(t);
                return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));
            }

            return NotFound();

        }

        private IASTNode Find(string name)
        {
            return Module.Transpiler.AST.FirstOrDefault(m => m is INamable && ((INamable)m).Name == name);
        }

        private dynamic GenerateData(ASTType node)
        {
            dynamic result = new ExpandoObject();
            foreach (var field in node.Fields)
            {
                var fakerDirective = field.Directives.FirstOrDefault(f => f.Key == "faker")?.Value;
                var (_mod, _type) = (field.Type.First().Value, field.Type.Last().Value);
                var data = (_mod, _type) switch
                {
                    ("Maybe", _) => GenerateBaseValue(_type),
                    ("List", _) => GenerateBaseValue(_type),
                    (_, _) => GenerateBaseValue(_type, fakerDirective)
                };

                AddProperty(result, field.Name, data);
            }
            return result;
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
                _ => "No recognised type",
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

    public static class DataHelpers {
        public static Object GetPropValue(this Object obj, String name)
        {
            foreach (var part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                var info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        public static T GetPropValue<T>(this Object obj, String name)
        {
            var retval = GetPropValue(obj, name);
            if (retval == null) { return default(T); }

            // throws InvalidCastException if types are incompatible
            return (T)retval;
        }
    }
}
