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
            }
        

        private void newNoteListener( object sender, RoutedEventArgs e )
            {
            this.saveCurrentNote();

                // a new note is added at the end
            var position = NotesWindow.NOTES.Count;
            NotesWindow.NOTES.Add( "" );

            this.loadNote( position );
            }


        private void previousNoteListener( object sender, RoutedEventArgs e )
            {
            var previousPosition = NotesWindow.CURRENT_POSITION - 1;

            if ( previousPosition < 0 )
                {
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
                return;
                }

            this.saveCurrentNote();
            this.loadNote( nextPosition );
            }


        private void loadNote( int position )
            {
            NotesWindow.CURRENT_POSITION = position;
            this.textBox.Text = NotesWindow.NOTES[ position ];

            this.Title = "Notes - " + (position + 1);
            }


        private void saveCurrentNote()
            {
            NotesWindow.NOTES[ NotesWindow.CURRENT_POSITION ] = this.textBox.Text;
            }
        }
    }
