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
                string[] all_files = Directory.GetFiles(from);

                if (!Directory.Exists(to))
                {
                    Directory.CreateDirectory(to);
                }

                foreach (string file in all_files)
                {
                    string file_name = Path.GetFileName(file);
                    string full_to = Path.Combine(to, file_name);
                    // Make sure to delete old version before moving.
                    if (File.Exists(full_to))
                    {
                        File.Delete(full_to);
                    }

                    File.Move(file, full_to);
                }

                if (recursive)
                {
                    string[] sub_dirs = Directory.GetDirectories(from);

                    foreach (string sub_dir in sub_dirs)
                    {
                        string dir_name = Path.GetFileNameWithoutExtension(sub_dir);
                        string full_to_dir = Path.Combine(to, dir_name);
                        MoveFiles(sub_dir, full_to_dir, true);
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
    }
}
