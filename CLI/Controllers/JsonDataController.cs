using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Bogus;
using Project;
using Compiler.AST;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Configuration;

namespace CLI.Controllers
{
    public class JsonDataController : ControllerBase
    {
        private Module? Module;

        [HttpGet("/api/data/{module}/{type}")]
        public IActionResult GetData(string module, string type, [FromQuery]bool list)
        {

            Module = Project.ProjectContext.Instance?.Modules.First(m => m.Name == module);
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
                var fakerDirective = field.Directives.FirstOrDefault(f => f.Key == "faker")?.Value.ToLower();
                var (_mod, _type) = (field.Types.First().Value, field.Types.Last().Value);
                var data = (_mod, _type) switch
                {
                    ("Maybe", _) => GenerateBaseValue(_type, field, fakerDirective),
                    ("List", _) => GenerateList(_type, field, fakerDirective),
                    (_, _) => GenerateBaseValue(_type, field, fakerDirective)
                };

                AddProperty(result, field.Name, data);
            }
            return result;
        }

        private dynamic GenerateEnum(ASTChoice choice)
        {
            var faker = new Faker();
            return faker.Random.ListItem(choice.Options.ToList()).Value;
        }

        private dynamic GenerateAlias(ASTAlias alias)
        {
            var fakerDirective = alias.Directives.FirstOrDefault(f => f.Key == "faker")?.Value.ToLower();
            var (_mod, _type) = (alias.Types.First().Value, alias.Types.Last().Value);
            return (_mod, _type) switch
            {
                ("Maybe", _) => GenerateBaseValue(_type, alias, fakerDirective),
                ("List", _) => GenerateList(_type, alias, fakerDirective),
                (_, _) => GenerateBaseValue(_type, alias, fakerDirective)
            };
        }

        private List<object> GenerateList(string _type, IRestrictable restrictable, string? fakerDirective = null)
        {
            var randomAmount = new Random().Next(0, 5);
            return new Faker<dynamic>()
                .Generate(randomAmount)
                .Select(i => this.GenerateBaseValue(_type, restrictable, fakerDirective))
                .ToList();
        }

        private object GenerateBaseValue(string _type, IRestrictable restrictable, string? fakerDirective = null)
        {
            var config = ProjectContext.Instance?.CarConfig ?? new CarConfig();
            var faker = new Faker("nl");
            dynamic? result = null;
            
            result = fakerDirective?.ToLower() switch
            {
                // person
                "person.firstname" => faker.Person.FirstName,
                "person.lastname" => faker.Person.LastName,
                "person.fullname" => faker.Person.FullName,
                "person.dateofbirth" => faker.Person.DateOfBirth.ToShortDateString(),
                "person.avatar" => faker.Person.Avatar,
                "person.email" => faker.Person.Email,
                "person.gender" => faker.Person.Gender.ToString("g"),
                "person.phone" => faker.Person.Phone,
                "person.username" => faker.Person.UserName,

                // address 
                "address.street" => faker.Address.StreetName(),
                "address.housenumber" => faker.Address.BuildingNumber(),
                "address.extension" => faker.Random.Char('A', 'Z').ToString(),
                "address.city" => faker.Address.City(),
                "address.postalcode" => faker.Address.ZipCode(),
                "address.country" => faker.Address.Country(),
                "address.countrycode" => faker.Address.CountryCode(),
                "address.fulladdress" => faker.Address.FullAddress(),
                _ => null
            };

            if (result != null) return result;

            var minR = restrictable.Restrictions.FirstOrDefault(r => r.Key == "min")?.Value;
            var maxR = restrictable.Restrictions.FirstOrDefault(r => r.Key == "max")?.Value;

            result = _type switch
            {
                "String" => faker.Lorem.Words(3),
                "Number" => faker.Random.Int(
                    minR == null ? config.DefaultRestrictions.NumberRestrictions.Min : int.Parse(minR),
                    maxR == null ? config.DefaultRestrictions.NumberRestrictions.Max : int.Parse(maxR)).ToString(),
                "Boolean" => faker.Random.Bool().ToString(),
                "Date" => faker.Person.DateOfBirth.ToShortDateString(),
                "Time" => faker.Person.DateOfBirth.ToShortTimeString(),
                "DateTime" => faker.Person.DateOfBirth.ToString(),
                _ => Generate(_type),
            };

            return result;
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


    //public static class BogusWrapper
    //{
    //    private static Faker Bogus { get => new Faker("nl"); }
    //    public static class Person
    //    {
    //        public static string FirstName => new Faker("nl").Person.FirstName;
    //        public static string LastName => BogusWrapper.Bogus.Person.LastName;
    //        public static string FullName => BogusWrapper.Bogus.Person.FullName;
    //        public static string DateOfBirth => BogusWrapper.Bogus.Person.DateOfBirth.ToShortDateString();
    //        public static string Avatar => BogusWrapper.Bogus.Person.Avatar;
    //        public static string Email => BogusWrapper.Bogus.Person.Email;
    //        public static string Gender => BogusWrapper.Bogus.Person.Gender.ToString("g");
    //        public static string Phone => BogusWrapper.Bogus.Person.Phone;
    //        public static string UserName => BogusWrapper.Bogus.Person.UserName;
    //    }
    //    public static class Address
    //    {
    //        public static string Street => BogusWrapper.Bogus.Address.StreetName();
    //        public static string HouseNumber => BogusWrapper.Bogus.Random.Number(1, 1000).ToString();
    //        public static string HouseNumberExtension => BogusWrapper.Bogus.Random.Char('A', 'Z').ToString();
    //        public static string PostalCode => BogusWrapper.Bogus.Address.ZipCode();
    //        public static string City => BogusWrapper.Bogus.Address.City();
    //        public static string Country => BogusWrapper.Bogus.Address.Country();
    //        public static string CountryCode => BogusWrapper.Bogus.Address.CountryCode();
    //        public static string FullAddress => BogusWrapper.Bogus.Address.FullAddress();
    //        public static string County => BogusWrapper.Bogus.Address.County();
    //    }

    //    public static class Finance
    //    {
    //        public static string Number => BogusWrapper.Bogus.Finance.Account();
    //    }
    //}


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
