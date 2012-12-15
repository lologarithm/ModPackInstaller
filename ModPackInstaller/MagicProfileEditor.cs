using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace ModPackInstaller
{
    class MagicProfileEditor
    {
        // TODO: Remove REGEX and replace with a real XML handler
        private static string MLProfileBaseText = "<Profile\n  <Name=\"%package_name%\">\n  <MinecraftJar=\"%install_location%\\\\bin\\\\minecraft.jar\">\n  <MaxMemory=\"1024\">\n  <BaseDir=\"%install_location%\">\n";
        private static string MLProfileModBaseText = "  <Mod\n    <File=\"%mod_file%\">\n    <Active=\"true\">\n  >\n";
        private static string MLProfileBaseEnding = "  <InactiveExternalMods=>\n>\n";

        public static string CreateProfile(string[] mod_files)
        {
            StringBuilder sb = new StringBuilder(MLProfileBaseText);
            foreach (string mod_file in mod_files)
                sb.Append(  MLProfileModBaseText.Replace("%mod_file%", mod_file).Replace(@"\", @"\\")  );
            sb.Append(MLProfileBaseEnding);

            return sb.ToString();
        }

        /// <summary>
        /// Currently this is not used at all... I just was playin around with what I could do.
        /// </summary>
        /// <param name="profiles_raw_data"></param>
        /// <returns></returns>
        private static List<MLProfile> ReadProfiles(string profiles_raw_data)
        {
            MatchCollection profile_matches = Regex.Matches(profiles_raw_data, "\\<Profile\\s*\\n\\s*\\<Name=\"(\\w*)\"(.*?)\\n\\>\\n", RegexOptions.Singleline);
            int prof_index = 0;
            List<MLProfile> profile_list = new List<MLProfile>();

            foreach ( Match profile_reg in profile_matches )
            {
                MLProfile profile = new MLProfile();
                profile.ProfileName = profile_reg.Groups[1].Value;
                profile.profile_char_start_index = profile_reg.Index;
                profile.profile_index = prof_index++;
                profile.profile_length = profile_reg.Length;

                profile_list.Add(profile);
            }

            return profile_list;
        }

        /// <summary>
        /// Sets the last profile in the list to be the currently active profile
        /// </summary>
        /// <param name="profile_file_text">Text of the magic launcher config file to fix</param>
        /// <returns></returns>
        private static string SetDefaultProfile(string profile_file_text)
        {
            MatchCollection profile_matches = Regex.Matches(profile_file_text, "\\<Profile\\s*\\n\\s*\\<Name=\"(\\w*)\"(.*?)\\n\\>\\n", RegexOptions.Singleline);
            return Regex.Replace(profile_file_text, "\\<ActiveProfileIndex=\"\\d+\"\\>", "<ActiveProfileIndex=\"" + (profile_matches.Count - 1) + "\">");
        }

        private static void CreateNewConfig()
        {

        }

        private static string DeleteProfilesNamed(string profile_file_text, string profile_name)
        {
            Match profile_match = null;
            do
            {
                profile_match = Regex.Match(profile_file_text, "\\<Profile\\s*\\n\\s*\\<Name=\"(" + profile_name + ")\"(.*?)\\n\\>\\n", RegexOptions.Singleline);
                if (!profile_match.Success)
                    break;
                
                profile_file_text = profile_file_text.Remove(profile_match.Index, profile_match.Length);

            } while (profile_match.Success);

            return profile_file_text;
        }


        /// <summary>
        /// Given a Minecraft directory and install directory this method will create or update the MagicLauncher profile for this mod pack
        /// </summary>
        /// <param name="mc_dir">Directory that MC is installed at (usually %appdata%/.minecraft)</param>
        /// <param name="install_dir">Directory that the Launcher and internal mods will be installed at.</param>
        /// <returns></returns>
        public static ConfigStatus CreateOrUpdateConfig(string pack_name, string mc_dir, string new_mc_dir, string install_dir, string packaged_profile)
        {
            ConfigStatus return_value = ConfigStatus.CreatedNew;
            string magic_dir = Path.Combine(mc_dir, "magic");

            if (!Directory.Exists(magic_dir))
            {
                Directory.CreateDirectory(magic_dir);
            }

            string config_string = "";
            string magic_config_file = Path.Combine(magic_dir, "MagicLauncher.cfg");

            if (File.Exists(magic_config_file))
            {
                return_value = ConfigStatus.UpdatedExisting;
                string file_contents = File.ReadAllText(magic_config_file);
                config_string = DeleteProfilesNamed(file_contents, pack_name);
            }

            if (packaged_profile != "")
                config_string = config_string + packaged_profile;
            else
                config_string = config_string + CreateProfile(Directory.GetFiles(Path.Combine(install_dir, "internal_mods")));
            if (config_string[config_string.Length - 1] != '\n')
                config_string += "\n";
            // So that magic launcher can properly read the escaped backslashes
            config_string = config_string.Replace("%minecraft_location%", mc_dir.Replace(@"\", @"\\"));
            config_string = config_string.Replace("%install_location%", new_mc_dir.Replace(@"\", @"\\"));
            config_string = config_string.Replace("%package_name%", pack_name.Replace(@"\", @"\\"));
            config_string = config_string.Replace("%int_mod_location%", Path.Combine(install_dir, "internal_mods").Replace(@"\", @"\\"));

            config_string = SetDefaultProfile(config_string);

            File.WriteAllText(magic_config_file, config_string);

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

    class MLProfile
    {
        public string ProfileName;
        public int profile_index;
        public int profile_char_start_index;
        public int profile_length;
    }
}
