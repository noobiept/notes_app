using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NotesApp.Models;

namespace NotesApp
{
    public partial class NotesWindow : Window
    {
        NotesContext db = new NotesContext();
        System.Windows.Forms.NotifyIcon notifyIcon;
        OptionsWindow optionsWindow;

        public NotesWindow()
        {
            InitializeComponent();

            this.db.Database.Migrate();
            this.db.validateDb();

            var config = this.getConfig();

            if (config.WindowWidth > 0)
            {
                this.Width = config.WindowWidth;
            }

            if (config.WindowHeight > 0)
            {
                this.Height = config.WindowHeight;
            }

            var left = config.WindowLeft;
            if (left > 0)
            {
                if (left > SystemParameters.PrimaryScreenWidth)
                {
                    left = SystemParameters.PrimaryScreenWidth - 100;
                }

                this.Left = left;
            }

            var top = config.WindowTop;
            if (top > 0)
            {
                if (top > SystemParameters.PrimaryScreenHeight)
                {
                    top = SystemParameters.PrimaryScreenHeight - 100;
                }

                this.Top = top;
            }

            if (config.IsHidden == true)
            {
                this.hideWindow();
            }

            if (config.AlwaysOnTop == true)
            {
                this.setAlwaysOnTop(true);
            }

            this.loadNote(config.CurrentNotePosition);

            // keyboard shortcuts
            var newNote = new RoutedCommand();
            newNote.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(newNote, newNoteListener));

            var removeNote = new RoutedCommand();
            removeNote.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(removeNote, removeNoteListener));

            var previous = new RoutedCommand();
            previous.InputGestures.Add(new KeyGesture(Key.Q, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(previous, previousNoteListener));

            var next = new RoutedCommand();
            next.InputGestures.Add(new KeyGesture(Key.W, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(next, nextNoteListener));

            // 'ctrl + 1' loads first note, 'ctrl + 2' loads second note, etc (from 1 to 9)
            for (int a = 0; a < 9; a++)
            {
                int position = a; // get a new variable with the value we need, to be used by the lambda function below
                var load = new RoutedCommand();
                load.InputGestures.Add(new KeyGesture(Key.D1 + a, ModifierKeys.Control));
                CommandBindings.Add(
                    new CommandBinding(
                        load,
                        (object sender, ExecutedRoutedEventArgs e) => this.loadNote(position)
                    )
                );
            }

            var hide = new RoutedCommand();
            hide.InputGestures.Add(new KeyGesture(Key.Escape));
            CommandBindings.Add(
                new CommandBinding(
                    hide,
                    (object sender, ExecutedRoutedEventArgs e) =>
                    {
                        this.hideWindow();
                    }
                )
            );

            // system tray icon
            var contextMenu = new System.Windows.Forms.ContextMenuStrip();

            var about = new System.Windows.Forms.ToolStripMenuItem();
            about.Text = "About";
            about.Click += (object sender, EventArgs e) =>
            {
                Utilities.openExternalUrl(Constants.aboutUrl);
            };

            var show = new System.Windows.Forms.ToolStripMenuItem();
            show.Text = "Show";
            show.Click += (object sender, EventArgs e) =>
            {
                this.showWindow();
            };

            var close = new System.Windows.Forms.ToolStripMenuItem();
            close.Text = "Close";
            close.Click += this.closeWindowListener;

            contextMenu.Items.Add(about);
            contextMenu.Items.Add(show);
            contextMenu.Items.Add(close);

            this.notifyIcon = new System.Windows.Forms.NotifyIcon();
            this.notifyIcon.Text = "Notes App";
            this.notifyIcon.Icon = new System.Drawing.Icon(@"assets/icon.ico");
            this.notifyIcon.DoubleClick += this.notifyIconClick;
            this.notifyIcon.ContextMenuStrip = contextMenu;
            this.notifyIcon.Visible = true;
        }

        private void newNoteListener(object sender, RoutedEventArgs e)
        {
            // a new note is added at the end
            this.db.Add(new Note());
            var position = this.db.Notes.Count() - 1;

            this.loadNote(position);
        }

        private void removeNoteListener(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Do you want to remove this note?",
                String.Format("Remove \"{0}\"?", this.Title),
                MessageBoxButton.OKCancel
            );

            if (result == MessageBoxResult.OK)
            {
                this.removeCurrentNote();
            }
        }

        private void previousNoteListener(object sender, RoutedEventArgs e)
        {
            var config = this.getConfig();
            var previousPosition = config.CurrentNotePosition - 1;

            if (previousPosition < 0)
            {
                this.textBox.Focus();
                return;
            }

            this.loadNote(previousPosition);
        }

        private void nextNoteListener(object sender, RoutedEventArgs e)
        {
            var config = this.getConfig();
            var nextPosition = config.CurrentNotePosition + 1;

            if (nextPosition >= this.db.Notes.Count())
            {
                this.textBox.Focus();
                return;
            }

            this.loadNote(nextPosition);
        }

        private Configuration getConfig()
        {
            return this.db.Config.First();
        }

        private Note getNoteAt(int position)
        {
            return this.db.Notes.ToList().ElementAt(position);
        }

        private void closeWindowListener(object sender, EventArgs e)
        {
            this.closeWindow();
        }

        private void alwaysOnTopListener(object sender, EventArgs e)
        {
            this.setAlwaysOnTop(!this.Topmost);
        }

        private void setAlwaysOnTop(bool value)
        {
            var config = this.getConfig();

            this.Topmost = value;
            config.AlwaysOnTop = value;
            this.AlwaysOnTopItem.IsChecked = value;
        }

        private void openOptions(object sender, EventArgs e)
        {
            if (this.optionsWindow != null)
            {
                this.optionsWindow.Activate();
            }
            else
            {
                this.optionsWindow = new OptionsWindow(
                    onClose: this.closeOptionsWindow,
                    onResetData: this.resetData,
                    onToggleMinimizeOnClose: this.toggleMinimizeOnClose
                );
                this.optionsWindow.Show();
            }
        }

        private void closeOptionsWindow()
        {
            if (this.optionsWindow != null)
            {
                this.optionsWindow.Close();
                this.optionsWindow = null;
            }
        }

        private void resetData() { }

        private void toggleMinimizeOnClose() { }

        private void removeCurrentNote()
        {
            var config = this.getConfig();

            // when there's only 1 note, don't remove it, clear it instead
            if (this.db.Notes.Count() <= 1)
            {
                var first = this.db.Notes.First();
                first.Content = "";
                this.textBox.Text = "";
                this.textBox.Focus();
            }
            else
            {
                var note = this.getNoteAt(config.CurrentNotePosition);
                this.db.Notes.Remove(note);

                var show = config.CurrentNotePosition;

                if (show >= this.db.Notes.Count())
                {
                    show--;
                }

                this.loadNote(show);
            }

            this.db.SaveChanges();
        }

        private void loadNote(int position)
        {
            var config = this.getConfig();
            var count = this.db.Notes.Count();

            if (position >= count)
            {
                position = count - 1;
            }

            var note = this.getNoteAt(position);

            config.CurrentNotePosition = position;

            this.textBox.Text = note.Content;
            this.textBox.Focus();

            this.Title = String.Format("Notes - {0}", position + 1);

            if (position == 0)
            {
                this.previous.IsEnabled = false;
            }
            else
            {
                this.previous.IsEnabled = true;
            }

            if (position + 1 == count)
            {
                this.next.IsEnabled = false;
            }
            else
            {
                this.next.IsEnabled = true;
            }
        }

        private void textChanged(object sender, TextChangedEventArgs e)
        {
            var config = this.getConfig();
            var note = this.getNoteAt(config.CurrentNotePosition);

            // save the current note when there's a change
            note.Content = this.textBox.Text;
        }

        public void saveToDisk()
        {
            var config = this.getConfig();
            config.WindowWidth = this.Width;
            config.WindowHeight = this.Height;
            config.WindowLeft = this.Left;
            config.WindowTop = this.Top;

            this.db.SaveChanges();
        }

        private void windowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.hideWindow();
        }

        private void showWindow()
        {
            var config = this.getConfig();

            config.IsHidden = false;
            this.Show();
            this.Activate();
        }

        private void hideWindow()
        {
            var config = this.getConfig();

            config.IsHidden = true;
            this.Hide();
        }

        private void notifyIconClick(object sender, EventArgs e)
        {
            var config = this.getConfig();

            if (config.IsHidden == false)
            {
                this.hideWindow();
            }
            else
            {
                this.showWindow();
            }
        }

        private void closeWindow()
        {
            this.saveToDisk();
            this.notifyIcon.Visible = false;
            Application.Current.Shutdown();
        }
    }
}
