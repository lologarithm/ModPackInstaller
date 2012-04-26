using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.ComponentModel;
using Ionic.Zip;
using System.IO;
using System.Threading;

namespace ModPackInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string temp_zip_file = "";
        private string install_dir = "";

        // 1. Prompt user to select installation location.
        // 2. Download .zip
        // 3. unzip
        // 4. Move bin/mod directory to .minecraft folder
        // 5. Check to see if magiclaucher .cfg exists.
        // 6. If exists, add new profile, if not, create new cfg
        // 7. Place MagicLauncher with external mods in the installation location.

        public MainWindow()
        {
            InitializeComponent();


            // Default install will be in mydocuments
            installLocation.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\IndustrialLauncher";
            install_dir = installLocation.Text;
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (dl_button.IsEnabled)
            {
                // Prevent user from hittin button again.
                dl_button.IsEnabled = false;
                //
                progressBar.Visibility = System.Windows.Visibility.Visible;

                // DL zip into temp file.
                temp_zip_file = System.IO.Path.GetTempFileName();

                // Async DL class that will update the progress bar as it downloads.
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                
                // TODO: make this configurable so we don't recompile everytime we change the location. This could also let other people use this.
                webClient.DownloadFileAsync(new Uri("http://lologarithm.dyndns.org/minecraft/industrial_client.zip"), temp_zip_file);
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // Update text with total bytes and % download
            StringBuilder sb = new StringBuilder("Downloading: ").Append(String.Format("{0:0.00}", Math.Round(e.BytesReceived / (1024 * 1024.0), 2)));
            sb.Append("mb / ").Append(Math.Round(e.TotalBytesToReceive / (1024*1024.0), 2)).Append("mb (");
            sb.Append(e.ProgressPercentage).Append("%)");
            UpdateProgressAndText(e.ProgressPercentage, sb.ToString());
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            startInstallButton.IsEnabled = true;
            UpdateProgressAndText(0, "Download Complete, Ready to Install.");
        }

        private void btn_SelectDirectory_clicked(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = install_dir;
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                install_dir = dialog.SelectedPath;
            }

            installLocation.Text = install_dir;
        }

        private void btn_Install_clicked(object sender, RoutedEventArgs e)
        {
            // When install button is clicked we kick off new thread so that UI thread doesn't freeze.
            Installer i = new Installer(this, temp_zip_file, install_dir);

            Thread install_thread = new Thread(i.DoInstallation);
            install_thread.Start();
        }

        public void UpdateProgressAndText(double progress, string status)
        {
            progressBar.Value = progress;
            status_text.Text = status;
        }

        public void DisableInstallButton()
        {
            startInstallButton.IsEnabled = false;
        }
    }
}
