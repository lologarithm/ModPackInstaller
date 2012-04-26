using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ionic.Zip;
using System.Windows.Threading;
using System.Threading;

namespace ModPackInstaller
{
    class Installer
    {
        private string PackageName = "";
        private string TemporaryZipDownload = "";
        private string TemporaryTextureDownload = "";
        private string InstallDirectory = "";
        private string MCInstallDirectory = "";
        private MainWindow Window;


        public Installer(MainWindow window, string package_name, string temp_zip_file, string install_dir, string mc_install_dir, string texture_pack)
        {
            this.Window = window;
            this.InstallDirectory = install_dir;
            this.TemporaryZipDownload = temp_zip_file;
            this.TemporaryTextureDownload = texture_pack;
            this.MCInstallDirectory = mc_install_dir;
            this.PackageName = package_name;
        }

        /// <summary>
        /// Does the installation steps for all the mods and configs.
        /// Calls back using the Dispatcher because it is assumed this will be on a separate thread.
        /// TODO: clean this method up, it is getting icky.
        /// </summary>
        public void DoInstallation()
        {
            string temp_unpacked_dir = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName();

            string new_mc_appdata_dir = InstallDirectory + @"\.minecraft";

            Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateInstallProgressAndText), 5, "Setting up minecraft directory");

            // If MC Directory or the \bin directories don't exist, throw error.
            if (!Directory.Exists(MCInstallDirectory) && !Directory.Exists( Path.Combine(MCInstallDirectory, "bin")) )
            {
                Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateInstallProgressAndText), 1, "Warning! No minecraft folder at:" + MCInstallDirectory);
                return;
            }

            if (!Directory.Exists(InstallDirectory))
            {
                Directory.CreateDirectory(InstallDirectory);
            }

            if (!Directory.Exists(new_mc_appdata_dir))
            {
                Directory.CreateDirectory(new_mc_appdata_dir);
                Utilities.DirectoryCopy(MCInstallDirectory + @"\bin\", new_mc_appdata_dir + @"\bin", true);
                Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateInstallProgressAndText), 30, "Setting up minecraft directory");
                Utilities.DirectoryCopy(MCInstallDirectory + @"\resources\", new_mc_appdata_dir + @"\resources", true);
            }


            Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateInstallProgressAndText), 50, "Unpacking download");

            var options = new ReadOptions { StatusMessageWriter = System.Console.Out };
            using (ZipFile zip = ZipFile.Read(TemporaryZipDownload, options))
            {
                zip.ExtractAll(temp_unpacked_dir);
            }

            File.Delete(TemporaryZipDownload);

            // TODO: Decide how we want to cleanup old version of data laying around like the internal_mods directory.

            // Copy all directories within the minecraft directory
            Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateInstallProgressAndText), 75, "Moving modded minecraft files.");
            Utilities.MoveFiles(temp_unpacked_dir + @"\minecraft\", new_mc_appdata_dir, true);

            Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateInstallProgressAndText), 80, "Moving internal mod files.");
            // Move all internal mods into the the internal mods directory
            Utilities.MoveFiles(temp_unpacked_dir + @"\i_mods", InstallDirectory + @"\internal_mods\", false);

            Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateInstallProgressAndText), 90, "Moving launcher files.");
            // Move all base files to install directory
            Utilities.MoveFiles(temp_unpacked_dir, InstallDirectory, false);

            Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateInstallProgressAndText), 93, "Updating Config.");
            // Now setup the MagicLauncher config
            MagicProfileEditor.ConfigStatus status = MagicProfileEditor.CreateOrUpdateConfig(PackageName, MCInstallDirectory, new_mc_appdata_dir, InstallDirectory);

            Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateInstallProgressAndText), 95, "Cleaning up temp files.");
            // Finally, delete all temp files
            Directory.Delete(temp_unpacked_dir, true);

            if ( TemporaryTextureDownload != "" )
            {
                Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateInstallProgressAndText), 99, "Waiting for Texture Download");
                while (!Window.TexturePackDownloaded)
                {
                    Thread.Sleep(100);
                }
                
                DoInstallTexturePack(new_mc_appdata_dir);
            }

            // If user already has MagicLauncher we let them know the name of the new profile to use.
            string new_text = "Installation Complete: ";
            if (status == MagicProfileEditor.ConfigStatus.CreatedNew)
                new_text += "Go to your install directory and run the exe!";
            else if (status == MagicProfileEditor.ConfigStatus.UpdatedExisting)
                new_text += "When running magic launcher use the new profile.";

            Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateInstallProgressAndText), 100, new_text);

            System.Diagnostics.Process.Start(InstallDirectory);
        }


        public void DoInstallTexturePack(string mc_dir)
        {
            Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateInstallProgressAndText), 99, "Installing Texture Pack");
            
            string file_name = Path.GetFileName(TemporaryTextureDownload);
            string dir = Path.Combine(mc_dir, "texturepacks");
            string file_full = Path.Combine(dir, file_name);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if ( File.Exists(file_full) )
                File.Delete(file_full);

            File.Move(TemporaryTextureDownload, file_full);
        }
    }
}
