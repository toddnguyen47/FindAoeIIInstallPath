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

namespace FindAoEII
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void SelectAllTextbox(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                tb.SelectAll();
                System.Windows.Clipboard.SetText(tb.Text);
            }
        }

        /// <summary>
        /// Find the path of Age of Empires II installation.
        /// Reference: https://github.com/AoE2CommunityGitHub/WololoKingdoms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindPath_Click(object sender, RoutedEventArgs e)
        {
            string regKey = "";
            // If the user is running on 64 bit
            if (Environment.Is64BitOperatingSystem)
            {
                regKey = "Software\\WOW6432Node\\Microsoft\\DirectPlay\\Applications\\Age of Empires II - The Conquerors Expansion";
            }
            // Else if user is running on 32 bit
            else
            {
                regKey = "Software\\Microsoft\\DirectPlay\\Applications\\Age of Empires II - The Conquerors Expansion";
            }
            this.FindPathOfAoe(regKey);
        }


        /// <summary>
        /// Helper function to find the path of Age of Empires II based on the regKeyInput
        /// </summary>
        /// <param name="regKeyInput">The input registry key</param>
        private void FindPathOfAoe(string regKeyInput)
        {
            var reg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(regKeyInput);
            if (null != reg)
            {
                var installLocation = reg.GetValue("CurrentDirectory");
                this.pathTextBox.Text = installLocation.ToString();
            }
            else
            {
                MessageBox.Show("Could not find AoE Installation Folder");
            }
        }

        private void copyButton_Click(object sender, RoutedEventArgs e)
        {
            if (null != this.pathTextBox && "" != this.pathTextBox.Text.Trim())
            {
                this.pathTextBox.Focus();
                this.pathTextBox.SelectAll();
                Clipboard.SetText(this.pathTextBox.Text);
                MessageBox.Show("Copied path to board!", "Copied", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
