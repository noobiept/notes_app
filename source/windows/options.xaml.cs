﻿using NotesApp.Models;
using System;
using System.ComponentModel;
using System.Windows;

namespace NotesApp
{
    public partial class OptionsWindow : Window
    {
        private Action onResetData;

        public OptionsWindow(Action onClose, Action onResetData, Action<bool> setAlwaysOnTop)
        {
            InitializeComponent();

            this.updateState();
            this.onResetData = onResetData;

            this.MinimizeOnClose.Checked += this.minimizeOnCloseListener;
            this.MinimizeOnClose.Unchecked += this.minimizeOnCloseListener;

            this.AlwaysOnTop.Checked += (object sender, RoutedEventArgs e) => setAlwaysOnTop(true);
            this.AlwaysOnTop.Unchecked += (object sender, RoutedEventArgs e) =>
                setAlwaysOnTop(false);

            this.Closing += (object? sender, CancelEventArgs e) => onClose();
        }

        private void closeListener(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void updateState()
        {
            using (var db = new NotesContext())
            {
                var config = db.getConfig();
                this.MinimizeOnClose.IsChecked = config.MinimizeOnClose;
                this.AlwaysOnTop.IsChecked = config.AlwaysOnTop;
            }
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
                onAccept: () =>
                {
                    this.onResetData();
                    this.updateState();
                }
            );
            confirmReset.Owner = this;
            confirmReset.ShowDialog();
        }
    }
}
