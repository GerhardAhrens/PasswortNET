//-----------------------------------------------------------------------
// <copyright file="DecomposedFilePath.cs" company="www.lifeprojects.de">
//     Class: DecomposedFilePath
//     Copyright © www.lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - www.lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>09.06.2022 06:52:02</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public sealed class DecomposedFilePath
    {
        public DecomposedFilePath(string filePath)
        {
            FullFilePath = Path.GetFullPath(filePath);
        }

        // "c:\myfiles\file(4).txt"
        public string FullFilePath { get; }

        // "file" or "file(1)"
        public string FileNameWithoutExt => Path.GetFileNameWithoutExtension(FullFilePath);

        // "file(13)" -> "file"
        public string FileNameWithoutExtAndSuffix => FileNameWithoutExt.Substring(0, FileNameWithoutExt.Length - Suffix.Length); // removes suffix

        // ".txt"
        public string Extenstion => Path.GetExtension(FullFilePath);

        // "c:\myfiles"
        public string DirectoryPath => Path.GetDirectoryName(FullFilePath);

        // "file(23)" -> "23", file -> stirng.Empty
        public string Suffix
        {
            get
            {
                // we want to extract suffix from file name, e.g. "(34)" from "file(34)"
                // I am not good at regex, but I hope it will work correctly

                var regex = new Regex(@"\([0-9]+\)$");
                Match match = regex.Match(FileNameWithoutExt);

                if (!match.Success) return string.Empty; // suffix not found

                return match.Value; // return "(number)"
            }
        }

        // tranlates suffix "(33)" to 33. If suffix is does not exist (string.empty), returns null (int?)
        public int? SuffixAsInt
        {
            get
            {
                if (Suffix == string.Empty) return null;

                string numberOnly = Suffix.Substring(1, Suffix.Length - 2); // remove '(' from beginning and ')' from end

                return int.Parse(numberOnly);
            }
        }

        // e.g. input is suffix: 56 then it changes file name from "file(34)" to "file(56)"
        public DecomposedFilePath ReplaceSuffix(int? suffix) // null - removes suffix
        {
            string strSuffix = suffix is null ? string.Empty : $"({suffix})"; // add ( and )

            string path = Path.Combine(DirectoryPath, FileNameWithoutExtAndSuffix + strSuffix + Extenstion); // build full path

            return new DecomposedFilePath(path);
        }

        public DecomposedFilePath GetFirstFreeFilePath(IEnumerable<FileInfo> filesInDir)
        {
            var decomposed = filesInDir
                .Select(x => new DecomposedFilePath(x.FullName))
                .Where(x => string.Equals(Extenstion, x.Extenstion, StringComparison.OrdinalIgnoreCase))
                .Where(x => string.Equals(FileNameWithoutExtAndSuffix, x.FileNameWithoutExtAndSuffix, StringComparison.OrdinalIgnoreCase))
                .Where(x => string.Equals(DirectoryPath, x.DirectoryPath, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (decomposed.Count() == 0)
            {
                return this;
            }

            int? firstFreeSuffix = Enumerable.Range(1, int.MaxValue) // start numbering duplicates from 1
                                  .Select(x => (int?)x) // change to int? because SuffixAsInt is of that type
                                  .Except(decomposed.Select(x => x.SuffixAsInt)) // remove existing suffixes
                                  .First(); // get first free suffix

            return ReplaceSuffix(firstFreeSuffix);
        }

        public override string ToString() => FullFilePath;
    }
}
