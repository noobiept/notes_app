using System;
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


namespace notes_app
    {
    public partial class NotesWindow : Window
        {
        static List<string> NOTES = new List<string>();
        static int CURRENT_POSITION = 0;


        public NotesWindow()
            {
            InitializeComponent();

                // start with a single note
            NotesWindow.NOTES.Add( "" );

            this.loadNote( 0 );

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
            var position = NotesWindow.NOTES.Count;
            NotesWindow.NOTES.Add( "" );

            this.loadNote( position );
            }


        private void removeNoteListener( object sender, RoutedEventArgs e )
            {
                // when there's only 1 note, don't remove it, clear it instead
            if ( NotesWindow.NOTES.Count <= 1 )
                {
                NotesWindow.NOTES[ 0 ] = "";
                this.textBox.Text = "";
                this.textBox.Focus();
                }

            else
                {
                NotesWindow.NOTES.RemoveAt( NotesWindow.CURRENT_POSITION );

                var show = NotesWindow.CURRENT_POSITION;

                if ( show >= NotesWindow.NOTES.Count )
                    {
                    show--;
                    }

                this.loadNote( show );
                }
            }


        private void previousNoteListener( object sender, RoutedEventArgs e )
            {
            var previousPosition = NotesWindow.CURRENT_POSITION - 1;

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
            var nextPosition = NotesWindow.CURRENT_POSITION + 1;

            if ( nextPosition >= NotesWindow.NOTES.Count )
                {
                this.textBox.Focus();
                return;
                }

            this.saveCurrentNote();
            this.loadNote( nextPosition );
            }


        private void loadNote( int position )
            {
            NotesWindow.CURRENT_POSITION = position;
            this.textBox.Text = NotesWindow.NOTES[ position ];
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

            if ( position + 1 == NotesWindow.NOTES.Count )
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
            NotesWindow.NOTES[ NotesWindow.CURRENT_POSITION ] = this.textBox.Text;
            }
        }
    }
