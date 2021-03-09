using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
