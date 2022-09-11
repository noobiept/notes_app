using System.Windows;

namespace NotesApp
{
    public partial class App : Application
    {
        private void applicationSessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            var notesWindow = (NotesWindow)this.MainWindow;

            notesWindow.saveWindowPositionDimension();
        }
    }
}
