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

        public DefaultRestrictions DefaultRestrictions { get; set; } = new DefaultRestrictions();
        public LexiconConfig LexiconConfig { get; set; } = new LexiconConfig();
        public XsdConfig XsdConfig { get; set; } = new XsdConfig();
        public ErdConfig ErdConfig { get; set; } = new ErdConfig();

        public static CarConfig Load(string path)
        {
            if (File.Exists(path))
            {
                var json = IO.ReadFileText(path);
                var serializer = new JsonSerializer();
                var result = serializer.Deserialize<CarConfig>(new JsonTextReader(new StringReader(json)));

                if (result is null)
                {
                    result = new CarConfig();
                    result.Save(path);
                }

                return result;
            }
            else
            {
                var carConfig = new CarConfig();
                carConfig.Save(path);
                return carConfig;
            }
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
}
 