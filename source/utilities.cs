using System.Diagnostics;
using System;

namespace NotesApp
{
    public struct Utilities
    {
        static public void openExternalUrl(String url)
        {
            System.Diagnostics.Process.Start(
                new ProcessStartInfo { FileName = url, UseShellExecute = true, }
            );
        }
    }
}
