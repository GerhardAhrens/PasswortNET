//-----------------------------------------------------------------------
// <copyright file="AssemblyMetaInfo.cs" company="www.pta.de">
//     Class: AssemblyMetaInfo
//     Copyright � www.pta.de 2024
// </copyright>
//
// <author>Gerhard Ahrens - www.pta.de</author>
// <email>gerhard.ahrens@pta.de</email>
// <date>03.12.2024 07:49:42</date>
//
// <summary>
// Klasse f�r 
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Core
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;

    using ModernBaseLibrary.Core;

    public class AssemblyMetaInfo : IAssemblyInfo
    {
        public string PacketName => "PasswordNET";

        public Version PacketVersion => new Version(1, 0, 2025, 10);

        public string AssemblyName => "PasswordNET";

        public Version AssemblyVersion => new Version(1,0,2025,1);

        public string Description => "Passwortverwaltung f�r Web-Zup�nge, Passw�rter, PIN, Lizenzen";

        public string Unternehmen => "Lifeprojects.de";

        public string Copyright => "� Lifeprojects.de 2025";

        public string GitRepository => "https://github.com/GerhardAhrens/PasswortNET";

        public string FrameworkVersion => RuntimeInformation.FrameworkDescription;

        public string OSPlatform => RuntimeInformation.OSArchitecture.ToString();

        public string RuntimeIdentifier => RuntimeInformation.RuntimeIdentifier;
    }
}
