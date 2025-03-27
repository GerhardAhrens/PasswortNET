//-----------------------------------------------------------------------
// <copyright file="AssemblyResource.cs" company="Lifeprojects.de">
//     Class: AssemblyResource
//     Copyright © Lifeprojects.de 2025
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>Gerhard Ahrens@Lifeprojects.de</email>
// <date>27.03.2025 14:33:08</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Resources
{
    using System.IO;
    using System.Reflection;

    public static class EmbeddedResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedResource"/> class.
        /// </summary>
        static EmbeddedResource()
        {
        }

        public static byte[] Extract(string filename)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            using (Stream resFilestream = executingAssembly.GetManifestResourceStream($"{executingAssembly.GetName().Name}.Resources.Picture.{filename}"))
            {
                if (resFilestream == null)
                {
                    return null;
                }

                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
        }
    }
}
