using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace NotesApp.Models
{
    public class NotesContext : DbContext
    {
        public DbSet<Note> Notes { get; set; } = default!;
        public DbSet<Configuration> Config { get; set; } = default!;

        public string DbPath { get; }

        public NotesContext()
        {
            var dataPath = Environment.SpecialFolder.ApplicationData;
            var path = Path.Join(Environment.GetFolderPath(dataPath), "notes");

            // make sure the directory exist
            Directory.CreateDirectory(path);

            DbPath = Path.Join(path, "notes.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options) =>
            options.UseSqlite($"Data Source={DbPath}");

        public void resetData()
        {
            using (var transaction = this.Database.BeginTransaction())
            {
                this.Notes.RemoveRange(this.Notes);
                this.Config.RemoveRange(this.Config);
                this.SaveChanges();
                this.validateDb();
                transaction.Commit();
            }
        }

        // Make sure there's at least 1 row of notes/configuration.
        public void validateDb()
        {
            var changed = false;

            if (!this.Config.Any())
            {
                var config = Configuration.CreateDefault();
                this.Config.Add(config);

                changed = true;
            }

            if (!this.Notes.Any())
            {
                var note = new Note();
                this.Notes.Add(note);

                changed = true;
            }

            if (changed)
            {
                this.SaveChanges();
            }
        }

        public Configuration getConfig()
        {
            return this.Config.First();
        }
    }
}
