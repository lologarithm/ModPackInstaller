using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ionic.Zip;
using System.Windows.Threading;
using System.Threading;

namespace IndustrialInstaller
{
    class Installer
    {
        private string TemporaryZipDownload = "";
        private string InstallDirectory = "";
        private MainWindow Window;


        public Installer(MainWindow window, string temp_zip_file, string install_dir)
        {
            this.Window = window;
            this.InstallDirectory = install_dir;
            this.TemporaryZipDownload = temp_zip_file;
        }

        /// <summary>
        /// Does the installation steps for all the mods and configs.
        /// Calls back using the Dispatcher because it is assumed this will be on a separate thread.
        /// </summary>
        public void DoInstallation()
        {
            // Disable Install Button so they dont click twice.
            Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(Window.DisableInstallButton));

            string temp_unpacked_dir = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName();

            string mc_appdata_dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.minecraft";
            string new_mc_appdata_dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.industrial_minecraft\.minecraft";

            Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateProgressAndText), 5, "Setting up minecraft directory");

            if (!Directory.Exists(mc_appdata_dir))
            {
                Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateProgressAndText), 1, "Warning! No minecraft folder at:" + mc_appdata_dir );
                return;
            }

            if (!Directory.Exists(InstallDirectory))
            {
                Directory.CreateDirectory(InstallDirectory);
            }

            if (!Directory.Exists(new_mc_appdata_dir))
            {
                Directory.CreateDirectory(new_mc_appdata_dir);
                // TODO: Copy the important folders over to here from base MC folder. Is there any I am missing?
                Utilities.DirectoryCopy(mc_appdata_dir + @"\bin\", new_mc_appdata_dir + @"\bin", true);
                Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateProgressAndText), 30, "Setting up minecraft directory");
                Utilities.DirectoryCopy(mc_appdata_dir + @"\resources\", new_mc_appdata_dir + @"\resources", true);
            }


            Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateProgressAndText), 50, "Unpacking download");

            var options = new ReadOptions { StatusMessageWriter = System.Console.Out };
            using (ZipFile zip = ZipFile.Read(TemporaryZipDownload, options))
            {
                // This call to ExtractAll() assumes:
                //   - none of the entries are password-protected.
                //   - want to extract all entries to current working directory
                //   - none of the files in the zip already exist in the directory;
                //     if they do, the method will throw.
                zip.ExtractAll(temp_unpacked_dir);
            }
            File.Delete(TemporaryZipDownload);

            // TODO: Decide how we want to cleanup old version of data laying around like the internal_mods directory.

            // Copy all directories within the minecraft directory
            Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateProgressAndText), 75, "Moving modded minecraft files.");
            Utilities.MoveFiles(temp_unpacked_dir + @"\minecraft\", new_mc_appdata_dir, true);

            Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateProgressAndText), 80, "Moving internal mod files.");
            // Move all internal mods into the the internal mods directory
            Utilities.MoveFiles(temp_unpacked_dir + @"\i_mods", InstallDirectory + @"\internal_mods\", false);

            Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateProgressAndText), 90, "Moving launcher files.");
            // Move all base files to install directory
            Utilities.MoveFiles(temp_unpacked_dir, InstallDirectory, false);
            
            Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateProgressAndText), 95, "Updating Config.");
            // Now setup the MagicLauncher config
            Utilities.ConfigStatus status = Utilities.CreateOrUpdateConfig(mc_appdata_dir, new_mc_appdata_dir, InstallDirectory);

            Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateProgressAndText), 99, "Cleaning up temp files.");
            // Finally, delete all temp files
            Directory.Delete(temp_unpacked_dir, true);

            // If user already has MagicLauncher we let them know the name of the new profile to use.
            string new_text = "Installation Complete: ";
            if (status == Utilities.ConfigStatus.CreatedNew)
                new_text += "Go to your install directory and run the exe!";
            else if (status == Utilities.ConfigStatus.UpdatedExisting)
                new_text += "When running magic launcher use profile \"IndustrialMinecraft\".";

            Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string>(Window.UpdateProgressAndText), 100, new_text);

            System.Diagnostics.Process.Start(InstallDirectory);
        }
    }
}
