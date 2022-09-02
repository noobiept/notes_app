using System;
using System.Windows;

namespace NotesApp
{
    public partial class OptionsWindow : Window
    {
        private Action onClose;
        private Action onToggleMinimizeOnClose;
        private Action onResetData;

        public OptionsWindow(Action onClose, Action onResetData, Action onToggleMinimizeOnClose)
        {
            InitializeComponent();

            this.onClose = onClose;
            this.onResetData = onResetData;
            this.onToggleMinimizeOnClose = onToggleMinimizeOnClose;
        }

        private void onCloseListener(object sender, RoutedEventArgs e)
        {
            this.onClose();
        }

        private void onToggleMinimizeOnCloseListener(object sender, RoutedEventArgs e)
        {
            this.onToggleMinimizeOnClose();
        }

        private void onResetDataListener(object sender, RoutedEventArgs e)
        {
            this.onResetData();
        }
    }
}
