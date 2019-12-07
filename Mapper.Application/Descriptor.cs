using System;

namespace Mapper.Application
{
    public class Descriptor
    {
        public string Module { get; set; } = "";
        public string Parent { get; internal set; } = "";
        public string Name { get; set; } = "";
        public string Title { get; set; }
        public string Description { get; set; } = "";
        public string Type { get; set; } = "";
        public string DescriptorType { get; set; } = "";

        public Descriptor() { }
        public Descriptor(string title)
        {
            this.Title = title;
        }

        public bool Is(string param)
        {
            return this.Name.Contains(param, StringComparison.OrdinalIgnoreCase);
        }
    }
}