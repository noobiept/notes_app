using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;


namespace NotesApp
    {
    public partial class NotesWindow : Window
        {
        public struct Data {
            public List<string> notes;
            public int currentPosition;
            public double windowWidth;
            public double windowHeight;
            public double windowLeft;
            public double windowTop;
        };

        static string FILE_NAME = "data.txt";
        static public Data DATA;

        System.Windows.Forms.NotifyIcon notifyIcon;
        bool isHidden;


        public NotesWindow()
            {
            InitializeComponent();

            this.isHidden = false;

            try {
                StreamReader file = new StreamReader( NotesWindow.FILE_NAME );

                string data = file.ReadToEnd();
                file.Close();

                NotesWindow.DATA = JsonConvert.DeserializeObject<Data>( data );

                this.Width = NotesWindow.DATA.windowWidth;
                this.Height = NotesWindow.DATA.windowHeight;

                var left = NotesWindow.DATA.windowLeft;

                if ( left > SystemParameters.PrimaryScreenWidth )
                    {
                    left = SystemParameters.PrimaryScreenWidth - 100;
                    }

                var top = NotesWindow.DATA.windowTop;

                if ( top > SystemParameters.PrimaryScreenHeight )
                    {
                    top = SystemParameters.PrimaryScreenHeight - 100;
                    }

                this.Left = left;
                this.Top = top;
                }
            
            catch( Exception )
                {
                    // start with a single note
                NotesWindow.DATA.notes = new List<string>();
                NotesWindow.DATA.notes.Add( "" );
                NotesWindow.DATA.currentPosition = 0;
                }

            this.loadNote( NotesWindow.DATA.currentPosition );

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
                CommandBindings.Add( new CommandBinding( load, ( object sender, ExecutedRoutedEventArgs e ) => this.loadNote( position ) ) );
                }

                // system tray icon
            var contextMenu = new System.Windows.Forms.ContextMenu();

            var close = new System.Windows.Forms.MenuItem();
            close.Text = "Close";
            close.Click += (object sender, EventArgs e) => { this.closeWindow(); };

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
            var position = NotesWindow.DATA.notes.Count;
            NotesWindow.DATA.notes.Add( "" );

            this.loadNote( position );
            }


        private void removeNoteListener( object sender, RoutedEventArgs e )
            {
                // when there's only 1 note, don't remove it, clear it instead
            if ( NotesWindow.DATA.notes.Count <= 1 )
                {
                NotesWindow.DATA.notes[ 0 ] = "";
                this.textBox.Text = "";
                this.textBox.Focus();
                }

            else
                {
                NotesWindow.DATA.notes.RemoveAt( NotesWindow.DATA.currentPosition );

                var show = NotesWindow.DATA.currentPosition;

                if ( show >= NotesWindow.DATA.notes.Count )
                    {
                    show--;
                    }

                this.loadNote( show );
                }
            }


        private void previousNoteListener( object sender, RoutedEventArgs e )
            {
            var previousPosition = NotesWindow.DATA.currentPosition - 1;

            if ( previousPosition < 0 )
                {
                this.textBox.Focus();
                return;
                }

            this.loadNote( previousPosition );
            }


        private void nextNoteListener( object sender, RoutedEventArgs e )
            {
            var nextPosition = NotesWindow.DATA.currentPosition + 1;

            if ( nextPosition >= NotesWindow.DATA.notes.Count )
                {
                this.textBox.Focus();
                return;
                }

            this.loadNote( nextPosition );
            }


        private void loadNote( int position )
            {
            if ( position >= NotesWindow.DATA.notes.Count )
                {
                position = NotesWindow.DATA.notes.Count - 1;
                }

            NotesWindow.DATA.currentPosition = position;
            this.textBox.Text = NotesWindow.DATA.notes[ position ];
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

            if ( position + 1 == NotesWindow.DATA.notes.Count )
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
            NotesWindow.DATA.notes[ NotesWindow.DATA.currentPosition ] = this.textBox.Text;
            }


        private void saveToDisk()
            {
            NotesWindow.DATA.windowWidth = this.Width;
            NotesWindow.DATA.windowHeight = this.Height;
            NotesWindow.DATA.windowLeft = this.Left;
            NotesWindow.DATA.windowTop = this.Top;

            string data = JsonConvert.SerializeObject( NotesWindow.DATA );

            StreamWriter file = new StreamWriter( NotesWindow.FILE_NAME );

            file.Write( data );
            file.Close();
            }


        private void windowClosing( object sender, System.ComponentModel.CancelEventArgs e )
            {
            e.Cancel = true;
            this.isHidden = true;
            this.Hide();
            }


        private void notifyIconClick(object sender, EventArgs e)
            {
            if ( this.isHidden == false )
                {
                this.isHidden = true;
                this.Hide();
                }

            else
                {
                this.isHidden = false;
                this.Show();
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
