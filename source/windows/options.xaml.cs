using System;
using System.ComponentModel;
using System.Windows;

namespace NotesApp
{
    public partial class OptionsWindow : Window
    {
        private Action<Boolean> setMinimizeOnClose;
        private Action onResetData;

        public OptionsWindow(
            Boolean minimizeOnCloseValue,
            Action onClose,
            Action onResetData,
            Action<Boolean> setMinimizeOnClose
        )
        {
            InitializeComponent();

            this.MinimizeOnClose.IsChecked = minimizeOnCloseValue;
            this.onResetData = onResetData;

            this.setMinimizeOnClose = setMinimizeOnClose;
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
            var value = this.MinimizeOnClose.IsChecked ?? false;
            this.setMinimizeOnClose(value);
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
