using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Configuration
{
    public class CarConfig
    {
        public string Version { get; set; } = "v0";
        public string PortNumber { get; set; } = "5000";
        public string Remote { get; set; } = "";

        
        public LexiconConfig LexiconConfig { get; set; } = new LexiconConfig();
        public XsdConfig XsdConfig { get; set; } = new XsdConfig();
        public ErdConfig ErdConfig { get; set; } = new ErdConfig();

        public static CarConfig? Load(string path)
        {

            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var serializer = new JsonSerializer();
                var result = serializer.Deserialize<CarConfig>(new JsonTextReader(new StringReader(json)));
                return result;
            }
            return null;
        }

        public void Save(string path)
        {
            using (var file = File.CreateText(path))
            {
                var serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, this);
            }
        }

    }

    public class LexiconConfig
    {
        public List<string> FunctionalOwners { get; set; } = new List<string>();
        public List<string> TechnicalOwners { get; set; } = new List<string>();
        public List<string> Domains { get; set; } = new List<string>();
    }

    public class XsdConfig
    {
        public bool Enable { get; set; } = true;
    }

    public class ErdConfig
    {
        public bool ShowExtendedFields { get; set; } = true;
    }


}
 