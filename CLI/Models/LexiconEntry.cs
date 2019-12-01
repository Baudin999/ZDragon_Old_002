using System;
using LiteDB;

namespace CLI.Models
{
    public class LexiconEntry
    {
        public Guid Id { get; set; } 
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Version { get; set; }
    }
}
