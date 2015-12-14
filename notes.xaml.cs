using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;


namespace NotesApp
    {
    public partial class NotesWindow : Window
        {
        struct Data {
            public List<string> notes;
            public int currentPosition; // position in the notes list, of the current opened note
            public double windowWidth;
            public double windowHeight;
            public double windowLeft;
            public double windowTop;
            public int version;         // version of the data structure (useful when updating from different versions that have incompatible changes)
            public bool isHidden;
            };

        #if DEBUG
            static string FILE_NAME = "data_debug.txt";
        #else
            static string FILE_NAME = "data.txt";
        #endif        
        static string DATA_PATH = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ), "notes_app",  NotesWindow.FILE_NAME );
        
        Data data;
        System.Windows.Forms.NotifyIcon notifyIcon;
        

        public NotesWindow()
            {
            InitializeComponent();

            try {
                StreamReader file = new StreamReader( NotesWindow.DATA_PATH );

                string data = file.ReadToEnd();
                file.Close();

                this.data = JsonConvert.DeserializeObject<Data>( data );

                this.Width = this.data.windowWidth;
                this.Height = this.data.windowHeight;

                var left = this.data.windowLeft;

                if ( left > SystemParameters.PrimaryScreenWidth )
                    {
                    left = SystemParameters.PrimaryScreenWidth - 100;
                    }

                var top = this.data.windowTop;

                if ( top > SystemParameters.PrimaryScreenHeight )
                    {
                    top = SystemParameters.PrimaryScreenHeight - 100;
                    }

                this.Left = left;
                this.Top = top;

                if ( this.data.isHidden == true )
                    {
                    this.hideWindow();
                    }
                }
            
            catch( Exception )
                {
                    // start with a single note
                this.data.notes = new List<string>();
                this.data.notes.Add( "" );
                this.data.currentPosition = 0;
                this.data.isHidden = false;
                }

            this.loadNote( this.data.currentPosition );

                // keyboard shortcuts
            var newNote = new RoutedCommand();
            newNote.InputGestures.Add( new KeyGesture( Key.N, ModifierKeys.Control ) );
            CommandBindings.Add( new CommandBinding( newNote, newNoteListener ) );

            var removeNote = new RoutedCommand();
            removeNote.InputGestures.Add( new KeyGesture( Key.R, ModifierKeys.Control ) );
            CommandBindings.Add( new CommandBinding( removeNote, removeNoteListener ) );

            var previous = new RoutedCommand();
            previous.InputGestures.Add( new KeyGesture( Key.Q, ModifierKeys.Control ) );
            CommandBindings.Add( new CommandBinding( previous, previousNoteListener ) );

            var next = new RoutedCommand();
            next.InputGestures.Add( new KeyGesture( Key.W, ModifierKeys.Control ) );
            CommandBindings.Add( new CommandBinding( next, nextNoteListener ) );

                // 'ctrl + 1' loads first note, 'ctrl + 2' loads second note, etc (from 1 to 9)
            for (int a = 0 ; a < 9 ; a++)
                {
                int position = a;   // get a new variable with the value we need, to be used by the lambda function below
                var load = new RoutedCommand();
                load.InputGestures.Add( new KeyGesture( Key.D1 + a, ModifierKeys.Control ) );
                CommandBindings.Add( new CommandBinding( load, (object sender, ExecutedRoutedEventArgs e) => this.loadNote( position ) ) );
                }
                
            var hide = new RoutedCommand();
            hide.InputGestures.Add( new KeyGesture( Key.Escape ) );
            CommandBindings.Add( new CommandBinding( hide, (object sender, ExecutedRoutedEventArgs e) => { this.hideWindow(); } ) );


                // system tray icon
            var contextMenu = new System.Windows.Forms.ContextMenu();

            var about = new System.Windows.Forms.MenuItem();
            about.Text = "About";
            about.Click += (object sender, EventArgs e) => { System.Diagnostics.Process.Start( "https://bitbucket.org/drk4/notes_app" ); };

            var show = new System.Windows.Forms.MenuItem();
            show.Text = "Show";
            show.Click += (object sender, EventArgs e) => { this.showWindow(); };

            var close = new System.Windows.Forms.MenuItem();
            close.Text = "Close";
            close.Click += (object sender, EventArgs e) => { this.closeWindow(); };

            contextMenu.MenuItems.Add( about );
            contextMenu.MenuItems.Add( show );
            contextMenu.MenuItems.Add( close );

            this.notifyIcon = new System.Windows.Forms.NotifyIcon();
            this.notifyIcon.Text = "Notes App";
            this.notifyIcon.Icon = new System.Drawing.Icon( @"assets/icon.ico" );
            this.notifyIcon.DoubleClick += this.notifyIconClick;
            this.notifyIcon.ContextMenu = contextMenu;
            this.notifyIcon.Visible = true;
            }
        

        private void newNoteListener( object sender, RoutedEventArgs e )
            {
                // a new note is added at the end
            var position = this.data.notes.Count;
            this.data.notes.Add( "" );

            this.loadNote( position );
            }


        private void removeNoteListener( object sender, RoutedEventArgs e )
            {
                // when there's only 1 note, don't remove it, clear it instead
            if ( this.data.notes.Count <= 1 )
                {
                this.data.notes[ 0 ] = "";
                this.textBox.Text = "";
                this.textBox.Focus();
                }

            else
                {
                this.data.notes.RemoveAt( this.data.currentPosition );

                var show = this.data.currentPosition;

                if ( show >= this.data.notes.Count )
                    {
                    show--;
                    }

                this.loadNote( show );
                }
            }


        private void previousNoteListener( object sender, RoutedEventArgs e )
            {
            var previousPosition = this.data.currentPosition - 1;

            if ( previousPosition < 0 )
                {
                this.textBox.Focus();
                return;
                }

            this.loadNote( previousPosition );
            }


        private void nextNoteListener( object sender, RoutedEventArgs e )
            {
            var nextPosition = this.data.currentPosition + 1;

            if ( nextPosition >= this.data.notes.Count )
                {
                this.textBox.Focus();
                return;
                }

            this.loadNote( nextPosition );
            }


        private void loadNote( int position )
            {
            if ( position >= this.data.notes.Count )
                {
                position = this.data.notes.Count - 1;
                }

            this.data.currentPosition = position;
            this.textBox.Text = this.data.notes[ position ];
            this.textBox.Focus();

            this.Title = "Notes - " + (position + 1);

            if ( position == 0 )
                {
                this.previous.IsEnabled = false;
                }

            else
                {
                this.previous.IsEnabled = true;
                }

            if ( position + 1 == this.data.notes.Count )
                {
                this.next.IsEnabled = false;
                }

            else
                {
                this.next.IsEnabled = true;
                }
            }


        private void textChanged( object sender, TextChangedEventArgs e )
            {
                // save the current note when there's a change
            this.data.notes[ this.data.currentPosition ] = this.textBox.Text;
            }


        public void saveToDisk()
            {
            this.data.windowWidth = this.Width;
            this.data.windowHeight = this.Height;
            this.data.windowLeft = this.Left;
            this.data.windowTop = this.Top;
            this.data.version = 1;

            string data = JsonConvert.SerializeObject( this.data );

                // make sure there's a directory created (otherwise the stream writer call will fail)
            System.IO.Directory.CreateDirectory( Path.GetDirectoryName( NotesWindow.DATA_PATH ) );
            StreamWriter file = new StreamWriter( NotesWindow.DATA_PATH );

            file.Write( data );
            file.Close();
            }


        private void windowClosing( object sender, System.ComponentModel.CancelEventArgs e )
            {
            e.Cancel = true;
            this.hideWindow();
            }


        private void showWindow()
            {
            this.data.isHidden = false;
            this.Show();
            this.Activate();
            }


        private void hideWindow()
            {
            this.data.isHidden = true;
            this.Hide();
            }


        private void notifyIconClick(object sender, EventArgs e)
            {
            if ( this.data.isHidden == false )
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
