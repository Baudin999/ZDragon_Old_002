using System;

namespace Mapper.Application
{
    public class Descriptor
    {
        public string Module { get; set; } = "";
        public string Parent { get; internal set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Type { get; set; } = "";
        public string DescriptorType { get; set; } = "";

        public bool Is(string param)
        {
            return this.Module.Contains(param, StringComparison.OrdinalIgnoreCase) ||
                this.Parent.Contains(param, StringComparison.OrdinalIgnoreCase) ||
                this.Name.Contains(param, StringComparison.OrdinalIgnoreCase) ||
                this.Description.Contains(param, StringComparison.OrdinalIgnoreCase) ||
                this.Type.Contains(param, StringComparison.OrdinalIgnoreCase);
        }
    }
}