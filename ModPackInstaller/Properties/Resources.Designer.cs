﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.261
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ModPackInstaller.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ModPackInstaller.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Profile
        ///  &lt;Name=&quot;Default&quot;&gt;
        ///  &lt;MinecraftJar=&quot;%minecraft_location%\\bin\\minecraft.jar&quot;&gt;
        ///  &lt;MaxMemory=&quot;1024&quot;&gt;
        ///  &lt;InactiveExternalMods
        ///    &quot;buildcraft-client-A-core-2.2.14.zip&quot;
        ///    &quot;buildcraft-client-B-builders-2.2.14.zip&quot;
        ///    &quot;buildcraft-client-B-energy-2.2.14.zip&quot;
        ///    &quot;buildcraft-client-B-factory-2.2.14.zip&quot;
        ///    &quot;buildcraft-client-B-transport-2.2.14.zip&quot;
        ///    &quot;industrialcraft-2-client_1.95.jar&quot;
        ///    &quot;NEI_RedPowerPlugin 1.2.2.zip&quot;
        ///    &quot;RedPowerCore-2.0pr5.zip&quot;
        ///    &quot;RedPowerLogic-2.0pr5.zip&quot;
        ///    &quot;RedPowerWiring [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string config_string {
            get {
                return ResourceManager.GetString("config_string", resourceCulture);
            }
        }
        
        internal static byte[] Ionic_Zip {
            get {
                object obj = ResourceManager.GetObject("Ionic_Zip", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Profile
        ///  &lt;Name=&quot;%package_name%&quot;&gt;
        ///  &lt;MinecraftJar=&quot;%install_location%\\bin\\industrial_minecraft.jar&quot;&gt;
        ///  &lt;MaxMemory=&quot;1024&quot;&gt;
        ///  &lt;BaseDir=&quot;%install_location%&quot;&gt;
        ///  &lt;Mod
        ///    &lt;File=&quot;%int_mod_location%\\OptiFine_1.2.5_HD_A5.zip&quot;&gt;
        ///    &lt;Active=&quot;true&quot;&gt;
        ///  &gt;
        ///  &lt;Mod
        ///    &lt;File=&quot;%int_mod_location%\\CodeChickenCore-Client 0.5.2.zip&quot;&gt;
        ///    &lt;Active=&quot;true&quot;&gt;
        ///  &gt;
        ///  &lt;Mod
        ///    &lt;File=&quot;%int_mod_location%\\NotEnoughItems-Client 1.2.2.zip&quot;&gt;
        ///    &lt;Active=&quot;true&quot;&gt;
        ///  &gt;
        ///  &lt;Mod
        ///    &lt;File=&quot;%int_mod_location%\\[1.2.5]ReiMinimap_v3.0_06.zip&quot;&gt;
        ///   [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string profile_string {
            get {
                return ResourceManager.GetString("profile_string", resourceCulture);
            }
        }
    }
}
