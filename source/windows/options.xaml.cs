using NotesApp.Models;
using System;
using System.ComponentModel;
using System.Windows;

namespace NotesApp
{
    public partial class OptionsWindow : Window
    {
        private Action onResetData;

        public OptionsWindow(Action onClose, Action onResetData)
        {
            InitializeComponent();

            using (var db = new NotesContext())
            {
                var config = db.getConfig();
                this.MinimizeOnClose.IsChecked = config.MinimizeOnClose;
            }

            this.onResetData = onResetData;

            this.MinimizeOnClose.Checked += this.minimizeOnCloseListener;
            this.MinimizeOnClose.Unchecked += this.minimizeOnCloseListener;

            this.Closing += (object sender, CancelEventArgs e) => onClose();
        }

        private void closeListener(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void minimizeOnCloseListener(object sender, RoutedEventArgs e)
        {
            using (var db = new NotesContext())
            {
                var value = this.MinimizeOnClose.IsChecked ?? false;
                var config = db.getConfig();
                config.MinimizeOnClose = value;
                db.SaveChanges();
            }
        }

        private void resetDataListener(object sender, RoutedEventArgs e)
        {
            var confirmReset = new ConfirmActionWindow(
                title: "Confirm data reset",
                body: "Do you want to reset all data?",
                onAccept: () => this.onResetData()
            );
            confirmReset.Owner = this;
            confirmReset.ShowDialog();
        }
    }
}
