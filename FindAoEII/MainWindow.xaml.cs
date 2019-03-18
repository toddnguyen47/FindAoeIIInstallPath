using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            // if (null != this.pathTextBox && "" != this.pathTextBox.Text.Trim())
            // {
            // this.pathTextBox.Focus();
            // this.pathTextBox.SelectAll();
            // Clipboard.SetText(this.pathTextBox.Text);
            // MessageBox.Show("Copied path to board!", "Copied", MessageBoxButton.OK, MessageBoxImage.Information);
            // }
            // Ref: https://stackoverflow.com/a/9732853/6323360

            // Check for empty AoE2 path
            string aoe2Path = this.pathTextBox.Text;
            if (String.IsNullOrEmpty(aoe2Path))
            {
                MessageBox.Show("ERROR! Try checking if you have the correct path to your AoE 2 folder.",
                        "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Ref: https://www.wpf-tutorial.com/misc-controls/the-progressbar-control/
            BackgroundWorker bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress = true;
            bgWorker.DoWork += bgWorker_DoWork;
            bgWorker.ProgressChanged += (sender2, e2) =>
            {
                this.copyProgressBar.IsIndeterminate = true;
                this.copyProgressBarText.Text = "Copying...";
            };
            bgWorker.RunWorkerCompleted += (sender2, e2) =>
            {
                this.copyProgressBar.IsIndeterminate = false;
                this.copyProgressBarText.Text = "";
                MessageBox.Show("Copying Completed!", "Copy Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            };
            // Pass arguments to worker
            // Ref: https://stackoverflow.com/a/4807200/6323360
            Tuple<String> workerArgs = new Tuple<string>(aoe2Path);
            bgWorker.RunWorkerAsync(workerArgs);
        }


        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            (sender as BackgroundWorker).ReportProgress(0);
            string curDir = System.AppDomain.CurrentDomain.BaseDirectory;
            string tempPath = System.IO.Path.Combine(curDir, "AoeII_Voobly");

            Tuple<String> workerArgs = (Tuple<String>)e.Argument;

            MainWindow.DirectoryCopy(tempPath, workerArgs.Item1, true);
        }


        // Reference: https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        public static void DirectoryCopy(string sourceDirPath, string destDirPath, bool copySubDirs)
        {
            // Get subdirectories for the specified directory
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(sourceDirPath);

            if (!dir.Exists)
            {
                var s = String.Format("{0} does not exist", sourceDirPath);
                Console.WriteLine(s);
                System.Diagnostics.Debug.WriteLine(s);
                return;
            }

            System.IO.DirectoryInfo[] subdirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it
            if (false == System.IO.Directory.Exists(destDirPath))
            {
                System.IO.Directory.CreateDirectory(destDirPath);
            }

            // Get the files in the directory and copy them to the new location
            System.IO.FileInfo[] files = dir.GetFiles();
            foreach (System.IO.FileInfo file in files)
            {
                string temppath = System.IO.Path.Combine(destDirPath, file.Name);
                // True to overwrite
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to the new location
            if (true == copySubDirs)
            {
                foreach (System.IO.DirectoryInfo subdir in subdirs)
                {
                    // Recursively call this copy function
                    string temppath = System.IO.Path.Combine(destDirPath, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        
        /// <summary>
        /// Enable the copy files button if the path in the path textbox is not empty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            bool pathExists = !String.IsNullOrEmpty(tb.Text);
            this.copyButton.IsEnabled = pathExists;
        }
    }
}
