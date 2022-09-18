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
        System.Windows.Forms.NotifyIcon notifyIcon = default!;
        OptionsWindow? optionsWindow = null;

        public NotesWindow()
        {
            InitializeComponent();

            using (var db = new NotesContext())
            {
                db.Database.Migrate();
                db.validateDb();

                var config = db.getConfig();

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
                    this.hideAllWindows(db);
                }

                if (config.AlwaysOnTop == true)
                {
                    this.setAlwaysOnTop(true);
                }

                this.loadNote(db, config.CurrentNotePosition);
            }

            this.setupKeyboardShortcuts();
            this.setupSystemTray();
        }

        private void setupSystemTray()
        {
            var contextMenu = new System.Windows.Forms.ContextMenuStrip();

            var about = new System.Windows.Forms.ToolStripMenuItem();
            about.Text = "About";
            about.Click += (object? sender, EventArgs e) =>
            {
                Utilities.openExternalUrl(Constants.aboutUrl);
            };

            var show = new System.Windows.Forms.ToolStripMenuItem();
            show.Text = "Show";
            show.Click += onShowWindowsListener;

            var close = new System.Windows.Forms.ToolStripMenuItem();
            close.Text = "Close";
            close.Click += this.notifyIconCloseAppListener;

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

        private void setupKeyboardShortcuts()
        {
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
                        (object sender, ExecutedRoutedEventArgs e) =>
                            this.loadNoteListener(position)
                    )
                );
            }

            var hide = new RoutedCommand();
            hide.InputGestures.Add(new KeyGesture(Key.Escape));
            CommandBindings.Add(new CommandBinding(hide, this.onEscPress));

            var options = new RoutedCommand();
            options.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(options, openOptions));
        }

        private void loadNoteListener(int position)
        {
            using (var db = new NotesContext())
            {
                this.loadNote(db, position);
                db.SaveChanges();
            }
        }

        private void onShowWindowsListener(object? sender, EventArgs e)
        {
            using (var db = new NotesContext())
            {
                this.showAllWindows(db);
            }
        }

        private void onEscPress(object? sender, ExecutedRoutedEventArgs e)
        {
            using (var db = new NotesContext())
            {
                var config = db.getConfig();

                if (config.MinimizeOnClose)
                {
                    this.hideAllWindows(db);
                    db.SaveChanges();
                }
            }
        }

        private void newNoteListener(object sender, RoutedEventArgs e)
        {
            using (var db = new NotesContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    // a new note is added at the end
                    db.Notes.Add(new Note());
                    db.SaveChanges();

                    var position = db.Notes.Count() - 1;
                    this.loadNote(db, position);
                    db.SaveChanges();
                    transaction.Commit();
                }
            }
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
            using (var db = new NotesContext())
            {
                var config = db.getConfig();
                var previousPosition = config.CurrentNotePosition - 1;

                if (previousPosition < 0)
                {
                    this.textBox.Focus();
                    return;
                }

                this.loadNote(db, previousPosition);
                db.SaveChanges();
            }
        }

        private void nextNoteListener(object sender, RoutedEventArgs e)
        {
            using (var db = new NotesContext())
            {
                var config = db.getConfig();
                var nextPosition = config.CurrentNotePosition + 1;

                if (nextPosition >= db.Notes.Count())
                {
                    this.textBox.Focus();
                    return;
                }

                this.loadNote(db, nextPosition);
                db.SaveChanges();
            }
        }

        private Note getNoteAt(NotesContext db, int position)
        {
            return db.Notes.ToList().ElementAt(position);
        }

        private void notifyIconCloseAppListener(object? sender, EventArgs e)
        {
            this.shutdownApp();
        }

        private void alwaysOnTopListener(object? sender, EventArgs e)
        {
            this.setAlwaysOnTop(!this.Topmost);
        }

        private void setAlwaysOnTop(bool value)
        {
            using (var db = new NotesContext())
            {
                var config = db.getConfig();

                this.Topmost = value;
                config.AlwaysOnTop = value;
                this.AlwaysOnTopItem.IsChecked = value;
                db.SaveChanges();
            }
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
                    onClose: this.optionsWindowClosed,
                    onResetData: this.resetData,
                    setAlwaysOnTop: setAlwaysOnTop
                );
                this.optionsWindow.Owner = this;
                this.optionsWindow.Show();
            }
        }

        private void optionsWindowClosed()
        {
            this.optionsWindow = null;
            this.Activate();
        }

        private void resetData()
        {
            using (var db = new NotesContext())
            {
                db.resetData();
                this.loadNote(db, 0);
            }
        }

        private void removeCurrentNote()
        {
            using (var db = new NotesContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    var position = 0;

                    // need to make sure there's always a note created, so when removing the last note add a new one
                    if (db.Notes.Count() <= 1)
                    {
                        var first = db.Notes.First();
                        db.Notes.Remove(first);

                        var newFirst = db.Notes.Add(new Note());
                        db.SaveChanges();
                    }
                    else
                    {
                        var config = db.getConfig();
                        var note = this.getNoteAt(db, config.CurrentNotePosition);
                        db.Notes.Remove(note);
                        db.SaveChanges();

                        position = config.CurrentNotePosition;

                        if (position >= db.Notes.Count())
                        {
                            position--;
                        }
                    }

                    this.loadNote(db, position);

                    db.SaveChanges();
                    transaction.Commit();
                }
            }
        }

        private void loadNote(NotesContext db, int position)
        {
            var config = db.getConfig();
            var count = db.Notes.Count();

            if (position >= count)
            {
                position = count - 1;
            }

            var note = this.getNoteAt(db, position);

            config.CurrentNotePosition = position;

            // disable the text changed listener so it doesn't interfere with the loading of the next note
            this.setTextChangedListener(false);
            this.textBox.Text = note.Content;
            this.textBox.Focus();
            this.setTextChangedListener(true);

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
            using (var db = new NotesContext())
            {
                var config = db.getConfig();
                var note = this.getNoteAt(db, config.CurrentNotePosition);

                // save the current note when there's a change
                note.Content = this.textBox.Text;
                note.Modified = DateTime.Now;
                db.SaveChanges();
            }
        }

        private void setTextChangedListener(bool enable)
        {
            if (enable)
            {
                this.textBox.TextChanged += this.textChanged;
            }
            else
            {
                this.textBox.TextChanged -= this.textChanged;
            }
        }

        public void saveWindowPositionDimension()
        {
            using (var db = new NotesContext())
            {
                var config = db.getConfig();
                config.WindowWidth = this.Width;
                config.WindowHeight = this.Height;
                config.WindowLeft = this.Left;
                config.WindowTop = this.Top;

                db.SaveChanges();
            }
        }

        private void windowClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            using (var db = new NotesContext())
            {
                var config = db.getConfig();

                if (config.MinimizeOnClose)
                {
                    e.Cancel = true;
                    this.hideAllWindows(db);
                    db.SaveChanges();
                }
                else
                {
                    this.shutdownApp();
                }
            }
        }

        private void showAllWindows(NotesContext db)
        {
            var config = db.getConfig();

            config.IsHidden = false;

            foreach (Window window in App.Current.Windows)
            {
                window.Show();
            }

            this.Activate();
        }

        private void hideAllWindows(NotesContext db)
        {
            var config = db.getConfig();
            config.IsHidden = true;

            foreach (Window window in App.Current.Windows)
            {
                window.Hide();
            }
        }

        private void notifyIconClick(object? sender, EventArgs e)
        {
            using (var db = new NotesContext())
            {
                var config = db.getConfig();

                if (config.IsHidden == false)
                {
                    this.hideAllWindows(db);
                }
                else
                {
                    this.showAllWindows(db);
                }

                db.SaveChanges();
            }
        }

        private void shutdownApp()
        {
            this.saveWindowPositionDimension();
            this.notifyIcon.Visible = false;
            this.Closing -= this.windowClosing;
            Application.Current.Shutdown();
        }
    }
}
