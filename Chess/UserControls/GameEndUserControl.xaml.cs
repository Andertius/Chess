using System;
using System.Windows;
using System.Windows.Controls;

namespace Chess.UserControls
{
    /// <summary>
    /// Interaction logic for GameEndUserControl.xaml
    /// </summary>
    public partial class GameEndUserControl : UserControl
    {
        public GameEndUserControl()
        {
            InitializeComponent();
        }

        public event EventHandler NewGamed;

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            NewGamed?.Invoke(this, EventArgs.Empty);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
