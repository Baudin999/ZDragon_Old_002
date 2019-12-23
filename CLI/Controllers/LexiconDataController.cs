using System;
using System.Collections.Generic;
using CLI.Models;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Project;

namespace CLI.Controllers
{
    public class LexiconDataController : ControllerBase
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

        [HttpGet("/api/lexicon/config")]
        public IActionResult ConfigurationData()
        {
            var project = FileProject.Current;
            if (project is null) return NotFound();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            else return Ok(project.CarConfig.LexiconConfig);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }


        [HttpGet("/api/lexicon/remote")]
        public IActionResult GetRemoteLexiconData()
        {
            var config = FileProject.Current?.CarConfig;
            if (config is null) return NotFound();
            else
            {
                return Ok();
            }
        }
    }   
}
