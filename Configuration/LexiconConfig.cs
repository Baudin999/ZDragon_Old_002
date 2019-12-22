using System;
using System.Collections.Generic;

namespace Configuration
{
    public class LexiconConfig
    {
        public List<string> FunctionalOwners { get; set; } = new List<string>();
        public List<string> TechnicalOwners { get; set; } = new List<string>();
        public List<string> Domains { get; set; } = new List<string>();
    }
}
