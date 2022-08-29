using System;

namespace NotesApp.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public Note()
        {
            this.Content = "";
            this.Created = DateTime.Now;
            this.Modified = DateTime.Now;
        }
    }
}
