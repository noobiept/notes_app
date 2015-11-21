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


namespace notes_app
    {
    public partial class NotesWindow : Window
        {
        public struct Data {
            public List<string> notes;
            public int currentPosition;
        };

        static string FILE_NAME = "data.txt";
        static public Data DATA;        


        public NotesWindow()
            {
            InitializeComponent();
            
            try {
                StreamReader file = new StreamReader( NotesWindow.FILE_NAME );

                string data = file.ReadToEnd();
                file.Close();

                NotesWindow.DATA = JsonConvert.DeserializeObject<Data>( data );
                }
            
            catch(Exception e)
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
            }
        

        private void newNoteListener( object sender, RoutedEventArgs e )
            {
            this.saveCurrentNote();

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

            this.saveCurrentNote();
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

            this.saveCurrentNote();
            this.loadNote( nextPosition );
            }


        private void loadNote( int position )
            {
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


        private void saveCurrentNote()
            {
            NotesWindow.DATA.notes[ NotesWindow.DATA.currentPosition ] = this.textBox.Text;
            }


        private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
            {
            this.saveCurrentNote();

                // save to disk
            string data = JsonConvert.SerializeObject( NotesWindow.DATA );

            StreamWriter file = new StreamWriter( NotesWindow.FILE_NAME );

            file.Write( data );
            file.Close();
            }
        }
    }
