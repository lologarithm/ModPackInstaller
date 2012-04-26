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
using System.Configuration;

namespace ModPackInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Note: BAD CODER - BAD! No logic in code behind!!!!!!
    /// Ah well, this was faster... for now...
    /// </summary>
    public partial class MainWindow : Window
    {
        private string temp_zip_file = "";
        private string temp_texture_file = "";
        private string install_dir = "";
        private string mc_install_dir = "";

        private bool main_zip_downloaded = false;
        public volatile bool TexturePackDownloaded = false;
        private bool install_button_pressed = false;

        public MainWindow()
        {
            InitializeComponent();

            header.Text = ConfigurationManager.AppSettings["header"];
            // Default install will be in my documents
            install_dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ConfigurationManager.AppSettings["package_name"]);
            // default MC directory
            mc_install_dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft");

            installLocation.Text = install_dir;
            mc_install_dir_box.Text = mc_install_dir;
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (dl_button.IsEnabled)
            {
                // Prevent user from hittin button again.
                dl_button.IsEnabled = false;
                //
                DLProgressBar.Visibility = System.Windows.Visibility.Visible;
                DLStatusText.Visibility = System.Windows.Visibility.Visible;

                // DL zip into temp file.
                temp_zip_file = System.IO.Path.GetTempFileName();

                // Async DL class that will update the progress bar as it downloads.
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(MainDLCompleted);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DLProgressChanged);
                webClient.DownloadFileAsync(new Uri(ConfigurationManager.AppSettings["mod_zip_url"]), temp_zip_file);
            }
        }

        private void DLProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // Update text with total bytes and % download
            StringBuilder sb = new StringBuilder("Downloading: ").Append(String.Format("{0:0.00}", Math.Round(e.BytesReceived / (1024 * 1024.0), 2)));
            sb.Append("mb / ").Append(Math.Round(e.TotalBytesToReceive / (1024*1024.0), 2)).Append("mb (");
            sb.Append(e.ProgressPercentage).Append("%)");
            UpdateDLProgressAndText(e.ProgressPercentage, sb.ToString());
        }

        private void MainDLCompleted(object sender, AsyncCompletedEventArgs e)
        {
            main_zip_downloaded = true;
            UpdateDLProgressAndText(0, "Download Complete, Ready to Install.");
            UpdateInstallProgressAndText(0, "Main Download Complete, Ready to Install.");

            // After main DL done, start up texture DL if it is needed.
            if ( ConfigurationManager.AppSettings["texture_zip_url"] != "" )
            {
                Uri texture_uri = new Uri(ConfigurationManager.AppSettings["texture_zip_url"]);
                temp_texture_file = System.IO.Path.Combine(System.IO.Path.GetTempPath(), texture_uri.Segments[texture_uri.Segments.Length - 1]);

                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(TextureDLCompleted);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DLProgressChanged);
                webClient.DownloadFileAsync(texture_uri, temp_texture_file);
            }

            // If people already pressed install before DL ready just kick it off
            if (install_button_pressed)
                StartInstallation();
        }

        private void TextureDLCompleted(object sender, AsyncCompletedEventArgs e)
        {
            TexturePackDownloaded = true;
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
            DisableInstallButton();
            // When install button is clicked we kick off new thread so that UI thread doesn't freeze.
            if (main_zip_downloaded)
            {
                StartInstallation();
            }
            else
            {
                // wait until download is ready to click install
                install_button_pressed = true;
            }
        }

        private void StartInstallation()
        {
            Installer installer = new Installer(this, ConfigurationManager.AppSettings["package_name"], temp_zip_file, install_dir, mc_install_dir, temp_texture_file);

            Thread install_thread = new Thread(installer.DoInstallation);
            install_thread.Start();
        }

        public void UpdateInstallProgressAndText(double progress, string status)
        {
            InstallProgressBar.Value = progress;
            InstallStatusText.Text = status;
        }

        public void UpdateDLProgressAndText(double progress, string status)
        {
            DLProgressBar.Value = progress;
            DLStatusText.Text = status;
        }
    
        public void DisableInstallButton()
        {
            startInstallButton.IsEnabled = false;
        }

        private void btn_SelectMCDir_clicked(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = install_dir;
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                mc_install_dir = dialog.SelectedPath;
            }

            mc_install_dir_box.Text = mc_install_dir;
        }
    }
}
