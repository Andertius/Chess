using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace Chess.UserControls
{
    public partial class SettingsUserControl : UserControl
    {
        public SettingsUserControl()
        {
            InitializeComponent();

            ReadFile();
            SettingsClosed += SaveToFile;

            WhiteTimeTextBox.IsEnabled = TimedGames.IsChecked ?? false;
            BlackTimeTextBox.IsEnabled = TimedGames.IsChecked ?? false;

            if (TimedGames.IsChecked == false)
            {
                WhiteTimeTextBlock.Foreground = Brushes.Gray;
                BlackTimeTextBlock.Foreground = Brushes.Gray;
            }
            else
            {
                WhiteTimeTextBlock.Foreground = Brushes.Black;
                BlackTimeTextBlock.Foreground = Brushes.Black;
            }
        }

        public event EventHandler SettingsClosed;

        public void Exit(object sender, RoutedEventArgs e)
        {
            SettingsClosed?.Invoke(this, EventArgs.Empty);
        }

        public void ReadFile()
        {
            var doc = new XmlDocument();
            doc.Load("Settings.xml");

            PlaySounds.IsChecked = doc.SelectSingleNode("Settings/PlaySounds").InnerText == "True";
            HighlightMoves.IsChecked = doc.SelectSingleNode("Settings/HighlightMoves").InnerText == "True";
            ShowLegalMoves.IsChecked = doc.SelectSingleNode("Settings/ShowLegalMoves").InnerText == "True";
            TimedGames.IsChecked = doc.SelectSingleNode("Settings/TimedGames").InnerText == "True";
            ResignConfirmation.IsChecked = doc.SelectSingleNode("Settings/ResignConfirmation").InnerText == "True";
            AutoQueen.IsChecked = doc.SelectSingleNode("Settings/AutoQueen").InnerText == "True";

            WhiteTimeTextBox.Text = doc.SelectSingleNode("Settings/WhiteTime").InnerText;
            BlackTimeTextBox.Text = doc.SelectSingleNode("Settings/BlackTime").InnerText;
        }

        private void SaveToFile(object sender, EventArgs e)
        {
            Visibility = Visibility.Collapsed;

            var xmlDoc = new XmlDocument();
            XmlDeclaration xmlDecl = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes");

            XmlNode rootNode = xmlDoc.CreateElement("Settings");
            xmlDoc.AppendChild(rootNode);
            xmlDoc.InsertBefore(xmlDecl, rootNode);

            XmlNode playSounds = xmlDoc.CreateElement("PlaySounds");
            playSounds.InnerText = $"{PlaySounds.IsChecked}";

            XmlNode highlightMoves = xmlDoc.CreateElement("HighlightMoves");
            highlightMoves.InnerText = $"{HighlightMoves.IsChecked}";

            XmlNode showLegalMoves = xmlDoc.CreateElement("ShowLegalMoves");
            showLegalMoves.InnerText = $"{ShowLegalMoves.IsChecked}";

            XmlNode timedGames = xmlDoc.CreateElement("TimedGames");
            timedGames.InnerText = $"{TimedGames.IsChecked}";

            XmlNode whiteTime = xmlDoc.CreateElement("WhiteTime");
            whiteTime.InnerText = $"{WhiteTimeTextBox.Text}";

            XmlNode blackTime = xmlDoc.CreateElement("BlackTime");
            blackTime.InnerText = $"{BlackTimeTextBox.Text}";

            XmlNode resignConfirmation = xmlDoc.CreateElement("ResignConfirmation");
            resignConfirmation.InnerText = $"{ResignConfirmation.IsChecked}";

            XmlNode autoQueen = xmlDoc.CreateElement("AutoQueen");
            autoQueen.InnerText = $"{AutoQueen.IsChecked}";

            rootNode.AppendChild(playSounds);
            rootNode.AppendChild(highlightMoves);
            rootNode.AppendChild(showLegalMoves);
            rootNode.AppendChild(timedGames);
            rootNode.AppendChild(resignConfirmation);
            rootNode.AppendChild(whiteTime);
            rootNode.AppendChild(blackTime);
            rootNode.AppendChild(autoQueen);

            xmlDoc.Save("Settings.xml");
        }

        private void TimedGames_Checked(object sender, RoutedEventArgs e)
        {
            WhiteTimeTextBox.IsEnabled = true;
            BlackTimeTextBox.IsEnabled = true;

            WhiteTimeTextBlock.Foreground = Brushes.Black;
            BlackTimeTextBlock.Foreground = Brushes.Black;
        }

        private void TimedGames_Unchecked(object sender, RoutedEventArgs e)
        {
            WhiteTimeTextBox.IsEnabled = false;
            BlackTimeTextBox.IsEnabled = false;

            WhiteTimeTextBlock.Foreground = Brushes.Gray;
            BlackTimeTextBlock.Foreground = Brushes.Gray;
        }
    }
}
