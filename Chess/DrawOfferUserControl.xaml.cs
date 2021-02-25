using System;
using System.Windows;
using System.Windows.Controls;

namespace Chess
{
    public partial class DrawOfferUserControl : UserControl
    {
        public DrawOfferUserControl()
        {
            InitializeComponent();
        }

        public event EventHandler Yessed;

        public event EventHandler Noed;

        private void No_Click(object sender, RoutedEventArgs e)
        {
            Noed?.Invoke(this, EventArgs.Empty);
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            Yessed?.Invoke(this, EventArgs.Empty);
        }
    }
}
