using System;
using System.Collections.Generic;
using CLI.Models;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace CLI.Controllers
{
    public class LexiconDataController
    {
        [HttpGet("/api/lexicon")]
        public IEnumerable<LexiconEntry> GetAll([FromQuery]string query)
        {
            return Database.GetLexicon(query);
        }

        [HttpGet("/api/lexicon/{id}")]
        public LexiconEntry Get(Guid id)
        {
            return Database.GetLexiconItem(id);
        }

        [HttpPost("/api/lexicon")]
        public LexiconEntry Insert([FromBody]LexiconEntry entry)
        {
            return Database.AddLexiconEntry(entry);
        }

        [HttpPut("/api/lexicon")]
        public LexiconEntry Update([FromBody]LexiconEntry entry)
        {
            return Database.UpdateLexiconEntry(entry);
        }

        [HttpDelete("/api/lexicon")]
        public void Delete([FromBody]LexiconEntry entry)
        {
            Database.DeleteLexiconEntry(entry);
        }
    }   
}
