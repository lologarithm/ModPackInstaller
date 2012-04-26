using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ModPackInstaller
{
    class MagicProfileEditor
    {
        // 1. Able to count total profiles.
        // 2. List profiles by name 
        // 3. Remove profile by name
        // 4. Add profiles

        // 5. Profiles should be stored in .zip not in client
        // 6. Create generalized system for allowing variables in the profile (aka %value%).

        public void InstallProfile(string profile_file)
        {

        }

        private Dictionary<string, MLProfile> ReadProfiles()
        {
            return null;
        }

        private void SetDefaultProfile(int id)
        {

        }

        private void CreateNewConfig()
        {

        }


        /// <summary>
        /// Given a Minecraft directory and install directory this method will copy all modloader mods, internal mods, MagicLauncher profile, and NEI profile to the correct locations.
        /// TODO: Separate out the different config copying.
        /// </summary>
        /// <param name="mc_dir">Directory that MC is installed at (usually %appdata%/.minecraft)</param>
        /// <param name="install_dir">Directory that the Launcher and internal mods will be installed at.</param>
        /// <returns></returns>
        public static ConfigStatus CreateOrUpdateConfig(string pack_name, string mc_dir, string new_mc_dir, string install_dir)
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

            config_string = config_string.Replace("%minecraft_location%", mc_dir);
            config_string = config_string.Replace("%install_location%", new_mc_dir);
            config_string = config_string.Replace("%package_name%", pack_name);
            config_string = config_string.Replace("%int_mod_location%", install_dir + @"\internal_mods");

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

    private class MLProfile
    {
        public string ProfileName;
        public string RawData;
    }
}
