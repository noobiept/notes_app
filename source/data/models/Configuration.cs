namespace NotesApp.Models
{
    public class Configuration
    {
        public int Id { get; set; }
        public int CurrentNotePosition { get; set; }
        public double WindowWidth { get; set; }
        public double WindowHeight { get; set; }
        public double WindowLeft { get; set; }
        public double WindowTop { get; set; }
        public bool IsHidden { get; set; }
        public bool AlwaysOnTop { get; set; }
        public bool MinimizeOnClose { get; set; }

        public static Configuration CreateDefault()
        {
            var config = new Configuration();
            config.CurrentNotePosition = 0;
            config.WindowWidth = -1;
            config.WindowHeight = -1;
            config.WindowLeft = -1;
            config.WindowTop = -1;
            config.IsHidden = false;
            config.AlwaysOnTop = false;
            config.MinimizeOnClose = false;

            return config;
        }
    }
}
