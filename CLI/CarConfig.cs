using System;
namespace CLI
{
    public class CarConfig
    {
        public string Path { get; set; }

        public CarConfig(string path)
        {
            this.Path = path;
        }

    }
}
