using System;
using System.Windows;

namespace NotesApp
{
    public partial class ConfirmActionWindow : Window
    {
        private Action onAccept;

        public ConfirmActionWindow(string title, string body, Action onAccept)
        {
            InitializeComponent();

            this.Title = title;
            this.Body.Text = body;
            this.onAccept = onAccept;
        }

        private void cancelListener(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void acceptListener(object sender, RoutedEventArgs e)
        {
            this.Close();
            this.onAccept();
        }
    }
}
