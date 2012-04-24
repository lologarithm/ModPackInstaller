using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndustrialInstaller
{
    class Utilities
    {
        public static void MoveFiles(string from, string to)
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
            }
        }

        public static ConfigStatus CreateOrUpdateConfig(string mc_dir, string install_dir)
        {
            ConfigStatus return_value = ConfigStatus.CreatedNew;

            if (!Directory.Exists(mc_dir + @"\magic\"))
            {
                Directory.CreateDirectory(mc_dir + @"\magic\");
            }

            string config_string = "";

            if (!File.Exists(mc_dir + @"\magic\" + "MagicLauncher.cfg"))
            {
                config_string = IndustrialInstaller.Properties.Resources.config_string;
            }
            else
            {
                // TODO: If player already has a profile called "IndustrialMinecraft" remove it so that a new one can be created.

                config_string = IndustrialInstaller.Properties.Resources.profile_string;
                
                return_value = ConfigStatus.UpdatedExisting;
            }

            config_string = config_string.Replace("%mc_jar%", mc_dir + @"\bin\" + "minecraft.jar");
            config_string = config_string.Replace("%mc_indust_jar%", mc_dir + @"\bin\" + "industrial_minecraft.jar");
            config_string = config_string.Replace("%i_mod%", install_dir + @"\internal_mods");

            // So that magic launcher can properly read the escaped backslashes
            config_string = config_string.Replace(@"\", @"\\");

            File.AppendAllText(mc_dir + @"\magic\" + "MagicLauncher.cfg", config_string);


            // Setup NEI options if they dont exist.
            if (!Directory.Exists(mc_dir + @"\config"))
            {
                Directory.CreateDirectory(mc_dir + @"\config");
            }

            if (!File.Exists(mc_dir + @"\config\NEI.cfg"))
            {
                byte[] file_data = IndustrialInstaller.Properties.Resources.NEI;
                File.WriteAllBytes(mc_dir + @"\config\NEI.cfg", file_data);
            }

            return return_value;
        }


        public enum ConfigStatus
        {
            CreatedNew,
            UpdatedExisting
        }
    }
}
