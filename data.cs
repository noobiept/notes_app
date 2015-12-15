using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace NotesApp
    {
    public struct Data
        {
        #if DEBUG
            const string FILE_NAME = "data_debug.txt";
        #else
            const string FILE_NAME = "data.txt";
        #endif
        static string DATA_PATH = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ), "notes_app", Data.FILE_NAME );

        const int APP_VERSION = 1;  // current version of the application data

        public List<string> notes;
        public int currentPosition; // position in the notes list, of the current opened note
        public double windowWidth;
        public double windowHeight;
        public double windowLeft;
        public double windowTop;
        public int version;         // version of the loaded data structure (useful to compare with the application version, when updating from different versions that have incompatible changes)
        public bool isHidden;


        static public void load( out Data data )
            {
            try
                {
                StreamReader file = new StreamReader( Data.DATA_PATH );

                string jsonData = file.ReadToEnd();
                file.Close();

                data = JsonConvert.DeserializeObject<Data>( jsonData );

                if ( data.version != Data.APP_VERSION )
                    {
                    Data.update( ref data );
                    }
                }

            catch( Exception )
                {
                Data.loadDefaults( out data );
                }
            }


        static public void loadDefaults( out Data data )
            {
            data = new Data();

            data.notes = new List<string>();
            data.notes.Add( "" );
            data.currentPosition = 0;
            data.windowWidth = -1;
            data.windowHeight = -1;
            data.windowLeft = -1;
            data.windowTop = -1;
            data.version = Data.APP_VERSION;
            data.isHidden = false;
            }


        /**
         * Update the data from a previous version.
         */
        public static void update( ref Data data )
            {
                //HERE
            }


        public static void save( ref Data data )
            {
            string dataJson = JsonConvert.SerializeObject( data );

                // make sure there's a directory created (otherwise the stream writer call will fail)
            System.IO.Directory.CreateDirectory( Path.GetDirectoryName( Data.DATA_PATH ) );
            StreamWriter file = new StreamWriter( Data.DATA_PATH );

            file.Write( dataJson );
            file.Close();
            }
        }
    }
