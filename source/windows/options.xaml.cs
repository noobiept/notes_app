using System;
using System.Windows;

namespace NotesApp
{
    public partial class OptionsWindow : Window
    {
        private Action onClose;
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
            this.onClose = onClose;
            this.onResetData = onResetData;

            this.setMinimizeOnClose = setMinimizeOnClose;
            this.MinimizeOnClose.Checked += this.minimizeOnCloseListener;
            this.MinimizeOnClose.Unchecked += this.minimizeOnCloseListener;
        }

        private void closeListener(object sender, RoutedEventArgs e)
        {
            this.onClose();
        }

        private void minimizeOnCloseListener(object sender, RoutedEventArgs e)
        {
            var value = this.MinimizeOnClose.IsChecked ?? false;
            this.setMinimizeOnClose(value);
        }

        private void resetDataListener(object sender, RoutedEventArgs e)
        {
            this.onResetData();
        }
    }
}
