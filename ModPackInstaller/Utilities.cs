using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ModPackInstaller
{
    class Utilities
    {
        /// <summary>
        /// Given a directory from and to this method will copy all files overwritting existing files.
        /// Optionally do this recursively
        /// </summary>
        /// <param name="from">Directory to copy from</param>
        /// <param name="to">Directory to copy to</param>
        /// <param name="recursive">if true will copy folders recursively</param>
        public static void MoveFiles(string from, string to, bool recursive)
        {
            if (Directory.Exists(from))
            {
                if (to.LastIndexOf(@"\") != to.Length - 1)
                {
                    to += @"\";
                }

                string[] all_files = Directory.GetFiles(from);

                if (!Directory.Exists(to))
                {
                    Directory.CreateDirectory(to);
                }

                foreach (string file in all_files)
                {
                    int slash_index = file.LastIndexOf(@"\");
                    string file_name = file.Substring(slash_index + 1, file.Length - slash_index - 1);

                    // Make sure to delete old version before moving.
                    if (File.Exists(to + file_name))
                    {
                        File.Delete(to + file_name);
                    }
                    
                    File.Move(file, to + file_name);
                }

                if (recursive)
                {
                    string[] sub_dirs = Directory.GetDirectories(from);

                    foreach (string sub_dir in sub_dirs)
                    {
                        int dir_slash_index = sub_dir.LastIndexOf(@"\");
                        string dir_name = sub_dir.Substring(dir_slash_index + 1, sub_dir.Length - dir_slash_index - 1);
                        MoveFiles(sub_dir, to + dir_name, true);
                    }

                }
            }
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }


            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, false);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {

                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        /// <summary>
        /// Given a Minecraft directory and install directory this method will copy all modloader mods, internal mods, MagicLauncher profile, and NEI profile to the correct locations.
        /// TODO: Separate out the different config copying. This will make code easier to read and easier to modify. Specifically we want to enable magiclauncher profile in a different
        /// directory than the options.txt and NEI.cfg
        /// </summary>
        /// <param name="mc_dir">Directory that MC is installed at (usually %appdata%/.minecraft)</param>
        /// <param name="install_dir">Directory that the Launcher and internal mods will be installed at.</param>
        /// <returns></returns>
        public static ConfigStatus CreateOrUpdateConfig(string mc_dir, string new_mc_dir, string install_dir)
        {
            ConfigStatus return_value = ConfigStatus.CreatedNew;

            if (!Directory.Exists(mc_dir + @"\magic\"))
            {
                Directory.CreateDirectory(mc_dir + @"\magic\");
            }

            string config_string = "";

            // TODO: Possibly add the resources for MagicLauncher profile to be part of zip instead of compiled.
            // This will allow for more flexibilty in the package without having to recompile code.
            if (!File.Exists(mc_dir + @"\magic\" + "MagicLauncher.cfg"))
            {
                config_string = ModPackInstaller.Properties.Resources.config_string;
            }
            else
            {
                // TODO: If player already has a profile called "IndustrialMinecraft" remove it so that a new one can be created.

                config_string = ModPackInstaller.Properties.Resources.profile_string;
                return_value = ConfigStatus.UpdatedExisting;
            }

            config_string = config_string.Replace("%mc_jar%", mc_dir);
            config_string = config_string.Replace("%indust_mc%", new_mc_dir);
            config_string = config_string.Replace("%i_mod%", install_dir + @"\internal_mods");

            // So that magic launcher can properly read the escaped backslashes
            config_string = config_string.Replace(@"\", @"\\");

            File.AppendAllText(mc_dir + @"\magic\" + "MagicLauncher.cfg", config_string);

            return return_value;
        }

        /// <summary>
        /// States after running installation of MagicLauncher configs
        /// This lets you tailor message to end user.
        /// </summary>
        public enum ConfigStatus
        {
            CreatedNew,
            UpdatedExisting
        }
    }
}
