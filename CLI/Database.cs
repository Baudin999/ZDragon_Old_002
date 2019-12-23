using System;
using System.Collections.Generic;
using System.IO;
using CLI.Models;
using CLI.Signals;
using LiteDB;
using Project;

namespace CLI
{
    public static class Database
    {
        private static string path = Path.Combine(ProjectContext.Instance?.OutPath ?? "", "Lexicon.db");
        private static ConnectionString ConnectionString()
        {
            var connectionString = new ConnectionString(path);
            connectionString.Mode = LiteDB.FileMode.Exclusive;
            return connectionString;
        }
        private static LiteDatabase? Db;
        private static LiteDatabase get()
        {
            if (Db is null)
            {
                Db = new LiteDatabase(ConnectionString());
                SignalSingleton.ExitSignal.Subscribe(() =>
                {
                    Console.WriteLine("Disposing of the database...");
                    Db.Dispose();
                });
            }
            return Db;
        }

        public static IEnumerable<LexiconEntry> GetLexicon(string query)
        {
            var col = get().GetCollection<LexiconEntry>("lexiconEntries");
            if (query == "all:")
            {
                return col.FindAll();
            }
            return col.Find(l => 
                l.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                l.Domain.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                l.Description.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        public static LexiconEntry GetLexiconItem(Guid id)
        {
            var col = get().GetCollection<LexiconEntry>("lexiconEntries");
            return col.FindOne(entry => entry.Id == id);
        }

        public static LexiconEntry AddLexiconEntry(LexiconEntry entry)
        {
            entry.Id = Guid.NewGuid();
            var col = get().GetCollection<LexiconEntry>("lexiconEntries");
            col.Insert(entry);
            return entry;
        }

        public static LexiconEntry UpdateLexiconEntry(LexiconEntry entry)
        {
            var col = get().GetCollection<LexiconEntry>("lexiconEntries");
            col.Update(entry);
            return entry;
        }

        public static void DeleteLexiconEntry(LexiconEntry entry)
        {
            var col = get().GetCollection<LexiconEntry>("lexiconEntries");
            col.Delete(e => e.Id == entry.Id);
        }
    }
}
